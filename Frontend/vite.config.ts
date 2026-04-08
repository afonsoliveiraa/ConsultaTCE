import { defineConfig, loadEnv } from "vite"; // Importamos o loadEnv
import preact from "@preact/preset-vite";
import { resolve } from "node:path";

export default defineConfig(({ mode }) => {
  // Carrega as variáveis de ambiente com base no diretório atual e no modo (dev/prod)
  // O terceiro parâmetro '' indica que queremos carregar todas, não apenas as VITE_
  const env = loadEnv(mode, process.cwd(), '');

  const frontendPort = Number(env.FRONTEND_PORT || "5173");
  const backendHttpsUrl = env.BACKEND_HTTPS_URL || "https://localhost:7113";

  return {
    plugins: [preact()],
    build: {
      outDir: resolve(__dirname, "../ConsultaTCE/wwwroot"),
      emptyOutDir: true,
    },
    server: {
      host: "localhost",
      port: frontendPort,
      strictPort: true,
      open: false,
      proxy: {
        "/api": {
          target: backendHttpsUrl,
          changeOrigin: true,
          secure: false,
        },
        "/swagger": {
          target: backendHttpsUrl,
          changeOrigin: true,
          secure: false,
        },
      },
    },
  };
});