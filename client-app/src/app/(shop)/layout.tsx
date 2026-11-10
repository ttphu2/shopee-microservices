import { Container } from "@/shared/ui/container";
import { SiteFooter } from "./_components/site-footer";
import { SiteHeader } from "./_components/site-header";
import { SiteNav } from "./_components/site-nav";
import styles from "./layout.module.scss";

// Shop application shell. Every page in the (shop) route group renders inside
// <main>; the group adds no URL segment and maps to the future "shop" Multi-Zone.
export default function ShopLayout({ children }: LayoutProps<"/">) {
  return (
    <div className={styles.shell}>
      <a href="#main-content" className={styles.skipLink}>
        Skip to main content
      </a>
      <SiteHeader />
      <SiteNav />
      <main id="main-content" className={styles.main}>
        <Container>{children}</Container>
      </main>
      <SiteFooter />
    </div>
  );
}
