import type { PermissionType } from '@/consts/roles';

export interface RoleView {
  id: number;
  name: string;
  description: string;
  numberOfUsers: number;
  permissions: PermissionType[];
}
