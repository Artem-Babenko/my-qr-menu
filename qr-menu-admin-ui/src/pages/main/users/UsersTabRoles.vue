<script setup lang="ts">
  import { ref } from 'vue';
  import { RoleList } from '@/components/lists';
  import { RoleModal } from '@/components/modals';
  import type { RoleView } from '@/types/roles';

  const modalShowed = ref(false);
  const editingRole = ref<RoleView | null>(null);

  const onAddClick = () => {
    editingRole.value = null;
    modalShowed.value = true;
  };

  const onEdit = (role: RoleView) => {
    editingRole.value = role;
    modalShowed.value = true;
  };

  defineExpose({ onAddClick });
</script>

<template>
  <role-list @edit="onEdit"></role-list>
  <role-modal v-model:showed="modalShowed" :role="editingRole"></role-modal>
</template>
