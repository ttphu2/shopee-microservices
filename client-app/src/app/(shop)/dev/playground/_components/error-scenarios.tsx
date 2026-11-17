"use client";

import { useMutation, useQuery, type MutationMeta } from "@tanstack/react-query";
import { getUserFacingMessage, isApiError } from "@/shared/api";
import { Button } from "@/shared/ui/button";
import { mockApiCall, mockTimeout, outcomes, problems, success } from "../_lib/mock-api";
import { ScenarioCard, ScenarioResult } from "./scenario-card";
import { ValidationMessageList } from "./validation-message-list";

type MutationScenarioProps = {
  title: string;
  expected: string;
  mutationFn: () => Promise<unknown>;
  meta?: MutationMeta | undefined;
};

function MutationScenario({ title, expected, mutationFn, meta = {} }: MutationScenarioProps) {
  const mutation = useMutation({ mutationFn, meta });
  const { error } = mutation;

  return (
    <ScenarioCard title={title} expected={expected}>
      <Button variant="secondary" disabled={mutation.isPending} onClick={() => mutation.mutate()}>
        Submit
      </Button>
      <ScenarioResult>
        {mutation.isPending && "Submitting…"}
        {error !== null && (isApiError(error) ? `ApiError ${error.status} · code "${error.code}"` : error.name)}
        {/* With errorToast: false the caller owns the message, as a real component would. */}
        {error !== null && meta.errorToast === false && ` · shown inline: ${getUserFacingMessage(error)}`}
        {mutation.isIdle && "Idle"}
      </ScenarioResult>
      {isApiError(error) && error.validationMessages !== undefined && meta.errorToast === false && (
        <ValidationMessageList messages={error.validationMessages} />
      )}
    </ScenarioCard>
  );
}

function InitialLoadQuery({ id, status, body, title, expected }: {
  id: string;
  status?: number;
  body: unknown;
  title: string;
  expected: string;
}) {
  const query = useQuery({
    queryKey: ["playground", "initial-load", id],
    queryFn: () => mockApiCall({ status, body }),
    enabled: false,
  });

  return (
    <ScenarioCard title={title} expected={expected}>
      <Button variant="secondary" onClick={() => void query.refetch()}>Load</Button>
      <ScenarioResult>
        {query.isFetching && `Attempt ${query.failureCount + 1}…`}
        {!query.isFetching && query.error !== null &&
          `Inline error after ${query.failureCount} attempt(s): ${getUserFacingMessage(query.error)}`}
        {!query.isFetching && query.error === null && "Idle"}
      </ScenarioResult>
    </ScenarioCard>
  );
}

// First call succeeds, every later call fails, so a refetch fails with data on screen.
let backgroundRefetchCalls = 0;

function BackgroundRefetchFailure() {
  const query = useQuery({
    queryKey: ["playground", "background-refetch"],
    queryFn: () =>
      backgroundRefetchCalls++ === 0
        ? mockApiCall<{ stock: number }>({ body: success({ stock: 7 }) })
        : mockApiCall<{ stock: number }>({ status: problems.serverError.status, body: problems.serverError }),
    enabled: false,
  });

  return (
    <ScenarioCard
      title="Background refetch fails"
      expected="Load succeeds; Refetch blocks through 2 retries (~4s), then one toast appears and the stale value stays visible."
    >
      <div>
        <Button variant="secondary" onClick={() => void query.refetch()}>
          {query.data === undefined ? "Load" : "Refetch"}
        </Button>
      </div>
      <ScenarioResult>
        {query.data !== undefined ? `Stock: ${query.data.stock}` : "No data yet"}
        {query.isFetching && ` · fetching (attempt ${query.failureCount + 1})…`}
        {!query.isFetching && query.isRefetchError && " · refetch failed, showing stale data"}
      </ScenarioResult>
    </ScenarioCard>
  );
}

export function ErrorScenarios() {
  return (
    <>
      <MutationScenario
        title="Mutation: server error (500)"
        expected="Error toast “Request failed” with a generic message; logged to the console."
        mutationFn={() => mockApiCall({ status: 500, body: problems.serverError })}
      />
      <MutationScenario
        title="Mutation: timeout"
        expected="Error toast saying the request took too long."
        mutationFn={() => mockTimeout()}
      />
      <MutationScenario
        title="Mutation: malformed request (ASP.NET 400)"
        expected="Error toast with the ProblemDetails title (4xx messages are safe to show); logged to the console."
        mutationFn={() => mockApiCall({ status: 400, body: problems.modelValidation })}
      />
      <MutationScenario
        title="Mutation: custom toast title"
        expected="Error toast titled “Couldn't add to cart” (meta.errorToast)."
        mutationFn={() => mockApiCall({ status: 500, body: problems.serverError })}
        meta={{ errorToast: "Couldn't add to cart" }}
      />
      <MutationScenario
        title="Mutation: errorToast false"
        expected="No toast; the card shows the error itself."
        mutationFn={() => mockApiCall({ status: 500, body: problems.serverError })}
        meta={{ errorToast: false }}
      />
      <MutationScenario
        title="Business error (200, isSuccessful false)"
        expected="Error toast showing the backend's validation messages; nothing logged."
        mutationFn={() => mockApiCall({ body: outcomes.validationFailure })}
      />
      <MutationScenario
        title="Business error shown by the form"
        expected="No toast and nothing logged; the messages are listed inline (errorToast: false)."
        mutationFn={() => mockApiCall({ body: outcomes.validationFailure })}
        meta={{ errorToast: false }}
      />
      <MutationScenario
        title="Optimistic-locking conflict"
        expected="Error toast asking to reload; nothing logged (it is a user message, not an incident)."
        mutationFn={() => mockApiCall({ body: outcomes.optimisticLocking })}
      />
      <InitialLoadQuery
        id="not-found"
        body={outcomes.notFound}
        title="Query: first load finds nothing"
        expected="200 with isSuccessful false: no toast, nothing logged, no retries; the card shows the message inline after 1 attempt."
      />
      <InitialLoadQuery
        id="unavailable"
        status={problems.unavailable.status}
        body={problems.unavailable}
        title="Query: first load fails with 503"
        expected="Overlay stays up through 2 retries (~4s), then the error shows inline after 3 attempts; no toast."
      />
      <BackgroundRefetchFailure />
    </>
  );
}
