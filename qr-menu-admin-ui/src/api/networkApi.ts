import type {
  CreateEstablishmentReq,
  CreateEstablishmentResp,
  Network,
} from '@/types/network';
import { api } from './api';

export const networkApi = {
  createEstablishment: (req: CreateEstablishmentReq) =>
    api.post<CreateEstablishmentResp>('/network/establishment', req),
  getNetwork: (networkId: number) => api.get<Network>(`/network/${networkId}`),
};
