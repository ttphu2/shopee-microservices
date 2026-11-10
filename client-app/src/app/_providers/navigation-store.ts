import { useStore } from "zustand";
import { createStore } from "zustand/vanilla";

// App Router has a navigation *start* hook (onRouterTransitionStart in
// src/instrumentation-client.ts) but no end hook, so the end is detected when the
// committed URL changes (see NavigationEndListener). The timeout keeps the
// blocking overlay from sticking if a navigation is aborted or lands on the same URL.
const SAFETY_TIMEOUT_MS = 10_000;

const navigationStore = createStore<{ pending: boolean }>(() => ({ pending: false }));

let safetyTimer: ReturnType<typeof setTimeout> | undefined;

export function startNavigation(url: string): void {
  const target = new URL(url, window.location.href);
  // Hash-only or same-URL navigations never change the committed URL, so they'd never end.
  if (target.pathname === window.location.pathname && target.search === window.location.search) {
    return;
  }

  navigationStore.setState({ pending: true });
  clearTimeout(safetyTimer);
  safetyTimer = setTimeout(endNavigation, SAFETY_TIMEOUT_MS);
}

export function endNavigation(): void {
  clearTimeout(safetyTimer);
  navigationStore.setState({ pending: false });
}

export function useNavigationPending(): boolean {
  return useStore(navigationStore, (state) => state.pending);
}
