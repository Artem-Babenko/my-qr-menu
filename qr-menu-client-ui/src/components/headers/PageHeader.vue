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
    gap: clamp(4px, 1vmin, 6px);
    margin-bottom: clamp(16px, 4vw, 22px);
  }
</style>
