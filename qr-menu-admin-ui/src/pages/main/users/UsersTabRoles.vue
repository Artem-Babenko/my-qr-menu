<script setup lang="ts">
  import { computed, ref } from 'vue';
  import { RoleList } from '@/components/lists';
  import { RoleModal } from '@/components/modals';
  import type { RoleView } from '@/types/roles';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';
  import { AppFlex, AppSearchInput } from '@/components/shared';
  import { AddButton } from '@/components/buttons';

  const { hasAny } = usePermissions();

  const modalShowed = ref(false);
  const editingRole = ref<RoleView | null>(null);
  const search = ref('');

  const canCreate = computed(() => hasAny(PermissionType.rolesCreate));
  const canEdit = computed(() => hasAny(PermissionType.rolesUpdate));

  const onAddClick = () => {
    if (!canCreate.value) return;
    editingRole.value = null;
    modalShowed.value = true;
  };

  const onEdit = (role: RoleView) => {
    if (!canEdit.value) return;
    editingRole.value = role;
    modalShowed.value = true;
  };
</script>

<template>
  <div class="tab">
    <app-flex class="controls" align="center" gap="20">
      <div class="search">
        <app-search-input
          v-model="search"
          placeholder="Пошук ролей..."
        ></app-search-input>
      </div>
      <add-button :disabled="!canCreate" @click="onAddClick">
        Додати роль
      </add-button>
    </app-flex>

    <role-list :search="search" @edit="onEdit"></role-list>
  </div>
  <role-modal v-model:showed="modalShowed" :role="editingRole"></role-modal>
</template>

<style scoped>
  .tab {
    display: flex;
    flex-direction: column;
    gap: 15px;
  }
  .controls {
    width: 100%;
    flex-wrap: wrap;
  }
  .search {
    flex: 1;
  }
  :deep(.search .app-search-input) {
    width: 100%;
  }

  @media (max-width: 768px) {
    .controls {
      gap: 12px;
    }
    .search {
      width: 100%;
      flex: 1 0 100%;
    }
    .controls :deep(.app-button) {
      width: 100%;
      justify-content: center;
    }
  }
</style>
