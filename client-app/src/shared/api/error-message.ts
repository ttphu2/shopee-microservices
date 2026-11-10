import { isApiError } from "./api-error";

const GENERIC_MESSAGE = "Something went wrong. Please try again.";

/**
 * Text that is safe to show a shopper for any failed API call. Server (5xx) and
 * unknown errors stay generic so internal details never reach the UI.
 */
export function getUserFacingMessage(error: unknown): string {
  if (isApiError(error)) {
    if (error.isValidation) return "Some details need your attention. Please check and try again.";
    if (error.status >= 400 && error.status < 500) return error.message;
    return GENERIC_MESSAGE;
  }

  if (error instanceof DOMException && error.name === "TimeoutError") {
    return "The request took too long. Please try again.";
  }

  return GENERIC_MESSAGE;
}
