# Architecture

This document describes how the codebase is organised today and the conventions we follow when adding features. If something here disagrees with the code, the code wins; please fix the document.

## Overview

Shopee Microservices is a learning project modelled on the Shopee marketplace. So far there is a single service, **ProductCatalog**, which manages products as an SPU (the product itself) with one or more SKUs (the sellable variants), along with brands and product types.

The service is built with .NET 10 and ASP.NET Core controllers. It uses MediatR for commands and queries, FluentValidation, Mapster and Serilog, and stores its data in MongoDB 8, running as a single-node replica set (`rs0`) so transactions work. Docker Compose runs the API and the database together.

The [README](README.md) lists the technologies we plan to use eventually: more services, messaging, gRPC, other databases, a gateway, observability and CI. Most of that hasn't been built yet.

## Project layout

```text
ShopeeMicroservices/
├── ShopeeMicroservices.slnx
├── docker-compose.yml / docker-compose.override.yml   # catalogdb (mongo:8, --replSet rs0) + productcatalog.api (8000→8080)
├── Infrastructure/                       # shared building blocks used by every service
│   ├── Abstractions/Messaging/           # ICommand, IQuery, handler interfaces
│   ├── Errors/                           # Error, ValidationOutcome, Validation<T>, ValidationMessages
│   ├── Abstractions/Behaviors/           # LoggingBehavior, ValidationBehavior, AddDefaultPipelineBehaviors()
│   └── Exceptions/                       # GlobalExceptionHandler, AddGlobalExceptionHandler(), OptimisticLockingException
└── Services/ProductCatalog/
    ├── ProductCatalog.API/               # Program.cs, ProductController, middleware, Dockerfile, http/catalog.http
    ├── ProductCatalog.Application/
    │   ├── Handlers/<UseCase>Handler/    # one folder per command or query
    │   ├── Handlers/Base/                # TransactionalCommandHandler
    │   ├── Handlers/Common/              # IProductCommand, ProductSkuCommand (shared by create and update)
    │   ├── Validators/                   # FluentValidation validators
    │   ├── Errors/ProductErrors.cs       # business errors returned by handlers
    │   ├── Resources/ServerMessages.resx # user-facing messages
    │   ├── DTOs/, Mappers/               # requests, responses, entity → DTO mapping
    │   └── DependencyInjection.cs
    ├── ProductCatalog.Domain/            # entities, IProductCatalogRepository, IUnitOfWork
    └── ProductCatalog.Infrastructure/    # MongoDB repository, unit of work, indexes, seeding
```

Two projects are called "Infrastructure":
- `ShopeeMicroservices/Infrastructure` is the shared library of building blocks.
- `ProductCatalog.Infrastructure` is where the MongoDB code lives.

## Layers and dependencies

Each service is split into four projects in a Clean Architecture style. Requests go through MediatR as commands or queries. Commands and queries use the same repository and data model; there is no separate read store.

```mermaid
graph TD
    API[ProductCatalog.API] --> APP[ProductCatalog.Application]
    API --> INF[ProductCatalog.Infrastructure]
    INF --> APP
    APP --> DOM[ProductCatalog.Domain]
    APP --> BB[Infrastructure - shared]
    INF -.-> MONGO[(MongoDB)]
```

| Project | Depends on | Should not |
|---|---|---|
| Domain | `MongoDB.Driver` (BSON attributes, `IClientSessionHandle`), `MediatR.Contracts` | reference other layers or do I/O |
| Application | Domain, shared Infrastructure | reference ASP.NET Core or ProductCatalog.Infrastructure |
| Infrastructure | Application, Domain | contain business rules |
| API | Application, Infrastructure (wiring only) | call repositories or contain business rules |
| Shared Infrastructure | MediatR, FluentValidation, Mapster, Serilog | reference any service |

As a rule of thumb, here is where each kind of logic belongs:

| Logic | Where it goes |
|---|---|
| Entity invariants | `Create`/`Update` methods on the entities |
| Rules that can be checked from the request alone | Validators |
| Rules that need the database, and orchestration | Handlers |
| HTTP concerns | Controller |
| MongoDB details | Repository |

## Request pipeline and endpoints

