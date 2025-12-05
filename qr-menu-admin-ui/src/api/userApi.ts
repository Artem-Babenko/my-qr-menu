import type { User } from '@/types/user';
import { api } from './api';

export const userApi = {
  current: () => api.get<User>('/user'),
};
