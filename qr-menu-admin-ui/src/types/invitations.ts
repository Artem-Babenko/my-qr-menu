import type { InvitationStatus } from '@/consts/invitations';

export interface NewInvitation {
  phone: string | null;
  name: string | null;
  surname: string | null;
  targetUserId: number | null;
  establishmentId: number | null;
  roleId: number | null;
}

export interface Invitation extends NewInvitation {
  id: string;
  status: InvitationStatus;
  createdAt: string;
  expiredAt: string;
  establishmentId: number;
  roleId: number;
}

export interface UserInvitation {
  id: string;
  roleId: number;
  roleName: string;
  networkId: number;
  establishmentId: number;
  establishmentName: string;
  establishmentAddress: string;
  status: InvitationStatus;
  createdAt: string;
  expiredAt: string;
  phone: string;
  name: string;
  surname: string;
}

export interface InvitationRequest {
  establishmentId: number;
  roleId: number;
}

export interface InvitationForExisting extends InvitationRequest {
  targetUserId: number;
}

export interface InvitationForNew extends InvitationRequest {
  phone: string;
  name: string;
  surname: string;
}

export interface InvitationResp {
  invitationId: string;
}
