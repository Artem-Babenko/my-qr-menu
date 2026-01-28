import type { PermissionType } from '@/consts/roles';

export interface RoleView {
  id: number;
  name: string;
  description: string;
  numberOfUsers: number;
  permissions: PermissionType[];
}

export interface RoleRequest {
  name: string;
  description?: string | null;
  networkId: number;
  permissions: PermissionType[];
}

export interface PermissionGroup {
  name: string;
  permissions: PermissionType[];
}
