import type { RoleRequest, RoleView } from '@/types/roles';
import { apiClient } from './api';

export const rolesApi = {
  all: (networkId: number) => apiClient.get<RoleView[]>(`roles/${networkId}`),
  create: (payload: RoleRequest) =>
    apiClient.post<RoleView>('/roles/create', payload),
  update: (roleId: number, payload: RoleRequest) =>
    apiClient.put<RoleView>(`/roles/${roleId}`, payload),
  delete: (roleId: number) => apiClient.delete<void>(`/roles/${roleId}`),
};
