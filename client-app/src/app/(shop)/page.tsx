import { ProductPlaceholderCard } from "./_components/product-placeholder-card";
import styles from "./page.module.scss";

// Static sample data that only demonstrates the layout. Real products will come
// from a catalog loader (features/catalog/server) as entities/product models.
const SAMPLE_PRODUCTS = [
  { id: "sample-1", name: "Wireless Noise-Cancelling Headphones" },
  { id: "sample-2", name: "Stainless Steel Water Bottle, 1L" },
  { id: "sample-3", name: "Ergonomic Office Chair" },
  { id: "sample-4", name: "Hardcover Notebook, Dotted" },
] as const;

export default function HomePage() {
  return (
    <div className={styles.page}>
      <section className={styles.intro}>
        <h1 className={styles.title}>Welcome to the store</h1>
        <p className={styles.lead}>
          Browse categories, search for products, and discover today&apos;s
          picks. This page is a placeholder for the storefront home.
        </p>
      </section>

      <section aria-labelledby="featured-heading">
        <h2 id="featured-heading" className={styles.sectionTitle}>
          Featured picks
        </h2>
        <ul className={styles.grid}>
          {SAMPLE_PRODUCTS.map((product) => (
            <li key={product.id}>
              <ProductPlaceholderCard name={product.name} />
            </li>
          ))}
        </ul>
      </section>
    </div>
  );
}
