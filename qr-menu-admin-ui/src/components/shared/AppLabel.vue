<script lang="ts" setup>
  import { AppText } from '.';

  defineProps<{ label: string; for?: string }>();
</script>

<template>
  <label class="app-label" :for="for">
    <app-text class="app-label-inner" weight="500" size="s">
      {{ label }}
    </app-text>
  </label>
</template>

<style scoped>
  .app-label,
  .app-label-inner {
    width: 100%;
    display: block;
  }
  .app-label {
    margin-bottom: 5px;
  }
</style>
