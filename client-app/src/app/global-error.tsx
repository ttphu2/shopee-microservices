"use client";

import { useEffect } from "react";
import { captureException } from "@/shared/observability/capture-exception";
import { Button } from "@/shared/ui/button";
import { ErrorState } from "@/shared/ui/error-state";
import styles from "./global-error.module.scss";
import "./globals.scss";

type Props = {
  error: Error & { digest?: string };
  retry: () => void;
};

// Last resort when the root layout itself fails. It replaces the whole document,
// so it brings its own <html>, <body> and global styles.
export default function GlobalError({ error, retry }: Props) {
  useEffect(() => {
    captureException(error, { boundary: "global", digest: error.digest });
  }, [error]);

  return (
    <html lang="en">
      <body>
        <title>Something went wrong</title>
        <main className={styles.page}>
          <ErrorState
            title="Something went wrong"
            description="The store couldn't be loaded. Please try again in a moment."
            reference={error.digest}
            actions={<Button onClick={() => retry()}>Try again</Button>}
          />
        </main>
      </body>
    </html>
  );
}
