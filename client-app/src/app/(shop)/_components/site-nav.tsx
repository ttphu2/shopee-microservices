import type { Route } from "next";
import Link from "next/link";
import { Container } from "@/shared/ui/container";
import styles from "./site-nav.module.scss";

type NavItem = {
  label: string;
  href: Route;
};

// Category hrefs target "/" until a product listing route exists; switching them
// to e.g. `/search?category=…` later is checked by typedRoutes.
const NAV_ITEMS: readonly NavItem[] = [
  { label: "Today's Deals", href: "/?category=deals" },
  { label: "Electronics", href: "/?category=electronics" },
  { label: "Fashion", href: "/?category=fashion" },
  { label: "Home & Kitchen", href: "/?category=home-kitchen" },
  { label: "Books", href: "/?category=books" },
  { label: "Sports & Outdoors", href: "/?category=sports-outdoors" },
  { label: "Toys & Games", href: "/?category=toys-games" },
];

export function SiteNav() {
  return (
    <nav aria-label="Shop by category" className={styles.nav}>
      <Container>
        <ul className={styles.list}>
          {NAV_ITEMS.map((item) => (
            <li key={item.label}>
              <Link href={item.href} className={styles.link}>
                {item.label}
              </Link>
            </li>
          ))}
        </ul>
      </Container>
    </nav>
  );
}
