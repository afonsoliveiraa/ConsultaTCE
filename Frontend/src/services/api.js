import axios from 'axios';

const api = axios.create({
    baseURL: '/api',
});

api.interceptors.request.use((config) => {
    // Aqui você injeta a chave UMA ÚNICA VEZ para todas as rotas do app
    config.headers['X-App-Secret'] = import.meta.env.VITE_APP_SECRET;
    return config;
});

export default api;