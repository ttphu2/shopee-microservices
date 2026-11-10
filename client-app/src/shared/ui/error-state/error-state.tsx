import type { ReactNode } from "react";
import styles from "./error-state.module.scss";

type Props = {
  title: string;
  description: string;
  /** Recovery actions, e.g. a retry button. */
  actions?: ReactNode;
  /** Support reference such as an error digest. */
  reference?: string | undefined;
};

export function ErrorState({ title, description, actions, reference }: Props) {
  return (
    <section role="alert" className={styles.errorState}>
      <h1 className={styles.title}>{title}</h1>
      <p className={styles.description}>{description}</p>
      {actions !== undefined && <div className={styles.actions}>{actions}</div>}
      {reference !== undefined && <p className={styles.reference}>Reference: {reference}</p>}
    </section>
  );
}
