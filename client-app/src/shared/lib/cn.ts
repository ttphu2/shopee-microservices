import { clsx, type ClassValue } from "clsx";

/** Joins conditional class names (e.g. a CSS module class plus a caller's className). */
export function cn(...inputs: ClassValue[]): string {
  return clsx(inputs);
}
