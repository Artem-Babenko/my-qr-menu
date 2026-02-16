<script lang="ts" setup>
  interface AppCardProps {
    type?: 'white' | 'yellow';
  }

  withDefaults(defineProps<AppCardProps>(), {
    type: 'white',
  });
</script>

<template>
  <div class="app-card" :class="type">
    <slot></slot>
  </div>
</template>

<style scoped>
  .app-card {
    padding: var(--card-padding);
    border-radius: var(--radius);
    border: 1px solid transparent;
  }
  .white {
    border-color: var(--outline-variant);
    background-color: var(--surface-container-lowest);
  }
  .yellow {
    border-color: var(--outline-variant);
    background-color: var(--tertiary-container);
  }
</style>
