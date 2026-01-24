import type {
  Invitation,
  InvitationForExisting,
  InvitationForNew,
  InvitationResp,
  UserInvitation,
} from '@/types/invitations';
import { apiClient } from './api';

export const invitationApi = {
  createForExistingUser: (payload: InvitationForExisting) =>
    apiClient.post<InvitationResp>('/invitations/for-existing-user', payload),
  createForNewUser: (payload: InvitationForNew) =>
    apiClient.post<InvitationResp>('/invitations/for-new-user', payload),
  getByNetwork: (networkId: number) =>
    apiClient.get<Invitation[]>(`/invitations/by-network/${networkId}`),
  getByCurrentUser: () =>
    apiClient.get<UserInvitation[]>('/invitations/by-current-user'),
};
