import axios from "axios";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "/api",
});

api.interceptors.request.use((config) => {
  // Centraliza o envio da chave do front-end em todas as requisicoes.
  config.headers["X-App-Secret"] = import.meta.env.VITE_APP_SECRET;
  return config;
});

export default api;
