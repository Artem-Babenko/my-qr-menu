<script lang="ts" setup>
  import {
    AppButton,
    AppIcon,
    AppSearchInput,
    AppTabs,
    AppText,
  } from '@/components/shared';
  import { UserPageTab } from '@/consts/tabs';
  import type { AppTab } from '@/types/components';
  import { computed } from 'vue';

  const props = defineProps<{
    tabs?: AppTab[];
    addDisabled?: boolean;
    hideAddButton?: boolean;
  }>();

  const selectedTab = defineModel<UserPageTab>('selectedTab', {
    required: true,
  });
  const search = defineModel<string>('search', { required: true });

  const emit = defineEmits<{ addButtonClick: [] }>();

  const defaultTabs: AppTab[] = [
    { id: UserPageTab.users, title: 'Користувачі' },
    { id: UserPageTab.roles, title: 'Ролі' },
    { id: UserPageTab.invites, title: 'Запрошення' },
  ];

  const tabs = computed(() => props.tabs ?? defaultTabs);

  const buttonTitles: Record<UserPageTab, string> = {
    [UserPageTab.users]: 'Запросити користувача',
    [UserPageTab.roles]: 'Додати роль',
    [UserPageTab.invites]: 'Створити запрошення',
  };

  const placeholders: Record<UserPageTab, string> = {
    [UserPageTab.users]: 'Пошук користувачів...',
    [UserPageTab.roles]: 'Пошук ролей...',
    [UserPageTab.invites]: 'Пошук запрошень...',
  };
</script>

<template>
  <div class="header">
    <app-text size="xxl" weight="600">Користувачі та ролі</app-text>
    <app-text color="secondary">
      Управління користувачами, ролями та правами доступу
    </app-text>
  </div>
  <div class="navigation">
    <app-tabs v-model:selected="selectedTab" :tabs="tabs"></app-tabs>
    <div class="right">
      <app-search-input
        v-model="search"
        :placeholder="placeholders[selectedTab]"
      ></app-search-input>
      <app-button
        v-if="!hideAddButton"
        :disabled="addDisabled"
        @click="emit('addButtonClick')"
      >
        <app-icon name="Plus" :size="15"></app-icon>
        {{ buttonTitles[selectedTab] }}
      </app-button>
    </div>
  </div>
</template>

<style scoped lang="scss">
  .header {
    display: flex;
    flex-direction: column;
    gap: 10px;
  }
  .navigation {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin: 20px 0;

    .right {
      display: flex;
      align-items: center;
      gap: 20px;

      .app-button {
        white-space: nowrap;
      }
    }
  }
</style>
