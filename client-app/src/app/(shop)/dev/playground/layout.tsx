import { notFound } from "next/navigation";
import { isPlaygroundEnabled } from "./_lib/playground-flag";

export default function PlaygroundLayout({ children }: LayoutProps<"/dev/playground">) {
  if (!isPlaygroundEnabled) notFound();
  return children;
}
