// The single reporting seam for errors. Features and boundaries call this, never a
// vendor SDK, so adding Sentry (or similar) later changes only this file.
export type ErrorContext = Record<string, unknown>;

export function captureException(error: unknown, context?: ErrorContext): void {
  console.error("[captureException]", error, context ?? {});
}
