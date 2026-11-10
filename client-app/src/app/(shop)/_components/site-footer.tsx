import { siteConfig } from "@/shared/config/site";
import { Container } from "@/shared/ui/container";
import styles from "./site-footer.module.scss";

// No current-year copyright: `new Date()` during prerender fails the build
// under cacheComponents, and a static footer keeps the shell fully prerenderable.
export function SiteFooter() {
  return (
    <footer className={styles.footer}>
      <Container className={styles.inner}>
        <p>© {siteConfig.name}. All rights reserved.</p>
      </Container>
    </footer>
  );
}
