<script lang="ts" setup>
  import { RoleCard } from '../cards';
  import BaseCardList from './BaseCardList.vue';
  import { useRolesStore } from '@/store/roles';
  import type { RoleView } from '@/types/roles';
  import { rolesApi } from '@/api/rolesApi';

  const emit = defineEmits<{ edit: [role: RoleView] }>();

  const rolesStore = useRolesStore();

  const openEdit = (role: RoleView) => {
    emit('edit', role);
  };

  const deleteRole = async (role: RoleView) => {
    const resp = await rolesApi.delete(role.id);
    if (!resp.success) throw resp.errorCode;
    rolesStore.deleteRole(role);
  };
</script>

<template>
  <base-card-list>
    <role-card
      v-for="role in rolesStore.roles"
      :key="role.id"
      :role="role"
      @edit="openEdit(role)"
      @delete="deleteRole(role)"
    ></role-card>
  </base-card-list>
</template>