`Program.cs` sets up the HTTP pipeline in this order:
1. Swagger (Development only)
2. HTTPS redirection
3. Correlation id logging
4. Serilog request logging
5. `UseExceptionHandler()`, which runs the shared `GlobalExceptionHandler`
6. Controllers

Inside MediatR, `AddDefaultPipelineBehaviors()` registers two behaviors:
1. `LoggingBehavior`
2. `ValidationBehavior`, which runs the validators for commands. A failed validation returns a failed `ValidationOutcome` without calling the handler.

Every endpoint answers **200** with a `ValidationOutcome` body, including business errors. The client checks `isSuccessful`, never the status code:

```json
{ "value": { ... }, "isSuccessful": true, "validationMessages": null }
{ "value": null, "isSuccessful": false, "validationMessages": { "messages": ["Brand not found."] } }
```

Commands without a result (update, delete) have no `value`. Controller actions return the handler's `Validation<T>` (or `ValidationOutcome`) directly, never `IActionResult`, so ASP.NET sends it as the 200 body and the return type alone documents the response in OpenAPI. Only unexpected exceptions return 500 `ProblemDetails`. ASP.NET's own model binding errors (for example malformed JSON) are still 400 `ProblemDetails` from `[ApiController]`.

| Endpoint | Handler | `value` | Validation messages |
|---|---|---|---|
| `POST /api/Product` | `CreateProductCommandHandler` | `CreateProductResult` | validation, unknown brand/category, SKU code taken |
| `GET /api/Product` | `GetProductsQueryHandler` | `GetProductsQueryResult` | — |
| `GET /api/Product/{id}` | `GetProductByIdQueryHandler` | `ProductDetailsDto` | not found |
| `PUT /api/Product/{id}` | `UpdateProductCommandHandler` | — | validation, not found, unknown brand/category, SKU code taken |
| `DELETE /api/Product/{id}` | `DeleteProductCommandHandler` | — | validation, not found |

## How the main flows work

### Commands

Create, update and delete all follow the same path:

```mermaid
sequenceDiagram
    participant C as Client
    participant PC as ProductController
    participant VB as ValidationBehavior
    participant H as Handler (TransactionalCommandHandler)
    participant R as ProductCatalogRepository
    C->>PC: POST / PUT / DELETE
    PC->>VB: Send(command)
    VB-->>C: 200, isSuccessful false, if validation fails
    VB->>H: Handle
    H->>H: Begin transaction
    H->>R: Reads and writes in the transaction
    H->>H: Commit on success, abort on failure
    H-->>PC: ValidationOutcome
    PC-->>C: 200 with the ValidationOutcome
```

- **Create** checks that the brand and category exist and that none of the SKU codes belong to another product, then inserts the product and its SKUs.
- **Update** replaces the product details and the full list of SKUs. `ProductCode` can't be changed. SKUs are matched by `SkuCode`:
  - Existing SKUs that appear in the request are updated in place and keep their ids.
  - New codes are inserted.
  - SKUs left out of the request are deleted.
- **Delete** removes the product together with all of its SKUs.

### Queries

`GetProductById` loads the product. If it exists, it then loads the SKUs, brand and product type in parallel, and returns a `ProductDetailsDto` that includes the SKUs and the price range. `GetProducts` returns every product, without paging.

## Domain model

A `ProductSpu` has:
- a `ProductCode`, name, description and images
- a brand and a category (a `ProductType`)
- a list of `VariantOptions`, such as Color or Storage, each with its allowed values

A `ProductSku` belongs to a product through `SpuId`, and has its own code, price, image and `Attributes`: one value per variant option. SKUs are stored in their own collection rather than inside the product document.

The entities are simple classes with public setters. Business rules live in three places:
- **Entity methods.** `ProductSpu.Create` and `Update` require a name, brand and category. `Update` methods set `LastModified` and never change `ProductCode`, `SpuId` or `SkuCode`.
- **[`ProductCommandValidator`](ShopeeMicroservices/Services/ProductCatalog/ProductCatalog.Application/Validators/ProductCommandValidator.cs),** shared by create and update. It checks:
  - there is at least one SKU, prices are positive, and SKU codes are unique within the product
  - variant option codes and values are unique
  - every SKU has exactly one attribute per variant option, using one of that option's values
  - no two SKUs have the same combination of values, so a product without variant options can only have one SKU
