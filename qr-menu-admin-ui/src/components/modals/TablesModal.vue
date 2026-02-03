<script setup lang="ts">
  import type { Establishment } from '@/types/network';
  import type { TableView } from '@/types/tables';
  import {
    AppButton,
    AppCard,
    AppFlex,
    AppIcon,
    AppModal,
    AppText,
  } from '../shared';

  const showed = defineModel<boolean>('showed', { required: true });

  withDefaults(
    defineProps<{
      establishment?: Establishment | null;
      tables?: TableView[];
      loading?: boolean;
      readonly?: boolean;
    }>(),
    {
      establishment: null,
      tables: () => [],
      loading: false,
      readonly: false,
    },
  );

  const emit = defineEmits<{
    add: [];
    edit: [table: TableView];
    delete: [table: TableView];
  }>();

  const noop = () => {};
</script>

<template>
  <app-modal v-model:showed="showed" title="Редагування столів" :width="650">
    <app-flex justify="space-between" align="center" class="head" :gap="10">
      <app-text v-if="establishment" color="secondary" line="m">
        {{ establishment.name }}
      </app-text>
      <app-button :disabled="readonly" @click="!readonly && emit('add')">
        <app-icon name="Plus" :size="16" :stroke-width="2.5"></app-icon>
        Додати стіл
      </app-button>
    </app-flex>

    <div class="list">
      <app-text v-if="loading" color="secondary">Завантаження...</app-text>

      <app-text v-else-if="tables.length === 0" color="secondary">
        Столів поки немає
      </app-text>

      <app-card
        v-else
        v-for="table in tables"
        :key="table.id"
        class="table-card"
      >
        <app-flex justify="space-between" align="center">
          <app-text weight="500">Стіл №{{ table.number }}</app-text>

          <app-flex class="icons" gap="10" align="center">
            <app-icon
              name="Pencil"
              :class="{ disabled: readonly }"
              @click="!readonly && emit('edit', table)"
            ></app-icon>
            <app-icon name="Copy" @click="noop"></app-icon>
            <app-icon name="QrCode" @click="noop"></app-icon>
            <app-icon
              name="Trash"
              :class="{ disabled: readonly }"
              @click="!readonly && emit('delete', table)"
            ></app-icon>
          </app-flex>
        </app-flex>
      </app-card>
    </div>
  </app-modal>
</template>

<style scoped>
  .head {
    margin-bottom: 15px;
  }

  .list {
    display: flex;
    flex-direction: column;
    gap: 10px;
    margin-top: 10px;
  }

  .table-card {
    padding: 12px 14px;
  }

  .icons .app-icon {
    color: var(--secondary-text);
    transition: all 0.2s ease;
    cursor: pointer;

    &:hover {
      color: var(--primary-text);
    }
  }
  .icons .app-icon.disabled {
    opacity: 0.5;
    cursor: default;
    pointer-events: none;
  }
</style>
