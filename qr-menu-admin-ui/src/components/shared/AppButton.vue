<script setup lang="ts">
  type ButtonTypes = 'filled' | 'outlined' | 'text';

  withDefaults(defineProps<{ type?: ButtonTypes }>(), {
    type: 'filled',
  });
</script>

<template>
  <button class="app-button" :class="type">
    <slot></slot>
  </button>
</template>

<style scoped>
  .app-button {
    border: 0;
    outline: 0;
    background-color: transparent;
    font: var(--font-s);
    font-weight: 600;
    padding: 0 12px;
    border-radius: 6px;
    cursor: pointer;
    position: relative;
    transition: all 0.2s ease;
    display: flex;
    align-items: center;
    justify-content: start;
    height: 36px;
    gap: 10px;
  }

  .filled {
    background-color: var(--primary);
    color: var(--text-on-primary);
  }
  .filled:hover {
    background-color: var(--hover-on-primary);
  }

  .text {
    background-color: var(--secondary);
    color: var(--text-on-secondary);
  }
  .text:hover {
    background-color: var(--hover-on-secondary);
    color: var(--hover-text-on-secondary);
  }
</style>
