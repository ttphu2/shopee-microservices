import type { ComponentProps } from "react";
import { cn } from "@/shared/lib/cn";
import styles from "./button.module.scss";

type Props = ComponentProps<"button"> & {
  variant?: "primary" | "secondary" | undefined;
};

// Native <button> until react-aria-components is installed correctly; the props
// stay compatible so callers don't change when it moves to RAC's Button.
export function Button({ variant = "primary", className, type = "button", ...props }: Props) {
  return (
    <button
      type={type}
      className={cn(styles.button, styles[variant], className)}
      {...props}
    />
  );
}
