import type { Metadata } from "next";
import Link from "next/link";
import { connection } from "next/server";
import { Suspense } from "react";
import { GlobalLoadingFallback } from "@/shared/ui/loading-overlay";
import { wait } from "../_lib/mock-api";
import styles from "../page.module.scss";

export const metadata: Metadata = {
  title: "Streaming page · Dev playground",
  robots: { index: false, follow: false },
};

const SERVER_DELAY_MS = 2000;

// Stands in for a per-user value (price, stock, cart count) streamed into a
// Partial Prerendering hole after the static shell has already rendered.
async function StreamedSection() {
  await connection();
  await wait(SERVER_DELAY_MS);
  return <p className={styles.lead}>Server data streamed in after {SERVER_DELAY_MS / 1000}s.</p>;
}

export default function StreamingPlaygroundPage() {
  return (
    <div className={styles.page}>
      <header className={styles.intro}>
        <h1 className={styles.title}>Streaming page</h1>
        <p className={styles.lead}>
          This static shell renders immediately; the section below waits on the server.
          The loading overlay stays up until it arrives.
        </p>
      </header>
      <Suspense
        fallback={
          <GlobalLoadingFallback>
            <p className={styles.lead}>Waiting for server data…</p>
          </GlobalLoadingFallback>
        }
      >
        <StreamedSection />
      </Suspense>
      <Link href="/dev/playground">← Back to the playground</Link>
    </div>
  );
}
