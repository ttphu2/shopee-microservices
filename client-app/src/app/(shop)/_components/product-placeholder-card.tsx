import styles from "./product-placeholder-card.module.scss";

type Props = {
  name: string;
};

// Sample-page only. The real ProductCard belongs in entities/product, and prices
// come from entities/money (Money + formatMoney) once product data exists, so
// this placeholder shows a skeleton instead of formatting a number itself.
export function ProductPlaceholderCard({ name }: Props) {
  return (
    <article className={styles.card}>
      <div className={styles.image} aria-hidden="true" />
      <div className={styles.body}>
        <h3 className={styles.name}>{name}</h3>
        <div className={styles.priceSkeleton} aria-hidden="true" />
      </div>
    </article>
  );
}
