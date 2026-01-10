import type {
  LoginReq,
  LoginResp,
  RegistrationReq,
  RegistrationResp,
} from '@/types/auth';
import { apiClient } from './api';

export const authApi = {
  login: (req: LoginReq) => apiClient.post<LoginResp>('/auth/login', req),
  reg: (req: RegistrationReq) =>
    apiClient.post<RegistrationResp>('auth/reg', req),
};
