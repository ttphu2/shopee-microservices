// src/shared/api/index.ts — client-safe: types and helpers only
import type { components } from "./generated/schema";
export type Schemas = components["schemas"];
export { ApiError, isApiError, type FieldErrors } from "./api-error";
export { getUserFacingMessage } from "./error-message";
export { unwrap } from "./unwrap";
