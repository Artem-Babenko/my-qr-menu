<script setup lang="ts">
  interface AppCheckboxProps {
    id?: string;
    disabled?: boolean;
    label?: string;
  }

  defineProps<AppCheckboxProps>();

  const model = defineModel<boolean>({
    required: true,
    default: false,
  });
</script>

<template>
  <label class="app-checkbox" :class="{ disabled }">
    <input
      type="checkbox"
      v-model="model"
      :id="id"
      :disabled="disabled"
      class="app-checkbox-input"
    />
    <span class="app-checkbox-mark"></span>
    <span v-if="label" class="app-checkbox-label">{{ label }}</span>
  </label>
</template>

<style scoped>
  .app-checkbox {
    display: flex;
    align-items: center;
    gap: 8px;
    cursor: pointer;
    user-select: none;
  }

  .app-checkbox.disabled {
    opacity: 0.6;
    cursor: not-allowed;
  }

  .app-checkbox-input {
    position: absolute;
    opacity: 0;
    width: 0;
    height: 0;
  }

  .app-checkbox-mark {
    width: 18px;
    height: 18px;
    border: 2px solid var(--border);
    border-radius: 4px;
    background-color: var(--secondary);
    position: relative;
    transition: all 0.2s ease;
    flex-shrink: 0;
  }

  .app-checkbox:hover:not(.disabled) .app-checkbox-mark {
    border-color: var(--primary);
  }

  .app-checkbox-input:checked + .app-checkbox-mark {
    background-color: var(--primary);
    border-color: var(--primary);
  }

  .app-checkbox-input:checked + .app-checkbox-mark::after {
    content: '';
    position: absolute;
    left: 5px;
    top: 2px;
    width: 5px;
    height: 9px;
    border: solid var(--text-on-primary);
    border-width: 0 2px 2px 0;
    transform: rotate(45deg);
  }

  .app-checkbox-input:focus + .app-checkbox-mark {
    outline: 2px solid var(--primary);
    outline-offset: 2px;
  }

  .app-checkbox-label {
    font: var(--font-s);
    color: var(--text-on-secondary);
  }

  .app-checkbox-input:disabled + .app-checkbox-mark {
    opacity: 0.6;
    cursor: not-allowed;
  }
</style>
