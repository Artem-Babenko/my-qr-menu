<script lang="ts" setup>
  interface AppCardProps {
    type?: 'default' | 'outlined' | 'tonal' | 'elevated';
  }

  withDefaults(defineProps<AppCardProps>(), {
    type: 'default',
  });
</script>

<template>
  <div class="app-card" :class="type">
    <slot></slot>
  </div>
</template>

<style scoped>
  .app-card {
    padding: 20px;
    border-radius: 16px;
    background-color: var(--surface-container-low);
    color: var(--on-surface);
    border: 1px solid transparent;
    transition: all 0.2s ease;
  }
  .outlined {
    border: 1px solid var(--outline-variant);
    background-color: var(--surface-container);
  }
  .tonal {
    background-color: var(--secondary-container);
    color: var(--on-secondary-container);
  }
  .elevated {
    background-color: var(--surface-container-high);
    color: var(--on-surface);
  }
</style>
