import type { User } from '@/types/user';
import { apiClient } from './api';

export const usersApi = {
  current: () => apiClient.get<User>('/users/current'),
  search: (word: string) =>
    apiClient.get<User[]>('/users/search', { params: { query: word } }),
};
