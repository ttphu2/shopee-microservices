"use client";

import { useMutation, useQueries, useQuery } from "@tanstack/react-query";
import Link from "next/link";
import { useActionState, useState } from "react";
import { Button } from "@/shared/ui/button";
import { useGlobalLoading } from "@/shared/ui/loading-overlay";
import { saveDraftAction, subscribeAction, type NewsletterState } from "../_lib/actions";
import { mockApiCall } from "../_lib/mock-api";
import { ScenarioCard, ScenarioResult } from "./scenario-card";

// Each run uses a new query key, so every click is a fresh first load.
function useRunCounter() {
  const [run, setRun] = useState(0);
  return { run, next: () => setRun((r) => r + 1) };
}

function SlowQuery() {
  const { run, next } = useRunCounter();
  const query = useQuery({
    queryKey: ["playground", "slow-query", run],
    queryFn: () => mockApiCall<{ items: number }>({ status: 200, body: { items: 24 }, delayMs: 2000 }),
    enabled: run > 0,
  });

  return (
    <ScenarioCard
      title="Slow first load (2s)"
      expected="The screen dims with a centred spinner after ~150ms; nothing is clickable until the data arrives."
    >
      <Button variant="secondary" onClick={next}>Load</Button>
      <ScenarioResult>
        {query.isFetching ? "Fetching…" : query.data ? `Loaded ${query.data.items} items` : "Idle"}
      </ScenarioResult>
    </ScenarioCard>
  );
}

function FastQuery() {
  const { run, next } = useRunCounter();
  const query = useQuery({
    queryKey: ["playground", "fast-query", run],
    queryFn: () => mockApiCall<{ ok: true }>({ status: 200, body: { ok: true }, delayMs: 80 }),
    enabled: run > 0,
  });

  return (
    <ScenarioCard
      title="Fast first load (80ms)"
      expected="No visible overlay: calls under 150ms are blocked but never flash the spinner."
    >
      <Button variant="secondary" onClick={next}>Load</Button>
      <ScenarioResult>{query.isFetching ? "Fetching…" : query.data ? `Loaded (run ${run})` : "Idle"}</ScenarioResult>
    </ScenarioCard>
  );
}

function BackgroundRefetch() {
  const query = useQuery({
    queryKey: ["playground", "background-refetch-ok"],
    queryFn: () => mockApiCall<{ at: string }>({
      status: 200,
      body: { at: new Date().toLocaleTimeString() },
      delayMs: 2000,
    }),
    enabled: false,
  });

  return (
    <ScenarioCard
      title="Refresh existing data (2s)"
      expected="Both Load and Refresh block, even though data is already on screen."
    >
      <Button variant="secondary" onClick={() => void query.refetch()}>
        {query.data === undefined ? "Load" : "Refresh"}
      </Button>
      <ScenarioResult>
        {query.data !== undefined ? `Data from ${query.data.at}` : "No data yet"}
        {query.isFetching && " · fetching…"}
      </ScenarioResult>
    </ScenarioCard>
  );
}

const PARALLEL_DELAYS_MS = [500, 1500, 3000] as const;

function ParallelQueries() {
  const { run, next } = useRunCounter();
  const queries = useQueries({
    queries: PARALLEL_DELAYS_MS.map((delayMs) => ({
      queryKey: ["playground", "parallel", delayMs, run],
      queryFn: () => mockApiCall<number>({ status: 200, body: delayMs, delayMs }),
      enabled: run > 0,
    })),
  });
  const done = queries.filter((q) => q.data !== undefined && !q.isFetching).length;

  return (
    <ScenarioCard
      title="Parallel loads (0.5s, 1.5s, 3s)"
      expected="One overlay until the slowest call finishes; it doesn't flicker between calls."
    >
      <Button variant="secondary" onClick={next}>Load all three</Button>
      <ScenarioResult>
        {queries.some((q) => q.isFetching) ? `${done}/3 done…` : done === 3 ? "All 3 loaded" : "Idle"}
      </ScenarioResult>
    </ScenarioCard>
  );
}

