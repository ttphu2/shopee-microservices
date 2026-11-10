import Form from "next/form";
import styles from "./search-form.module.scss";

// Submits as a GET (`?q=…`) with client-side navigation. It targets "/" until a
// search results route exists; typedRoutes will validate the new target then.
type Props = {
  className?: string | undefined;
};

export function SearchForm({ className }: Props) {
  return (
    <Form action="/" role="search" className={className}>
      <div className={styles.field}>
        <label htmlFor="site-search" className={styles.label}>
          Search products
        </label>
        <input
          id="site-search"
          name="q"
          type="search"
          placeholder="Search products"
          autoComplete="off"
          className={styles.input}
        />
        <button type="submit" className={styles.button} aria-label="Search">
          <svg
            aria-hidden="true"
            viewBox="0 0 24 24"
            width="20"
            height="20"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="round"
          >
            <circle cx="11" cy="11" r="7" />
            <path d="m20 20-3.5-3.5" />
          </svg>
        </button>
      </div>
    </Form>
  );
}
