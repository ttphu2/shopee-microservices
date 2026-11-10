import { startNavigation } from "@/app/_providers/navigation-store";
import { captureException } from "@/shared/observability/capture-exception";

// Runs before hydration. Keep it light: Next.js warns above 16ms.

// Errors outside React's tree (event handlers, timers, unhandled promises) never
// reach an error boundary, so report them here.
window.addEventListener("error", (event) => {
  captureException(event.error ?? event.message, { source: "window.error" });
});
window.addEventListener("unhandledrejection", (event) => {
  captureException(event.reason, { source: "unhandledrejection" });
});

export function onRouterTransitionStart(url: string): void {
  startNavigation(url);
}
