import { unwrap, type ValidationOutcome } from "@/shared/api";

// Mock BFF for the playground. Responses use the backend's real wire formats and
// go through the real unwrap, so the playground exercises the same parsing, retry,
// toast and boundary rules as production calls.

type MockResponse = {
  status?: number | undefined;
  body?: unknown;
  delayMs?: number;
};

export function wait(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

/** Resolves with the outcome's value; otherwise throws the normalised ApiError. */
export async function mockApiCall<T>({ status = 200, body, delayMs = 600 }: MockResponse): Promise<T> {
  await wait(delayMs);
  const response = new Response(body === undefined ? null : JSON.stringify(body), {
    status,
    headers: { "Content-Type": "application/json" },
  });
  if (response.ok) return unwrap<T>({ data: (await response.json()) as ValidationOutcome<T>, response });
  return unwrap<T>({ error: body, response });
}

export function mockTimeout(delayMs = 600): Promise<never> {
  return wait(delayMs).then(() => {
    throw new DOMException("The operation timed out.", "TimeoutError");
  });
}

export function success<T>(value: T): ValidationOutcome<T> {
  return { value, isSuccessful: true, validationMessages: null };
}

export function failure(...messages: string[]): ValidationOutcome<never> {
  return { value: null, isSuccessful: false, validationMessages: { messages } };
}

export const outcomes = {
  validationFailure: failure("'Name' must not be empty.", "'Price' must be greater than '0'."),
  notFound: failure("The requested resource was not found."),
  optimisticLocking: failure(
    "This data was changed by someone else in the meantime. Reload it and try again.",
  ),
} as const;

// Non-2xx bodies: GlobalExceptionHandler's 500, ASP.NET's model-binding 400, and a
// gateway-style 503.
export const problems = {
  serverError: {
    status: 500,
    type: "ServerError",
    title: "Server error",
    detail: "An unexpected error has occurred",
  },
  // ASP.NET's automatic [ApiController] model validation (e.g. malformed JSON).
  modelValidation: {
    status: 400,
    type: "https://tools.ietf.org/html/rfc9110#section-15.5.1",
    title: "One or more validation errors occurred.",
    errors: { ProductCode: ["The ProductCode field is required."] },
  },
  unavailable: {
    status: 503,
    type: "ServiceUnavailable",
    title: "Service unavailable",
    detail: "Catalog service is restarting",
  },
} as const;
