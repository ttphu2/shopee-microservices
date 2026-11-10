import type { FieldErrors } from "@/shared/api";
import styles from "./field-error-list.module.scss";

type Props = {
  fieldErrors: FieldErrors;
};

/** Stands in for a form mapping ApiError.fieldErrors onto its inputs. */
export function FieldErrorList({ fieldErrors }: Props) {
  return (
    <ul className={styles.list}>
      {Object.entries(fieldErrors).map(([field, messages]) => (
        <li key={field}>
          <code className={styles.field}>{field}</code>: {messages.join(" ")}
        </li>
      ))}
    </ul>
  );
}
