import type { Metadata } from "next";
import { Suspense } from "react";
import { ApiError } from "@/shared/api";
import { BoundaryScenarios } from "./_components/boundary-scenarios";
import { ErrorScenarios } from "./_components/error-scenarios";
import { LoadingScenarios } from "./_components/loading-scenarios";
import { problems } from "./_lib/mock-api";
import styles from "./page.module.scss";

export const metadata: Metadata = {
  title: "Dev playground",
  robots: { index: false, follow: false },
};

// Throws like a Server Component whose BFF call failed, when the URL asks for it.
async function ServerFailureProbe({ searchParams }: Pick<PageProps<"/dev/playground">, "searchParams">) {
  const { fail } = await searchParams;
  if (fail === "server") {
    throw ApiError.from(new Response(null, { status: 503 }), problems.unavailable);
  }
  return null;
}

export default function PlaygroundPage({ searchParams }: PageProps<"/dev/playground">) {
  return (
    <div className={styles.page}>
      <Suspense fallback={null}>
        <ServerFailureProbe searchParams={searchParams} />
      </Suspense>

      <header className={styles.intro}>
        <h1 className={styles.title}>Dev playground</h1>
        <p className={styles.lead}>
          Mocked API calls for checking global loading and error handling by hand. Responses
          use the backend&apos;s real error formats; nothing here calls the real API. Open the
          browser console to see what gets reported.
        </p>
      </header>

      <section aria-labelledby="loading-heading" className={styles.section}>
        <h2 id="loading-heading" className={styles.sectionTitle}>Global loading</h2>
        <div className={styles.grid}>
          <LoadingScenarios />
        </div>
      </section>

      <section aria-labelledby="errors-heading" className={styles.section}>
        <h2 id="errors-heading" className={styles.sectionTitle}>API errors</h2>
        <div className={styles.grid}>
          <ErrorScenarios />
        </div>
      </section>

      <section aria-labelledby="boundaries-heading" className={styles.section}>
        <h2 id="boundaries-heading" className={styles.sectionTitle}>Error boundaries &amp; reporting</h2>
        <div className={styles.grid}>
          <BoundaryScenarios />
        </div>
      </section>
    </div>
  );
}
