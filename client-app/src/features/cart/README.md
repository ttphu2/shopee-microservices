# cart

**Purpose:** the shopper's cart: header entry point today; mini-cart, add-to-cart and the cart page later.

**Owner:** _TBD_

## Public API (`index.ts`)

- `CartLink`: header cart entry. Presentational; it renders the `itemCount` it is given.

## Key flows

- The count is a static placeholder passed in by the shop layout. Once the cart has data, the
  count becomes its own async Server Component in a `<Suspense>` hole (it is per-user), loaded
  through `server.ts`, so the rest of the header stays in the prerendered shell.
- Links to `/` until the `(checkout)/cart` route exists.
