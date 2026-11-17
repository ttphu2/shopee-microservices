import styles from "./validation-message-list.module.scss";

type Props = {
  messages: readonly string[];
};

/** Stands in for a form listing ApiError.validationMessages above its inputs. */
export function ValidationMessageList({ messages }: Props) {
  return (
    <ul className={styles.list}>
      {messages.map((message) => (
        <li key={message}>{message}</li>
      ))}
    </ul>
  );
}
