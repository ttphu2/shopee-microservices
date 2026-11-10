import type { ReactNode } from "react";
import { cn } from "@/shared/lib/cn";
import styles from "./container.module.scss";

type Props = {
  children: ReactNode;
  className?: string | undefined;
};

export function Container({ children, className }: Props) {
  return <div className={cn(styles.container, className)}>{children}</div>;
}
