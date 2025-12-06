import { tokenStorage } from '@/utils/tokenStorage';
import axios, { AxiosError, HttpStatusCode } from 'axios';

export const api = axios.create({
  baseURL: 'http://localhost:5195',
});

export function initApi(onUnauthorized: () => void) {
  api.interceptors.request.use((config) => {
    const token = tokenStorage.get();
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
  });

  api.interceptors.response.use(
    (response) => response,
    (error: AxiosError) => {
      const status = error.response?.status;

      if (status === HttpStatusCode.Unauthorized) {
        onUnauthorized();
      }

      return Promise.reject(error);
    },
  );
}
