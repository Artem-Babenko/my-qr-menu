import type {
  CreateEstablishmentReq,
  CreateEstablishmentResp,
  DashboardStats,
  Network,
} from '@/types/network';
import { apiClient } from './api';

export const networkApi = {
  createEstablishment: (req: CreateEstablishmentReq) =>
    apiClient.post<CreateEstablishmentResp>('/network/establishment', req),
  getNetwork: (networkId: number) =>
    apiClient.get<Network>(`/network/${networkId}`),
  getDashboardStats: (networkId: number) =>
    apiClient.get<DashboardStats>(`/network/${networkId}/dashboard-stats`),
  updateNetworkName: (networkId: number, payload: { name: string }) =>
    apiClient.put<void>(`/network/${networkId}`, payload),
  updateEstablishment: (
    establishmentId: number,
    payload: { name: string; address: string },
  ) => apiClient.put<void>(`/establishments/${establishmentId}`, payload),
  deleteEstablishment: (establishmentId: number) =>
    apiClient.delete<void>(`/establishments/${establishmentId}`),
};
