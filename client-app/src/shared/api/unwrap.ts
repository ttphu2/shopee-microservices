import { ApiError, type ValidationOutcome } from "./api-error";

export function unwrap<T>(result: {
  data?: ValidationOutcome<T>;
  error?: unknown;
  response: Response;
}): T {
  const { data, response } = result;
  if (data === undefined) throw ApiError.from(response, result.error);
  if (data.isSuccessful !== true) throw ApiError.fromOutcome(response, data);
  return data.value as T;
}
