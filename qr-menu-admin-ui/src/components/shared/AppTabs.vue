<script lang="ts" setup>
import type { AppTab } from '@/types/components';
import { AppText } from '.';

const selected = defineModel<AppTab>('selected', { required: true });

defineProps<{ tabs: AppTab[] }>();
</script>

<template>
  <div class="app-tabs">
    <div
      v-for="tab in tabs"
      :key="tab.id"
      class="tab"
      :class="{ selected: selected.id === tab.id }"
      @click="selected = tab"
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
  padding: 5px 15px;
  border: 1px solid transparent;
  border-bottom-color: var(--border);
  cursor: pointer;
}
.tab.selected {
  border-color: var(--primary-text);
}
</style>
