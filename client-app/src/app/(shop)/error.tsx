"use client";

import { useEffect } from "react";
import { captureException } from "@/shared/observability/capture-exception";
import { Button } from "@/shared/ui/button";
import { ErrorState } from "@/shared/ui/error-state";

type Props = {
  error: Error & { digest?: string };
  retry: () => void;
};

// Unexpected failures (BFF down, timeouts, bugs) in any shop page. Rendered inside
// the shop layout, so the header and navigation stay usable. In production,
// Server Component errors arrive without their message; the digest links the
// shopper's report to the server log written by onRequestError.
export default function ShopError({ error, retry }: Props) {
  useEffect(() => {
    captureException(error, { boundary: "shop", digest: error.digest });
  }, [error]);

  return (
    <ErrorState
      title="Something went wrong"
      description="We couldn't load this page. Please try again in a moment."
      reference={error.digest}
      actions={<Button onClick={() => retry()}>Try again</Button>}
    />
  );
}
