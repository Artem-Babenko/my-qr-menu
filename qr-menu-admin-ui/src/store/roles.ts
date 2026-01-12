import type { RoleView } from '@/types/roles';
import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useRolesStore = defineStore('roles', () => {
  const roles = ref<RoleView[] | null>(null);

  return { roles };
});
