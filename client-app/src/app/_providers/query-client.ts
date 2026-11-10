import { isServer, MutationCache, QueryCache, QueryClient } from "@tanstack/react-query";
import { getUserFacingMessage, isApiError } from "@/shared/api";
import { captureException } from "@/shared/observability/capture-exception";
import { toast } from "@/shared/ui/toast";

const MAX_RETRIES = 2;

// Global error policy for client-side API calls, so features don't repeat it:
// - Queries: the component shows initial-load errors inline (or via `throwOnError`);
//   only a failed *background* refetch toasts, because stale data is still on screen.
// - Mutations: toast unless the caller opts out (`meta.errorToast: false`) or the
//   error carries field errors, which the form maps onto its inputs.
// - 4xx responses are never retried; they won't succeed on a second attempt.
function makeQueryClient(): QueryClient {
  return new QueryClient({
    queryCache: new QueryCache({
      onError: (error, query) => {
        captureException(error, { source: "query", queryKey: query.queryKey });
        if (query.state.data === undefined) return;
        notify(error, query.meta?.errorToast);
      },
    }),
    mutationCache: new MutationCache({
      onError: (error, _variables, _onMutateResult, mutation) => {
        // Validation failures are expected outcomes, not incidents: no report, no toast.
        if (isApiError(error) && error.isValidation) return;
        captureException(error, { source: "mutation", mutationKey: mutation.options.mutationKey });
        notify(error, mutation.meta?.errorToast);
      },
    }),
    defaultOptions: {
      queries: {
        // Above zero so data rendered on the server isn't refetched right after hydration.
        staleTime: 60_000,
        // Every fetch blocks the screen with the loading overlay, so refetching on tab
        // focus would freeze the page each time the shopper switches back. Refetches
        // happen on explicit refresh or invalidation (e.g. after a mutation) instead.
        refetchOnWindowFocus: false,
        retry: (failureCount, error) =>
          !(isApiError(error) && error.status < 500) && failureCount < MAX_RETRIES,
      },
    },
  });
}

function notify(error: unknown, errorToast: string | false | undefined): void {
  if (isServer || errorToast === false) return;
  toast.error(errorToast ?? "Request failed", getUserFacingMessage(error));
}

let browserQueryClient: QueryClient | undefined;

/** A fresh client per server render (no cross-request sharing), one per browser tab. */
export function getQueryClient(): QueryClient {
  if (isServer) return makeQueryClient();
  return (browserQueryClient ??= makeQueryClient());
}
