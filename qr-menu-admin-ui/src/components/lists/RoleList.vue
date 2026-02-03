<script lang="ts" setup>
  import { RoleCard } from '../cards';
  import BaseCardList from './BaseCardList.vue';
  import { useRolesStore } from '@/store/roles';
  import type { RoleView } from '@/types/roles';
  import { rolesApi } from '@/api/rolesApi';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';

  const emit = defineEmits<{ edit: [role: RoleView] }>();

  const rolesStore = useRolesStore();
  const { hasAny } = usePermissions();

  const canDelete = () => hasAny(PermissionType.rolesDelete);

  const openEdit = (role: RoleView) => {
    emit('edit', role);
  };

  const deleteRole = async (role: RoleView) => {
    if (!canDelete()) return;
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
