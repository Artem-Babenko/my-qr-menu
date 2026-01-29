import type { UpdateUserEstablishmentsRequest, User } from '@/types/user';
import { apiClient } from './api';

export const usersApi = {
  current: () => apiClient.get<User>('/users/current'),
  search: (phone: string) =>
    apiClient.get<User>('/users/search', { params: { phone } }),
  byNetwork: (networkId: number) =>
    apiClient.get<User[]>(`/users/by-network/${networkId}`),
  updateEstablishments: (
    userId: number,
    payload: UpdateUserEstablishmentsRequest,
  ) => apiClient.put<void>(`/users/${userId}/establishments`, payload),
};
