// src/shared/api/api-error.ts
/** Field path (as sent by the backend) → messages, e.g. { "skus[0].price": ["Must be positive"] }. */
export type FieldErrors = Record<string, string[]>;

/**
 * The one normalised error for failed BFF responses, so features never inspect
 * raw responses or ProblemDetails bodies.
 */
export class ApiError extends Error {
  constructor(
    readonly status: number,
    readonly code: string,
    message: string,
    readonly fieldErrors?: FieldErrors | undefined,
  ) {
    super(message);
    this.name = "ApiError";
  }

  /** A 4xx with field errors: the caller (usually a form) is expected to show them inline. */
  get isValidation(): boolean {
    return this.fieldErrors !== undefined;
  }

  static from(response: Response, body: unknown): ApiError {
    const problem = isRecord(body) ? body : {};
    const code = readString(problem, "type") ?? "unknown";
    const message =
      readString(problem, "detail") ??
      readString(problem, "title") ??
      `Request failed with status ${response.status}`;

    return new ApiError(response.status, code, message, toFieldErrors(problem["errors"]));
  }
}

export function isApiError(error: unknown): error is ApiError {
  return error instanceof ApiError;
}

// The backend sends `errors` in two shapes: its ExceptionHandlingMiddleware writes
// [{ propertyName, errorMessage }], while ASP.NET's automatic model validation
// ([ApiController]) writes { Field: ["message"] }.
function toFieldErrors(errors: unknown): FieldErrors | undefined {
  const fieldErrors: FieldErrors = {};

  if (Array.isArray(errors)) {
    for (const entry of errors) {
      if (!isRecord(entry)) continue;
      const field = readString(entry, "propertyName") ?? "";
      const message = readString(entry, "errorMessage");
      if (message !== undefined) (fieldErrors[field] ??= []).push(message);
    }
  } else if (isRecord(errors)) {
    for (const [field, messages] of Object.entries(errors)) {
      if (Array.isArray(messages)) {
        fieldErrors[field] = messages.filter((m): m is string => typeof m === "string");
      }
    }
  }

  return Object.keys(fieldErrors).length > 0 ? fieldErrors : undefined;
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === "object" && value !== null && !Array.isArray(value);
}

function readString(record: Record<string, unknown>, key: string): string | undefined {
  const value = record[key];
  return typeof value === "string" && value.length > 0 ? value : undefined;
}
