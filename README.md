
# 🛒 Shopee Microservices

A practical microservices project inspired by the Shopee platform, built with .NET 10 using Vertical Slice Architecture, CQRS, DDD, Event Sourcing, gRPC, MongoDB, and RabbitMQ. 

On the DevOps side, the infrastructure is fully containerized with Docker, orchestrated using K8s, and automated end-to-end with a GitHub Actions CI/CD pipeline. 

An API Gateway is also included to support secure routing and scalable traffic, alongside complete system observability with OpenTelemetry, Prometheus, and Grafana.

## 📋 Requirements
### ⚙️ Functional Requirements
- [x] Authentication
- [x] Product Catalog
- [x] Shopping Cart
- [X] Payment Integration
- [X] Order Tracking
- [ ] Seller Portal
- [ ] Notifications
- [ ] Reviews & ratings 
- [ ] Recommendation system
### 👀 Non-functional Requirements
- [x] Scalability
- [x] Distributed data consistency
- [x] Availablity
- [x] Fault-tolerance
- [x] High concurrency
## 🏛️ Architecture
- Microservices
- Vertical Slice Architecture
- CQRS
- Domain-Driven Design (DDD)
- Event Sourcing
## 🛠️ Tech Stack

**💻 Backend:** .NET 10, EF Core, MediatR, Carter, FluentValidation, AutoMapper, gRPC, RabbitMQ, MassTransit, YARP Reverse Proxy.

**🎨 Frontend:** React, TypeScript.

**🗄️ Database & Storage**: PostgreSQL, MongoDB, Redis.

**☁️ DevOps & Infrastructure:** Docker, Kubernetes, GitHub Actions.

**📊 Observability**: OpenTelemetry, Prometheus, Grafana, Jaeger.

**🧪 Testing:** xUnit, FluentAssertions.

## 🧩 ERD

## 🏗️ High-level Design

## License

[MIT](https://choosealicense.com/licenses/mit/)

