import type { ApiResponse } from '@/types/api';
import { tokenStorage } from '@/utils/tokenStorage';
import axios, { AxiosError, type AxiosRequestConfig } from 'axios';

const apiInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5195',
});

export function initApi(onUnauthorized: () => void) {
  apiInstance.interceptors.request.use((config) => {
    const token = tokenStorage.get();
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
  });

  apiInstance.interceptors.response.use(
    (response) => response,
    (error: AxiosError) => {
      if (error.response?.status === 401) {
        onUnauthorized();
      }
      return Promise.reject(error);
    },
  );
}

async function wrap<T>(promise: Promise<any>): Promise<ApiResponse<T>> {
  try {
    const resp = await promise;
    return resp.data;
  } catch (e: any) {
    const err = e as AxiosError;
    const raw = err.response?.data;

    if (raw && typeof raw === 'object' && 'success' in raw) {
      return raw as ApiResponse<T>;
    }

    return {
      success: false,
      data: null,
      errorCode: (raw as any)?.errorCode || 'unknown_error',
    };
  }
}

export const apiClient = {
  get: <T>(url: string, config?: AxiosRequestConfig) =>
    wrap<T>(apiInstance.get(url, config)),

  post: <T>(url: string, data?: unknown, config?: AxiosRequestConfig) =>
    wrap<T>(apiInstance.post(url, data, config)),

  put: <T>(url: string, data?: unknown, config?: AxiosRequestConfig) =>
    wrap<T>(apiInstance.put(url, data, config)),

  delete: <T>(url: string, config?: AxiosRequestConfig) =>
    wrap<T>(apiInstance.delete(url, config)),
};
