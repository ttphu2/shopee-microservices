import { createStore } from "zustand/vanilla";

export type ToastVariant = "error" | "info";

export type Toast = {
  id: number;
  variant: ToastVariant;
  title: string;
  description?: string | undefined;
};

type ToastState = { toasts: readonly Toast[] };

const MAX_VISIBLE_TOASTS = 3;
const INFO_TIMEOUT_MS = 5_000;

// A module-level queue (like React Aria's ToastQueue): any code can raise a toast
// imperatively and the single <Toaster /> renders it. Toasts are only raised from
// browser events, so the queue is never shared between requests on the server.
export const toastStore = createStore<ToastState>(() => ({ toasts: [] }));

let nextId = 1;

function show(variant: ToastVariant, title: string, description?: string): void {
  const { toasts } = toastStore.getState();
  // Repeated failures (e.g. a retried background refetch) shouldn't stack copies.
  if (toasts.some((t) => t.variant === variant && t.title === title)) return;

  const toast: Toast = { id: nextId++, variant, title, description };
  toastStore.setState({ toasts: [...toasts, toast].slice(-MAX_VISIBLE_TOASTS) });

  // Errors stay until dismissed so they can't vanish before they're read.
  if (variant === "info") setTimeout(() => dismissToast(toast.id), INFO_TIMEOUT_MS);
}

export function dismissToast(id: number): void {
  toastStore.setState(({ toasts }) => ({ toasts: toasts.filter((t) => t.id !== id) }));
}

export const toast = {
  error: (title: string, description?: string) => show("error", title, description),
  info: (title: string, description?: string) => show("info", title, description),
};
