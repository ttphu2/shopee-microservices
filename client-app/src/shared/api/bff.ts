import "server-only";
import createClient from "openapi-fetch";
import type { paths } from "./generated/schema";
import { env } from "@/shared/config/env";

// Fail fast instead of holding a render open when the BFF hangs; the TimeoutError
// reaches the nearest error boundary like any other unexpected failure.
const BFF_TIMEOUT_MS = 10_000;

export const bff = createClient<paths>({
  baseUrl: env.BFF_URL,
  fetch: (request) =>
    fetch(request, {
      signal: AbortSignal.any([request.signal, AbortSignal.timeout(BFF_TIMEOUT_MS)]),
    }),
});
