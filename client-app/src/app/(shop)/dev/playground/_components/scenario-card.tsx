import type { ReactNode } from "react";
import styles from "./scenario-card.module.scss";

type Props = {
  title: string;
  /** What the tester should observe; makes each card a checklist item. */
  expected: string;
  children: ReactNode;
};

export function ScenarioCard({ title, expected, children }: Props) {
  return (
    <article className={styles.card}>
      <h3 className={styles.title}>{title}</h3>
      <p className={styles.expected}>
        <span className={styles.expectedLabel}>Expect:</span> {expected}
      </p>
      <div className={styles.controls}>{children}</div>
    </article>
  );
}

type ResultProps = {
  children: ReactNode;
};

/** Inline outcome of a scenario, announced politely to screen readers. */
export function ScenarioResult({ children }: ResultProps) {
  return (
    <p aria-live="polite" className={styles.result}>
      {children}
    </p>
  );
}
