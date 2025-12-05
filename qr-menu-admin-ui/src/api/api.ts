import { tokenStorage } from '@/utils/tokenStorage';
import axios from 'axios';

export const api = axios.create({
  baseURL: 'http://localhost:5195',
});

api.interceptors.request.use((config) => {
  const token = tokenStorage.get();
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});
