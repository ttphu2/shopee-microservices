"use client";

import type { ReactNode } from "react";
import { useGlobalLoading } from "./global-loading-store";

type Props = {
  /** Optional skeleton, rendered under the overlay while the data streams in. */
  children?: ReactNode;
};

/**
 * Suspense fallback for server data: keeps the global loading overlay up while
 * the boundary waits, e.g. `<Suspense fallback={<GlobalLoadingFallback />}>`.
 */
export function GlobalLoadingFallback({ children }: Props) {
  useGlobalLoading(true);
  return children;
}
