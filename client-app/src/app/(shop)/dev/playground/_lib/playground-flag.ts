// Dev-only playground. A production build hides it unless built with
// ENABLE_DEV_PLAYGROUND=true (read at build time), which is useful for checking
// production-only behaviour such as hidden server error messages.
export const isPlaygroundEnabled =
  process.env.NODE_ENV !== "production" || process.env.ENABLE_DEV_PLAYGROUND === "true";
