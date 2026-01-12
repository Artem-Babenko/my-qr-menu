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
    padding: 20px;
    border-radius: 10px;
    border: 1px solid transparent;
  }
  .white {
    border-color: var(--border);
    background-color: var(--background);
  }
  .yellow {
    border-color: var(--accent);
    background-color: var(--hover-on-accent);
  }
</style>
