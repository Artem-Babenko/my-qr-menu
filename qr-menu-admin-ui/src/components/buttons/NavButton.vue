<script setup lang="ts">
  import { AppButton, AppIcon, type IconName } from '@/components/shared';

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
    :disabled="disabled"
  >
    <app-icon :name="icon" :size="20"></app-icon>
    {{ label }}
  </app-button>
</template>

<style scoped>
  .nav-button {
    justify-content: start;
  }
</style>