- **The handlers,** for anything that needs the database: the brand and category must exist, and a SKU code can't already belong to another product.

## Persistence

Data lives in the `CatalogDb` database, in four collections: `ProductSpu`, `ProductSkus`, `Brands` and `Types`. The collection names come from `DatabaseSettings`. Ids are GUID v7 strings.

There is no "ignore extra elements" convention, so any document field without a matching property throws when it's read. Plain typed `Find` queries avoid that problem.

Writes go through transactions:
- `MongoUnitOfWork` starts a session per command and makes it available to the repository through `IMongoSessionAccessor`.
- The repository refuses to write outside a transaction.
- Inside a command handler, call the repository one step at a time. A MongoDB session can't be used by parallel calls. Queries don't use a session, so they can run in parallel.

At startup, `DatabaseIndexes` creates non-unique indexes on `ProductSkus.SpuId` and `ProductSkus.SkuCode`. `DatabaseSeeder` then fills empty collections from the JSON files in `Data/SeedData`. Seeded ids are different in every database, and every seeded product gets the first brand and type.

## Cross-cutting concerns

**Errors.** Business errors are never exceptions and never change the status code. FluentValidation results, domain rule checks and "not found / no access" all become `ValidationMessages` in a failed `ValidationOutcome`, sent with HTTP 200:
- `ValidationBehavior` turns validator failures into `ValidationMessages`.
- Handlers return an `Error` (`Error.NotFound` or one of the `ProductErrors`); it converts implicitly to a failed `ValidationOutcome` or `Validation<T>`. Use `Error.NotFound` for "no access" as well, so the response doesn't reveal that the resource exists.

`GlobalExceptionHandler`, in the shared Infrastructure project so every service can use it, handles exceptions:
- `OptimisticLockingException` is the only exception that becomes a user message: 200 with `Error.OptimisticLocking`.
- Anything else is a 500 `ProblemDetails` with a generic message. The details are only logged.

A new service registers it with `builder.Services.AddGlobalExceptionHandler()` and `app.UseExceptionHandler()`.

**Messages.** User-facing validation and error messages live in [`ServerMessages.resx`](ShopeeMicroservices/Services/ProductCatalog/ProductCatalog.Application/Resources/ServerMessages.resx). Visual Studio regenerates `ServerMessages.Designer.cs` whenever the file is edited.
- Look messages up when they're used, so they follow the current culture: `WithMessage(_ => ServerMessages.X)` in validators, and properties in `ProductErrors`.
- Fill placeholders with `string.Format(CultureInfo.CurrentCulture, ...)`.

**Logging.** Serilog reads its settings from configuration, but `appsettings.json` doesn't have a `Serilog` section yet, so nothing is written anywhere. To see logs locally, set these two environment variables:
- `Serilog__Using__0=Serilog.Sinks.Console`
- `Serilog__WriteTo__0__Name=Console`

**Authentication.** Not implemented yet; all endpoints are open.

**API contract.** Requests use the domain types `VariantOption` and `SkuAttribute` directly. A SKU attribute looks like `{ "attributeCode": "color", "value": "Black" }`, where `attributeCode` matches the code of one of the variant options.

## Adding a feature

1. **Create a handler folder.** Add `Handlers/<UseCase>Handler/` with the command or query and its handler.
2. **Commands:**
   - Inherit from `TransactionalCommandHandler`.
   - Return an `Error` for business errors (it becomes a failed `ValidationOutcome`). Don't throw.
   - Add a validator in `Validators/`.
   - Put new messages in `ServerMessages.resx`.
3. **Queries** return DTOs, never entities.
4. **Keep controllers thin:**
   - Map the request with Mapster.
   - Pass the `CancellationToken` to `sender.Send`.
   - Declare the action as `Task<Validation<TResult>>` (queries and commands with a result) or `Task<ValidationOutcome>` (commands without one), and return `sender.Send(...)` as is. Don't return `IActionResult`, `Ok(...)` or other status codes: the outcome carries any errors.
   - Don't add `[ProducesResponseType]` to actions. A new controller copies `ProductController`'s two class-level attributes: an untyped `[ProducesResponseType(StatusCodes.Status200OK)]`, which documents each action's 200 with its return type, and the 500 `ProblemDetails` one. Without the untyped 200, any other response attribute stops ASP.NET from inferring the 200, and it disappears from OpenAPI.
   - After changing an endpoint, run `pnpm gen:openapi && pnpm gen:api` in `client-app` to update `openapi/bff.json` and the TypeScript types. It needs no database: `Program.cs` skips `DatabaseInitializer` when the Swashbuckle CLI loads it.
