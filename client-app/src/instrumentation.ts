import type { Instrumentation } from "next";
import { captureException } from "@/shared/observability/capture-exception";

// Every server-side failure (render, Server Action, route handler, proxy) lands
// here, including the original error behind a digest shown by an error boundary.
export const onRequestError: Instrumentation.onRequestError = (error, request, context) => {
  captureException(error, {
    digest: typeof error === "object" && error !== null && "digest" in error ? error.digest : undefined,
    method: request.method,
    path: request.path,
    routePath: context.routePath,
    routeType: context.routeType,
  });
};
