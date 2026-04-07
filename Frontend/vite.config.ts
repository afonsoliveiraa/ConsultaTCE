import { defineConfig } from "vite";
import preact from "@preact/preset-vite";
import { resolve } from "node:path";

export default defineConfig({
  plugins: [preact()],
  build: {
    outDir: resolve(__dirname, "../ConsultaTCE/wwwroot"),
    emptyOutDir: true,
  },
  server: {
    port: 3000,
    proxy: {
      "/api": {
        target: "http://localhost:5130",
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