function SlowMutation() {
  const mutation = useMutation({
    mutationFn: () => mockApiCall<{ id: string }>({ status: 200, body: { id: "p-123" }, delayMs: 2000 }),
  });

  return (
    <ScenarioCard
      title="Slow mutation (2s)"
      expected="The overlay blocks while saving, so the shopper can't submit twice."
    >
      <Button variant="secondary" onClick={() => mutation.mutate()}>Save</Button>
      <ScenarioResult>
        {mutation.isPending ? "Saving…" : mutation.isSuccess ? `Saved ${mutation.data.id}` : "Idle"}
      </ScenarioResult>
    </ScenarioCard>
  );
}

function ServerActionViaMutation() {
  const mutation = useMutation({ mutationFn: () => saveDraftAction() });

  return (
    <ScenarioCard
      title="Server Action via useMutation (2s)"
      expected="Blocks automatically: Server Actions called through useMutation are tracked like any mutation."
    >
      <Button variant="secondary" onClick={() => mutation.mutate()}>Save draft</Button>
      <ScenarioResult>
        {mutation.isPending ? "Saving…" : mutation.isSuccess ? `Saved at ${mutation.data.savedAt}` : "Idle"}
      </ScenarioResult>
    </ScenarioCard>
  );
}

const INITIAL_NEWSLETTER_STATE: NewsletterState = { message: "Idle" };

function ServerActionForm() {
  const [state, formAction, isPending] = useActionState(subscribeAction, INITIAL_NEWSLETTER_STATE);
  // Form actions bypass TanStack Query, so the pending state is registered explicitly.
  useGlobalLoading(isPending);

  return (
    <ScenarioCard
      title="Server Action form (useActionState, 2s)"
      expected="Blocks while submitting, via useGlobalLoading(isPending)."
    >
      <form action={formAction}>
        <input name="email" type="email" defaultValue="shopper@example.com" aria-label="Email" />{" "}
        <Button variant="secondary" type="submit">Subscribe</Button>
      </form>
      <ScenarioResult>{isPending ? "Submitting…" : state.message}</ScenarioResult>
    </ScenarioCard>
  );
}

function ClickCounter() {
  const [clicks, setClicks] = useState(0);

  return (
    <ScenarioCard
      title="Blocking check"
      expected="Start any slow scenario, then try clicking here or pressing Tab/Enter/Escape: the count must not change until it finishes."
    >
      <Button variant="secondary" onClick={() => setClicks((c) => c + 1)}>Click me</Button>
      <ScenarioResult>Clicks: {clicks}</ScenarioResult>
    </ScenarioCard>
  );
}

function SlowNavigation() {
  return (
    <ScenarioCard
      title="Slow server navigation (2s)"
      expected="This page stays visible under the overlay until the next page's server data is ready."
    >
      {/* No prefetch, so the navigation must wait on the server like an uncached page. */}
      <Link href="/dev/playground/slow" prefetch={false}>
        Go to a page that takes 2s on the server →
      </Link>
    </ScenarioCard>
  );
}

function StreamingNavigation() {
  return (
    <ScenarioCard
      title="Streamed server data (2s)"
      expected="The new page's shell appears at once, but stays blocked until its streamed section arrives."
    >
      <Link href="/dev/playground/streaming" prefetch={false}>
        Go to a page with a streamed section →
      </Link>
    </ScenarioCard>
  );
}

export function LoadingScenarios() {
  return (
    <>
      <SlowQuery />
      <FastQuery />
      <BackgroundRefetch />
      <ParallelQueries />
      <SlowMutation />
      <ServerActionViaMutation />
      <ServerActionForm />
      <ClickCounter />
      <SlowNavigation />
      <StreamingNavigation />
    </>
  );
}
