<script lang="ts" setup>
  import type { Establishment } from '@/types/network';
  import { computed } from 'vue';
  import { AppCard, AppFlex, AppIcon, AppText } from '../shared';
  import { CardDropdown } from '../dropdowns';
  import type { ActionButton } from '../dropdowns';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';

  const props = defineProps<{ establishment: Establishment }>();
  const emit = defineEmits<{
    edit: [establishment: Establishment];
    tables: [establishment: Establishment];
    delete: [establishment: Establishment];
  }>();

  const handleEdit = () => emit('edit', props.establishment);
  const handleTables = () => emit('tables', props.establishment);
  const handleDelete = () => emit('delete', props.establishment);
  const { has } = usePermissions();

  const canUpdate = computed(() =>
    has(PermissionType.establishmentsUpdate, props.establishment.id),
  );
  const canDelete = computed(() =>
    has(PermissionType.establishmentsDelete, props.establishment.id),
  );
  const canTables = computed(() =>
    has(PermissionType.tablesView, props.establishment.id),
  );

  const buttons = computed<ActionButton[]>(() => [
    {
      icon: 'Pencil',
      title: 'Редагувати',
      click: handleEdit,
      disabled: () => !canUpdate.value,
    },
    {
      icon: 'Table',
      title: 'Столи',
      click: handleTables,
      disabled: () => !canTables.value,
    },
    {
      icon: 'Trash',
      title: 'Видалити',
      click: handleDelete,
      disabled: () =>
        (props.establishment.usersCount ?? 0) > 0 || !canDelete.value,
    },
  ]);
</script>

<template>
  <app-card class="est-card">
    <app-flex justify="space-between" align="flex-start">
      <app-flex :gap="10">
        <app-flex class="circle-icon" justify="center" align="center">
          <app-icon name="Store" :size="20"></app-icon>
        </app-flex>
        <app-flex direction="column" align="flex-start" :gap="6" class="info">
          <app-text weight="600" class="title">
            {{ establishment.name }}
          </app-text>
          <app-text color="secondary" size="xs" line="m" class="address">
            {{ establishment.address }}
          </app-text>
          <app-text color="secondary" size="xs">
            <app-flex :gap="4" align="center">
              <app-icon name="Users" :size="12"></app-icon>
              {{ establishment.usersCount ?? 0 }} користувачів
            </app-flex>
          </app-text>
        </app-flex>
      </app-flex>
      <card-dropdown :buttons="buttons"></card-dropdown>
    </app-flex>
  </app-card>
</template>

<style scoped>
  .est-card {
    height: 180px;
    display: flex;
    flex-direction: column;
    justify-content: center;
  }

  .info {
    min-width: 0;
  }

  .title,
  .address {
    max-width: 100%;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .address {
    display: -webkit-box;
    -webkit-box-orient: vertical;
    -webkit-line-clamp: 2;
    overflow: hidden;
  }

  .circle-icon {
    background-color: var(--hover-on-secondary);
    width: 40px;
    height: 40px;
    border-radius: 50%;
    flex-shrink: 0;
  }
</style>
