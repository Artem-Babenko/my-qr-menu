import type {
  LoginReq,
  LoginResp,
  RegistrationReq,
  RegistrationResp,
} from '@/types/auth';
import { api } from './api';

export const authApi = {
  login: (req: LoginReq) => api.post<LoginResp>('/auth/login', req),
  reg: (req: RegistrationReq) => api.post<RegistrationResp>('auth/reg', req),
};
