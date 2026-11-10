import { ApiError } from "@/shared/api";

// Mock BFF for the playground. Responses use the backend's real wire formats and
// go through the real ApiError.from, so the playground exercises the same parsing,
// retry, toast and boundary rules as production calls.

type MockResponse = {
  status: number;
  body?: unknown;
  delayMs?: number;
};

export function wait(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

/** Resolves with the body for 2xx; otherwise throws the normalised ApiError. */
export async function mockApiCall<T>({ status, body, delayMs = 600 }: MockResponse): Promise<T> {
  await wait(delayMs);
  const response = new Response(body === undefined ? null : JSON.stringify(body), {
    status,
    headers: { "Content-Type": "application/json" },
  });
  if (response.ok) return (await response.json()) as T;
  throw ApiError.from(response, body);
}

export function mockTimeout(delayMs = 600): Promise<never> {
  return wait(delayMs).then(() => {
    throw new DOMException("The operation timed out.", "TimeoutError");
  });
}

// Bodies as written by ProductCatalog.API's ExceptionHandlingMiddleware.
export const problems = {
  serverError: {
    status: 500,
    type: "ServerError",
    title: "Server error",
    detail: "An unexpected error has occurred",
  },
  validationFailure: {
    status: 400,
    type: "ValidationFailure",
    title: "Validation error",
    detail: "One or more validation errors has occurred",
    errors: [
      { propertyName: "Name", errorMessage: "'Name' must not be empty." },
      { propertyName: "Skus[0].Price", errorMessage: "'Price' must be greater than '0'." },
    ],
  },
  // ASP.NET's automatic [ApiController] model validation (e.g. malformed JSON).
  modelValidation: {
    status: 400,
    type: "https://tools.ietf.org/html/rfc9110#section-15.5.1",
    title: "One or more validation errors occurred.",
    errors: { ProductCode: ["The ProductCode field is required."] },
  },
  notFound: {
    status: 404,
    type: "NotFound",
    title: "Not found",
    detail: "This product is no longer available.",
  },
  unavailable: {
    status: 503,
    type: "ServiceUnavailable",
    title: "Service unavailable",
    detail: "Catalog service is restarting",
  },
} as const;
