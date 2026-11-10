"use client";

import { useStore } from "zustand";
import { cn } from "@/shared/lib/cn";
import { dismissToast, toastStore } from "./toast-queue";
import styles from "./toaster.module.scss";

/** Renders the toast queue. Mount once, at the root of the app. */
export function Toaster() {
  const toasts = useStore(toastStore, (state) => state.toasts);

  return (
    <section aria-label="Notifications" className={styles.region}>
      <ol className={styles.list}>
        {toasts.map((toast) => (
          <li
            key={toast.id}
            role={toast.variant === "error" ? "alert" : "status"}
            className={cn(styles.toast, styles[toast.variant])}
          >
            <div className={styles.content}>
              <p className={styles.title}>{toast.title}</p>
              {toast.description !== undefined && (
                <p className={styles.description}>{toast.description}</p>
              )}
            </div>
            <button
              type="button"
              aria-label="Dismiss notification"
              className={styles.dismiss}
              onClick={() => dismissToast(toast.id)}
            >
              <svg aria-hidden="true" viewBox="0 0 24 24" width="16" height="16" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round">
                <path d="M6 6l12 12M18 6 6 18" />
              </svg>
            </button>
          </li>
        ))}
      </ol>
    </section>
  );
}
