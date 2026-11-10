"use client";

// Marked as a client module so Server Components can import this folder's public
// API (e.g. GlobalLoadingFallback) without pulling these hooks into the server graph.
import { useLayoutEffect } from "react";
import { useStore } from "zustand";
import { createStore } from "zustand/vanilla";

// Registry for pending work that TanStack Query and the router can't see (Server
// Actions via useActionState/useTransition, streamed <Suspense> data). The app's
// global loading overlay adds this count to its own sources. Only browser code
// registers, so the module-level store is never shared between server requests.
const loadingStore = createStore<{ count: number }>(() => ({ count: 0 }));

/** Shows the global loading overlay while `isLoading` is true (and the caller is mounted). */
export function useGlobalLoading(isLoading: boolean): void {
  // Layout effect, so blocking starts in the same commit as the pending state.
  useLayoutEffect(() => {
    if (!isLoading) return;
    loadingStore.setState(({ count }) => ({ count: count + 1 }));
    return () => loadingStore.setState(({ count }) => ({ count: count - 1 }));
  }, [isLoading]);
}

export function useGlobalLoadingCount(): number {
  return useStore(loadingStore, (state) => state.count);
}
