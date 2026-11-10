"use client";

import { useIsFetching, useIsMutating } from "@tanstack/react-query";
import { usePathname, useSearchParams } from "next/navigation";
import { Suspense, useEffect, type ReactNode } from "react";
import { LoadingOverlay, useGlobalLoadingCount } from "@/shared/ui/loading-overlay";
import { endNavigation, useNavigationPending } from "./navigation-store";

type Props = {
  children: ReactNode;
};

/**
 * Blocks the app (its children) during every API call:
 * - route navigations waiting on Server Component data;
 * - every TanStack query fetch (first loads and refetches) and mutation;
 * - anything registered with useGlobalLoading / GlobalLoadingFallback
 *   (Server Actions outside TanStack, streamed <Suspense> data).
 */
export function GlobalLoading({ children }: Props) {
  const fetching = useIsFetching();
  const mutating = useIsMutating();
  const registered = useGlobalLoadingCount();
  const navigating = useNavigationPending();

  return (
    <>
      <LoadingOverlay active={navigating || fetching + mutating + registered > 0}>
        {children}
      </LoadingOverlay>
      {/* useSearchParams opts its subtree out of prerendering, so it gets its own boundary. */}
      <Suspense fallback={null}>
        <NavigationEndListener />
      </Suspense>
    </>
  );
}

function NavigationEndListener() {
  const pathname = usePathname();
  const searchParams = useSearchParams();

  useEffect(() => {
    endNavigation();
  }, [pathname, searchParams]);

  return null;
}
