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

  const hasAny = (permission: PermissionType) => {
    for (const set of permissionsByEstablishmentId.value.values()) {
      if (set.has(permission)) return true;
    }
    return false;
  };

  const hasAnyOf = (permissions: PermissionType[]) => {
    if (!permissions.length) return false;
    for (const set of permissionsByEstablishmentId.value.values()) {
      for (const p of permissions) {
        if (set.has(p)) return true;
      }
    }
    return false;
  };

  return { has, hasAny, hasAnyOf, permissionsByEstablishmentId };
}
