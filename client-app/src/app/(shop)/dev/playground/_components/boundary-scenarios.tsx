"use client";

import Link from "next/link";
import { useState } from "react";
import { Button } from "@/shared/ui/button";
import { ScenarioCard } from "./scenario-card";

function ClientRenderError() {
  const [shouldThrow, setShouldThrow] = useState(false);
  if (shouldThrow) throw new Error("Playground: client render error");

  return (
    <ScenarioCard
      title="Client render error"
      expected="The shop error page replaces this page inside the shell; “Try again” brings the playground back."
    >
      <Button variant="secondary" onClick={() => setShouldThrow(true)}>Throw while rendering</Button>
    </ScenarioCard>
  );
}

function ServerComponentError() {
  return (
    <ScenarioCard
      title="Server Component error"
      expected="Shop error page with a Reference; the server log has the same digest. Use the browser's Back to return."
    >
      <Link href="/dev/playground?fail=server">Load this page with a failing Server Component →</Link>
    </ScenarioCard>
  );
}

function UnhandledRejection() {
  return (
    <ScenarioCard
      title="Unhandled promise rejection"
      expected="Nothing on screen; the console shows [captureException] with source “unhandledrejection”."
    >
      <Button
        variant="secondary"
        onClick={() => void Promise.reject(new Error("Playground: unhandled rejection"))}
      >
        Reject a promise
      </Button>
    </ScenarioCard>
  );
}

function EventHandlerError() {
  return (
    <ScenarioCard
      title="Error in an event handler"
      expected="Not caught by error boundaries; the console shows [captureException] with source “window.error”."
    >
      <Button
        variant="secondary"
        onClick={() => {
          throw new Error("Playground: event handler error");
        }}
      >
        Throw in onClick
      </Button>
    </ScenarioCard>
  );
}

export function BoundaryScenarios() {
  return (
    <>
      <ClientRenderError />
      <ServerComponentError />
      <UnhandledRejection />
      <EventHandlerError />
    </>
  );
}
