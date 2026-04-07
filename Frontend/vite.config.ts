import { defineConfig } from "vite";
import preact from "@preact/preset-vite";
import { resolve } from "node:path";

const frontendPort = Number(process.env.FRONTEND_PORT ?? "3000");
const backendHttpsUrl = process.env.BACKEND_HTTPS_URL ?? "https://localhost:7113";

export default defineConfig({
  plugins: [preact()],
  build: {
    // O build do frontend alimenta diretamente o wwwroot servido pelo projeto ASP.NET.
    outDir: resolve(__dirname, "../ConsultaTCE/wwwroot"),
    emptyOutDir: true,
  },
  server: {
    port: frontendPort,
    strictPort: true,
    // O frontend de desenvolvimento sobe apenas em HTTPS para alinhar com o backend.
    https: true,
    proxy: {
      "/api": {
        // Em desenvolvimento, toda chamada para /api vai para a porta HTTPS do backend.
        target: backendHttpsUrl,
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
