<script setup lang="ts">
  import type { CategoryView } from '@/types/categories';
  import { computed } from 'vue';
  import { AppCard, AppFlex, AppText } from '../shared';
  import { CardDropdown } from '../dropdowns';
  import type { ActionButton } from '../dropdowns';

  const props = defineProps<{ category: CategoryView }>();
  const emit = defineEmits<{
    edit: [category: CategoryView];
    delete: [category: CategoryView];
  }>();

  const buttons = computed<ActionButton[]>(() => [
    {
      icon: 'Pencil',
      title: 'Редагувати',
      click: () => emit('edit', props.category),
    },
    {
      icon: 'Trash',
      title: 'Видалити',
      click: () => emit('delete', props.category),
    },
  ]);
</script>

<template>
  <app-card class="card">
    <app-flex justify="space-between" align="center" :gap="10">
      <app-flex direction="column" align="flex-start" :gap="6" class="info">
        <app-text weight="600" class="title">
          {{ category.name }}
          <app-text v-if="!category.isActive" size="xs" color="secondary">
            (неактивна)
          </app-text>
        </app-text>

        <app-text v-if="category.description" color="secondary" size="xs" line="m">
          {{ category.description }}
        </app-text>
      </app-flex>

      <card-dropdown :buttons="buttons"></card-dropdown>
    </app-flex>
  </app-card>
</template>

<style scoped>
  .card {
    padding: 14px 16px;
  }

  .info {
    min-width: 0;
  }

  .title {
    display: flex;
    align-items: center;
    gap: 8px;
    max-width: 100%;
    overflow: hidden;
    text-overflow: ellipsis;
  }
</style>

