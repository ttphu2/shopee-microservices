import type { NextConfig } from "next";
import path from "node:path";

const nextConfig: NextConfig = {
  /* config options here */
  experimental: {
    agentFeedback: true,
  },
  output: "standalone",
  cacheComponents: true,
  typedRoutes: true,
  partialPrefetching: true,
  reactCompiler: true,
  sassOptions: {
    loadPaths: [path.join(process.cwd(), "src/shared/ui/styles")],
  },
};

export default nextConfig;