5. **New repository methods** should pass the `CancellationToken` through to the driver.
6. **Before you open a PR:**
   - Build the solution.
   - Try the endpoint with [`catalog.http`](ShopeeMicroservices/Services/ProductCatalog/ProductCatalog.API/http/catalog.http), including the error cases.
   - Update this document if you changed a convention.

## Known issues

- **SKU codes aren't unique in the database yet.** Older development data may already contain duplicates, so the `SkuCode` index is still non-unique. The handlers reject duplicates, but two simultaneous requests could still get through.
- **`ProductCode` isn't unique either**, and there is no optimistic concurrency, so the last update wins.
- **`BaseEntity` sets `Id` and `CreatedDate` in its constructor**, which also runs when documents are loaded. A document without `CreatedDate` comes back with the current time.
- **There are no tests or CI pipelines yet.**
- **Seeded ids differ per database.** Update the variables at the top of `catalog.http` before using it.
- **The Docker build context is `ShopeeMicroservices/`.** When the API references a new project, add a `COPY` line for its `.csproj` to the [`Dockerfile`](ShopeeMicroservices/Services/ProductCatalog/ProductCatalog.API/Dockerfile).
- **Some code is unused for now:**
  - `Repository<T>`
  - `IProductCatalogQueryService`
  - `OptimisticLockingException`: nothing throws it yet, because entities have no version field
  - `Aggregate<T>` and domain events
  - `Inventory`
  - the brand/type lookups by name

## Quick reference

| Task | Start from |
|---|---|
| Add a command endpoint | [`UpdateProductCommandHandler`](ShopeeMicroservices/Services/ProductCatalog/ProductCatalog.Application/Handlers/UpdateProductCommandHandler/UpdateProductCommandHandler.cs) |
| Add a query endpoint | [`GetProductByIdQueryHandler`](ShopeeMicroservices/Services/ProductCatalog/ProductCatalog.Application/Handlers/GetProductByIdQueryHandler/GetProductByIdQueryHandler.cs) |
| Add validation | [`UpdateProductCommandValidator`](ShopeeMicroservices/Services/ProductCatalog/ProductCatalog.Application/Validators/UpdateProductCommandValidator.cs) |
| Add a business error | [`ProductErrors`](ShopeeMicroservices/Services/ProductCatalog/ProductCatalog.Application/Errors/ProductErrors.cs) + `ServerMessages.resx` |
| Add a repository method | [`ProductCatalogRepository`](ShopeeMicroservices/Services/ProductCatalog/ProductCatalog.Infrastructure/Repositories/ProductCatalogRepository.cs) |
| Add an index | [`DatabaseIndexes`](ShopeeMicroservices/Services/ProductCatalog/ProductCatalog.Infrastructure/Data/DatabaseIndexes.cs) |

**Build and run** (from the repo root):

```bash
dotnet build ShopeeMicroservices/ShopeeMicroservices.slnx
```

```bash
dotnet run --project ShopeeMicroservices/Services/ProductCatalog/ProductCatalog.API
```

**Running the API outside Docker against the Compose database.** Use this connection string:
- `DatabaseSettings__ConnectionString=mongodb://localhost:27017/?directConnection=true`

The replica set member is registered as `catalogdb:27017`, a hostname that only exists inside Docker.

**First Compose run.** Initialise the replica set once:

```bash
docker exec catalogdb mongosh --eval "rs.initiate({_id:'rs0',members:[{_id:0,host:'catalogdb:27017'}]})"
```

## What's next

1. Clean up duplicate SKU codes, then make the `SkuCode` index unique, and add a unique index on `ProductCode`.
2. Return plain DTOs from `GET /api/Product` and add paging.
3. Move `Id`/`CreatedDate` assignment out of the `BaseEntity` constructor.
4. Add optimistic concurrency for product updates.
5. Add tests and a CI pipeline.
6. Configure Serilog sinks.
