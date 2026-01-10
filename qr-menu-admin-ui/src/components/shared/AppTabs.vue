<script lang="ts" setup>
  import type { AppTab, AppTabId } from '@/types/components';
  import { AppText } from '.';

  const selected = defineModel<AppTabId>('selected', { required: true });

  defineProps<{ tabs: AppTab[] }>();
</script>

<template>
  <div class="app-tabs">
    <div
      v-for="tab in tabs"
      :key="tab.id"
      class="tab"
      :class="{ selected: selected === tab.id }"
      @click="selected = tab.id"
    >
      <app-text size="xs" weight="500">{{ tab.title }}</app-text>
    </div>
  </div>
</template>

<style scoped>
  .app-tabs {
    display: flex;
  }
  .tab {
    text-align: center;
    padding: 7px 15px;
    border: 1px solid var(--border);
    cursor: pointer;
    transition: 0.2s ease;
  }
  .selected {
    background-color: var(--primary);
    color: var(--text-on-primary);
    border-color: var(--primary);
  }
  .tab:last-of-type {
    border-radius: 0 5px 5px 0;
  }
  .tab:first-of-type {
    border-radius: 5px 0 0 5px;
  }
  .tab + .tab {
    border-left: 0;
  }
</style>
