"use client";

import { useLayoutEffect, useRef, type ReactNode } from "react";
import styles from "./loading-overlay.module.scss";

type Props = {
  active: boolean;
  /** The content to block while active. */
  children: ReactNode;
  label?: string | undefined;
};

/**
 * Blocks its children while `active`: they become `inert` (no clicks, focus or
 * keyboard input), and a full-screen scrim with a centred spinner covers them.
 * Blocking starts in the same commit; the scrim and spinner fade in after a short
 * CSS delay, so quick calls are protected from double submits without flashing.
 *
 * Not a modal <dialog>: browsers deliberately let Escape close those, and a
 * loading state must not be dismissible while the call is still running.
 */
export function LoadingOverlay({ active, children, label = "Loading" }: Props) {
  const overlayRef = useRef<HTMLDivElement>(null);
  const lastFocusedRef = useRef<HTMLElement | null>(null);

  // Before paint: move focus onto the overlay (the content is inert, so its focus
  // would otherwise drop to <body>), then hand it back when loading ends — unless
  // the user deliberately moved focus elsewhere meanwhile (e.g. to a toast).
  useLayoutEffect(() => {
    if (active) {
      overlayRef.current?.focus({ preventScroll: true });
      return;
    }
    const focusIsUnclaimed = document.activeElement === null || document.activeElement === document.body;
    const lastFocused = lastFocusedRef.current;
    if (focusIsUnclaimed && lastFocused?.isConnected) lastFocused.focus({ preventScroll: true });
  }, [active]);

  return (
    <>
      <div
        inert={active}
        className={styles.content}
        onFocus={(event) => {
          if (event.target instanceof HTMLElement) lastFocusedRef.current = event.target;
        }}
      >
        {children}
      </div>
      {active && (
        <div
          ref={overlayRef}
          role="progressbar"
          aria-label={label}
          tabIndex={-1}
          className={styles.overlay}
        >
          <div className={styles.spinner} />
        </div>
      )}
    </>
  );
}
