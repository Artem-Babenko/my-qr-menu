import { PermissionType } from '@/consts/roles';
import { computed } from 'vue';
import { useRolesStore } from '@/store/roles';
import { useUserStore } from '@/store/user';

export function usePermissions() {
  const userStore = useUserStore();
  const rolesStore = useRolesStore();

  const roleById = computed(() => {
    const roles = rolesStore.roles ?? [];
    return new Map(roles.map((r) => [r.id, r]));
  });

  const permissionsByEstablishmentId = computed(() => {
    const accesses = userStore.user?.accesses ?? [];
    const map = new Map<number, Set<PermissionType>>();

    for (const access of accesses) {
      const role = roleById.value.get(access.roleId);
      if (!role) continue;
      const set = map.get(access.establishmentId) ?? new Set<PermissionType>();
      for (const p of role.permissions) set.add(p);
      map.set(access.establishmentId, set);
    }

    return map;
  });

  const has = (permission: PermissionType, establishmentId?: number | null) => {
    if (!userStore.user) return false;
    if (!establishmentId) return false;
    const set = permissionsByEstablishmentId.value.get(establishmentId);
    if (!set) return false;
    return set.has(permission);
  };

  return { has, permissionsByEstablishmentId };
}
