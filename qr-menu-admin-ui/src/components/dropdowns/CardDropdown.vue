<script lang="ts" setup>
  import { DotsButton } from '../buttons';
  import { AppButton, AppDropdown, AppIcon, type IconName } from '../shared';

  interface ActionButton {
    icon: IconName;
    title: string;
    click: () => void;
  }

  const buttons: ActionButton[] = [
    { icon: 'Pencil', title: 'Редагувати', click: () => {} },
    { icon: 'Trash', title: 'Видалити', click: () => {} },
  ];
</script>

<template>
  <app-dropdown anchor="bottom right" self="top right">
    <template #target>
      <dots-button></dots-button>
    </template>
    <template #default>
      <div class="menu-content">
        <app-button
          type="text"
          v-for="btn in buttons"
          :key="btn.title"
          @click="btn.click"
        >
          <app-icon :name="btn.icon" :size="14"></app-icon>
          {{ btn.title }}
        </app-button>
      </div>
    </template>
  </app-dropdown>
</template>

<style scoped>
  .menu-content {
    display: flex;
    flex-direction: column;
    gap: 3px;
  }
  .menu-content .app-button {
    text-align: left;
    padding: 5px 10px;
    gap: 10px;
  }
</style>
