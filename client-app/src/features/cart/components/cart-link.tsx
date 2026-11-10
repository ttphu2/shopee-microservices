import Link from "next/link";
import styles from "./cart-link.module.scss";

// Presentational only: the item count is passed in. The Basket feature will own
// fetching it (e.g. a <Suspense>-wrapped wrapper that renders this component).
type Props = {
  itemCount: number;
};

export function CartLink({ itemCount }: Props) {
  return (
    // Points at "/" until a cart route exists.
    <Link href="/" className={styles.link} aria-label={`Cart, ${itemCount} items`}>
      <span className={styles.icon}>
        <svg
          aria-hidden="true"
          viewBox="0 0 24 24"
          width="28"
          height="28"
          fill="none"
          stroke="currentColor"
          strokeWidth="1.75"
          strokeLinecap="round"
          strokeLinejoin="round"
        >
          <path d="M3 4h2l2.4 11.2a1 1 0 0 0 1 .8h9.2a1 1 0 0 0 1-.8L20 8H6.2" />
          <circle cx="9.5" cy="19.5" r="1.25" />
          <circle cx="17" cy="19.5" r="1.25" />
        </svg>
        <span className={styles.count} aria-hidden="true">
          {itemCount}
        </span>
      </span>
      <span className={styles.label}>Cart</span>
    </Link>
  );
}
