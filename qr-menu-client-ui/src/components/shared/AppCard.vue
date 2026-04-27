<script lang="ts" setup>
  interface AppCardProps {
    type?: 'default' | 'outlined' | 'tonal' | 'elevated' | 'accent';
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
    padding: var(--card-padding);
    border-radius: var(--radius-card);
    border: 1px solid transparent;
    background-color: var(--app-card-bg);
    color: var(--on-surface);
    box-shadow: var(--app-card-shadow);
    transition:
      background-color 0.2s ease,
      box-shadow 0.2s ease;
  }

  .outlined {
    border-color: var(--outline-variant);
    background-color: var(--surface-container);
    box-shadow: var(--app-card-shadow);
  }

  .tonal {
    background-color: var(--secondary-container);
    color: var(--on-secondary-container);
    box-shadow: var(--app-card-shadow);
  }

  .elevated {
    background-color: var(--surface-container-high);
    color: var(--on-surface);
    box-shadow: var(--app-card-shadow);
  }

  .accent {
    background-color: var(--tertiary-container);
    color: var(--on-tertiary-container);
    box-shadow: var(--app-card-shadow);
  }

  @media (max-width: 640px) {
    .app-card {
      padding: clamp(12px, 3vw, 14px);
      border-radius: clamp(12px, 3.2vmin, 14px);
    }
  }
</style>
