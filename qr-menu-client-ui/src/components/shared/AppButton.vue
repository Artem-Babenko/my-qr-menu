<script setup lang="ts">
  type ButtonTypes = 'filled' | 'outline' | 'text';

  interface AppButtonProps {
    type?: ButtonTypes;
    disabled?: boolean;
  }

  withDefaults(defineProps<AppButtonProps>(), {
    type: 'filled',
  });
</script>

<template>
  <button class="app-button" :class="[type, { disabled }]" :disabled="disabled">
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
    padding: 0 clamp(12px, 3vw, 16px);
    border-radius: var(--radius-sm);
    position: relative;
    transition: all 0.2s ease;
    display: flex;
    align-items: center;
    justify-content: center;
    height: clamp(40px, 10vmin, 48px);
    gap: clamp(6px, 1.5vw, 10px);
    min-width: 0;
  }

  .app-button:hover:not(.disabled) {
    cursor: pointer;
  }

  .filled {
    background-color: var(--primary);
    color: var(--text-on-primary);
  }
  .filled:hover:not(.disabled) {
    background-color: var(--hover-on-primary);
  }

  .outline,
  .text {
    background-color: var(--secondary);
    color: var(--text-on-secondary);
  }
  .outline:hover:not(.disabled),
  .text:hover:not(.disabled) {
    background-color: var(--hover-on-secondary);
    color: var(--hover-text-on-secondary);
  }

  .outline {
    border: 1px solid var(--border);
  }

  .disabled {
    opacity: 0.6;
  }
</style>
