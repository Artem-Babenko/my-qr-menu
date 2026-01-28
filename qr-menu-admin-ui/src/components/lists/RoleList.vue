<script lang="ts" setup>
  import { ref } from 'vue';
  import { RoleCard } from '../cards';
  import BaseCardList from './BaseCardList.vue';
  import { useRolesStore } from '@/store/roles';
  import { RoleModal } from '../modals';
  import type { RoleView } from '@/types/roles';
  import { rolesApi } from '@/api/rolesApi';

  const modalShowed = defineModel<boolean>('showed', { required: true });

  const rolesStore = useRolesStore();

  const editingRole = ref<RoleView | null>(null);

  const openEditModal = (role: RoleView) => {
    editingRole.value = role;
    modalShowed.value = true;
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
      @edit="openEditModal(role)"
      @delete="deleteRole(role)"
    ></role-card>

    <role-modal v-model:showed="modalShowed" :role="editingRole"></role-modal>
  </base-card-list>
</template>
