import "server-only";
import { z } from "zod";

const schema = z.object({
  BFF_URL: z.url(),
  REVALIDATE_SECRET: z.string().min(32),
});

export const env = schema.parse(process.env);
