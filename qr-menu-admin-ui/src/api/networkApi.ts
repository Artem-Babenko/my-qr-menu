import type {
  CreateEstablishmentReq,
  CreateEstablishmentResp,
  Network,
} from '@/types/network';
import { apiClient } from './api';

export const networkApi = {
  createEstablishment: (req: CreateEstablishmentReq) =>
    apiClient.post<CreateEstablishmentResp>('/network/establishment', req),
  getNetwork: (networkId: number) =>
    apiClient.get<Network>(`/network/${networkId}`),
};
