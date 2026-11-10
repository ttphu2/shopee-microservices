"use server";

import { wait } from "./mock-api";
import { isPlaygroundEnabled } from "./playground-flag";

const ACTION_DELAY_MS = 2000;

// Server Actions are reachable endpoints in any build that includes the page,
// so they enforce the playground gate themselves.
function assertPlaygroundEnabled(): void {
  if (!isPlaygroundEnabled) throw new Error("Not available");
}

export async function saveDraftAction(): Promise<{ savedAt: string }> {
  assertPlaygroundEnabled();
  await wait(ACTION_DELAY_MS);
  return { savedAt: new Date().toLocaleTimeString() };
}

export type NewsletterState = { message: string };

export async function subscribeAction(
  _previous: NewsletterState,
  formData: FormData,
): Promise<NewsletterState> {
  assertPlaygroundEnabled();
  await wait(ACTION_DELAY_MS);
  const email = formData.get("email");
  return { message: `Subscribed ${typeof email === "string" && email !== "" ? email : "(no email)"}` };
}
