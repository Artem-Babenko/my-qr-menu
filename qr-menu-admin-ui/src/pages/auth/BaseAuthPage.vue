<script setup lang="ts">
import { AppText } from '@/components/shared';

defineProps<{ pageTitle: string }>();
</script>

<template>
  <div class="base-auth-page">
    <app-text weight="600" size="xl">{{ pageTitle }}</app-text>
    <slot></slot>
  </div>
</template>

<style scoped lang="scss">
.base-auth-page {
  margin: 60px auto;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 10px;

  :deep(.app-button) {
    margin-top: 10px;
    min-width: 100px;
  }
  :deep(.app-input) {
    width: 250px;
  }
}
</style>
