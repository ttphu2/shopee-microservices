# search

**Purpose:** product search: the header search box today; results, facets and suggestions later.

**Owner:** _TBD_

## Public API (`index.ts`)

- `SearchForm`: header search box. A GET form that puts the query in `?q=`.

## Key flows

- Submitting navigates client-side with the query in the URL. It targets `/` until the
  `(shop)/search` route exists; then change the form `action` (checked by `typedRoutes`).
