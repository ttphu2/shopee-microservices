import "@tanstack/react-query";
import type { ApiError } from "./api-error";

// Typed `meta` options that the app-level QueryClient reads to decide on error
// toasts. Features set them per query/mutation; defaults apply otherwise.
// (Every call shows the global loading overlay; there is deliberately no opt-out.)
type ApiCallMeta = {
  /** Toast title on failure, or `false` when the caller shows the error itself. */
  errorToast?: string | false;
};

declare module "@tanstack/react-query" {
  interface Register {
    defaultError: ApiError | Error;
    queryMeta: ApiCallMeta;
    mutationMeta: ApiCallMeta;
  }
}
