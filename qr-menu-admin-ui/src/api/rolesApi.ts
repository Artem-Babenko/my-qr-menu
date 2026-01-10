import type { RoleView } from '@/types/roles';
import { apiClient } from './api';

export const rolesApi = {
  all: (networkId: number) => apiClient.get<RoleView[]>(`roles/${networkId}`),
};
