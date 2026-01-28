import type { RoleView } from '@/types/roles';
import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useRolesStore = defineStore('roles', () => {
  const roles = ref<RoleView[] | null>(null);

  const addRole = (role: RoleView) => {
    if (!roles.value) {
      roles.value = [];
    }
    roles.value.push(role);
  };

  const updateRole = (role: RoleView) => {
    if (!roles.value) return;
    const index = roles.value.findIndex((r) => r.id === role.id);
    if (index !== -1) {
      roles.value[index] = role;
    }
  };

  const deleteRole = (role: RoleView) => {
    if (!roles.value) return;
    const index = roles.value.findIndex((r) => r.id === role.id);
    if (index !== -1) {
      roles.value.splice(index, 1);
    }
  };

  return { roles, addRole, updateRole, deleteRole };
});
