<script setup lang="ts">
  import { AppText } from '@/components/shared';

  defineProps<{
    title: string;
    subtitle?: string;
  }>();
</script>

<template>
  <div class="page-header">
    <app-text size="xl" weight="600" line="m">{{ title }}</app-text>
    <app-text v-if="subtitle" size="s" color="secondary" line="m">
      {{ subtitle }}
    </app-text>
  </div>
</template>

<style scoped>
  .page-header {
    display: flex;
    flex-direction: column;
    gap: clamp(2px, 0.5vmin, 4px);
    margin-bottom: var(--gap-lg);
  }
</style>
