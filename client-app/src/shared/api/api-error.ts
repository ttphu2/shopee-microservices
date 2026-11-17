// src/shared/api/api-error.ts

export type ValidationOutcome<T = unknown> = {
  readonly isSuccessful?: boolean;
  readonly validationMessages?: { readonly messages?: readonly string[] | null } | null;
  readonly value?: T | null;
};

/**
 * The one normalised error for failed BFF calls, so features never inspect raw
 * responses, outcomes or ProblemDetails bodies.
 */
export class ApiError extends Error {
  constructor(
    readonly status: number,
    readonly code: string,
    message: string,
    readonly validationMessages?: readonly string[] | undefined,
  ) {
    super(message);
    this.name = "ApiError";
  }

  /** A business error: an expected outcome whose messages are meant for the shopper. */
  get isValidation(): boolean {
    return this.validationMessages !== undefined;
  }

  static from(response: Response, body: unknown): ApiError {
    const problem = isRecord(body) ? body : {};
    const code = readString(problem, "type") ?? "unknown";
    const message =
      readString(problem, "detail") ??
      readString(problem, "title") ??
      `Request failed with status ${response.status}`;

    return new ApiError(response.status, code, message);
  }

  static fromOutcome(response: Response, outcome: ValidationOutcome): ApiError {
    const messages = (outcome.validationMessages?.messages ?? []).filter(
      (message) => typeof message === "string" && message.length > 0,
    );
    if (messages.length === 0) messages.push("The request could not be completed.");

    return new ApiError(response.status, "ValidationFailure", messages.join(" "), messages);
  }
}

export function isApiError(error: unknown): error is ApiError {
  return error instanceof ApiError;
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === "object" && value !== null && !Array.isArray(value);
}

function readString(record: Record<string, unknown>, key: string): string | undefined {
  const value = record[key];
  return typeof value === "string" && value.length > 0 ? value : undefined;
}
