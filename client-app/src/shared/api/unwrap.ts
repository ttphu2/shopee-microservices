import { ApiError } from "./api-error";

export function unwrap<T>(result: {
  data?: T;
  error?: unknown;
  response: Response;
}): T {
  if (result.data !== undefined) return result.data;
  throw ApiError.from(result.response, result.error);
}
