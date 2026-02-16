<script setup lang="ts">
  export type ButtonType = 'filled' | 'outline' | 'text' | 'tonal';

  interface AppButtonProps {
    type?: ButtonType;
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
    padding: 0 12px;
    border-radius: 12px;
    position: relative;
    transition:
      background-color 0.2s ease,
      color 0.2s ease;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    height: 36px;
    gap: 10px;
    cursor: pointer;
    position: relative;
    overflow: hidden;
  }

  .app-button::after {
    content: '';
    position: absolute;
    inset: 0;
    background-color: currentColor;
    opacity: 0;
    transition: opacity 0.2s ease;
    pointer-events: none;
  }
  .app-button:hover:not(.disabled)::after {
    opacity: 0.08;
  }
  .app-button:active:not(.disabled)::after {
    opacity: 0.12;
  }

  .filled {
    background-color: var(--primary);
    color: var(--on-primary);
  }
  .tonal {
    background-color: var(--secondary-container);
    color: var(--on-secondary-container);
  }
  .outline {
    background-color: transparent;
    color: var(--primary);
    border: 1px solid var(--outline);
  }
  .text {
    background-color: transparent;
    color: var(--primary);
  }

  .disabled {
    opacity: 0.38;
    pointer-events: none;
  }
</style>
