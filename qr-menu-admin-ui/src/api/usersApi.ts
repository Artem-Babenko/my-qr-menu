import type { User } from '@/types/user';
import { api } from './api';

export const usersApi = {
  current: () => api.get<User>('/user/current'),
};
