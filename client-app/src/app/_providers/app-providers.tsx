"use client";

import { QueryClientProvider } from "@tanstack/react-query";
import type { ReactNode } from "react";
import { Toaster } from "@/shared/ui/toast";
import { GlobalLoading } from "./global-loading";
import { getQueryClient } from "./query-client";

type Props = {
  children: ReactNode;
};

/** Client-side app context plus the global overlays (loading, toasts). */
export function AppProviders({ children }: Props) {
  const queryClient = getQueryClient();

  return (
    <QueryClientProvider client={queryClient}>
      <GlobalLoading>{children}</GlobalLoading>
      <Toaster />
    </QueryClientProvider>
  );
}
