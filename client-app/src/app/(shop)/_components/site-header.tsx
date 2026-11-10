import Link from "next/link";
import { CartLink } from "@/features/cart";
import { SearchForm } from "@/features/search";
import { siteConfig } from "@/shared/config/site";
import { Container } from "@/shared/ui/container";
import styles from "./site-header.module.scss";

// Placeholder until the cart feature exposes a per-user count (a <Suspense> hole).
const PLACEHOLDER_CART_ITEM_COUNT = 0;

export function SiteHeader() {
  return (
    <header className={styles.header}>
      <Container className={styles.bar}>
        <Link href="/" className={styles.brand}>
          {siteConfig.name}
        </Link>

        <SearchForm className={styles.search} />

        <div className={styles.actions}>
          {/* Points at "/" until account/sign-in routes exist. */}
          <Link href="/" className={styles.account}>
            <span className={styles.accountGreeting}>Hello, sign in</span>
            <span className={styles.accountLabel}>Account</span>
          </Link>
          <CartLink itemCount={PLACEHOLDER_CART_ITEM_COUNT} />
        </div>
      </Container>
    </header>
  );
}
