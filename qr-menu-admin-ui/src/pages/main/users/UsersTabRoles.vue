<script setup lang="ts">
  import { ref } from 'vue';
  import { RoleList } from '@/components/lists';
  import { RoleModal } from '@/components/modals';
  import type { RoleView } from '@/types/roles';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';

  const { hasAny } = usePermissions();

  const modalShowed = ref(false);
  const editingRole = ref<RoleView | null>(null);

  const canCreate = () => hasAny(PermissionType.rolesCreate);
  const canEdit = () => hasAny(PermissionType.rolesUpdate);

  const onAddClick = () => {
    if (!canCreate()) return;
    editingRole.value = null;
    modalShowed.value = true;
  };

  const onEdit = (role: RoleView) => {
    if (!canEdit()) return;
    editingRole.value = role;
    modalShowed.value = true;
  };

  defineExpose({ onAddClick });
</script>

<template>
  <role-list @edit="onEdit"></role-list>
  <role-modal v-model:showed="modalShowed" :role="editingRole"></role-modal>
</template>
