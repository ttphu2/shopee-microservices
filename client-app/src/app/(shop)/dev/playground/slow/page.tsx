import type { Metadata } from "next";
import Link from "next/link";
import { connection } from "next/server";
import { wait } from "../_lib/mock-api";
import styles from "../page.module.scss";

export const metadata: Metadata = {
  title: "Slow page · Dev playground",
  robots: { index: false, follow: false },
};

// Deliberately blocks navigation (no <Suspense>) to simulate an uncached page
// waiting on a slow BFF, which is when the blocking navigation overlay matters.
export const instant = false;

const SERVER_DELAY_MS = 2000;

export default async function SlowPlaygroundPage() {
  await connection();
  await wait(SERVER_DELAY_MS);

  return (
    <div className={styles.page}>
      <header className={styles.intro}>
        <h1 className={styles.title}>Slow page loaded</h1>
        <p className={styles.lead}>
          The server took {SERVER_DELAY_MS / 1000}s to render this page. During that time the
          previous page stayed on screen under the loading overlay.
        </p>
      </header>
      <Link href="/dev/playground">← Back to the playground</Link>
    </div>
  );
}
