import type { ApiResponse } from '@/types/api';
import axios, { AxiosError, type AxiosRequestConfig } from 'axios';

const apiInstance = axios.create({
  baseURL: 'http://localhost:5196',
});

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
};
