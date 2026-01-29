<script setup lang="ts">
  import {
    AppButton,
    AppIcon,
    AppText,
    type IconName,
  } from '@/components/shared';

  defineProps<{
    label: string;
    icon: IconName;
    selected: boolean;
    disabled: boolean;
  }>();
</script>

<template>
  <app-button
    class="nav-button"
    :type="selected ? 'filled' : 'text'"
    :class="{ disabled }"
  >
    <div class="nav-button__inner">
      <app-icon :name="icon" :size="20"></app-icon>
      <app-text weight="600">{{ label }}</app-text>
    </div>
  </app-button>
</template>

<style scoped>
  .nav-button__inner {
    display: flex;
    align-items: center;
    justify-content: start;
    gap: 10px;
  }
  .nav-button.disabled {
    opacity: 0.6;
  }
</style>
