<script setup lang="ts">
  interface AppInputProps {
    placeholder?: string;
    type?: 'text' | 'textarea';
    id?: string;
    disabled?: boolean;
  }

  defineProps<AppInputProps>();

  const model = defineModel<string | null>({
    required: true,
  });
</script>

<template>
  <div class="app-input">
    <textarea
      v-if="type === 'textarea'"
      v-model="model"
      :id="id"
      :placeholder="placeholder"
      :disabled="disabled"
      class="app-input-inner textarea"
      :class="{ disabled }"
      rows="3"
    ></textarea>
    <input
      v-else
      v-model="model"
      :id="id"
      :placeholder="placeholder"
      :disabled="disabled"
      class="app-input-inner"
      :class="{ disabled }"
    />
  </div>
</template>

<style scoped>
  .app-input {
    position: relative;
    width: 100%;
    display: flex;
  }
  .app-input-inner {
    padding: 0 clamp(10px, 2.5vw, 14px);
    height: clamp(38px, 9vmin, 44px);
    border-radius: var(--radius-sm);
    flex: 1;
    border: 1px solid var(--border);
    outline: 0;
    transition: 0.2s ease;
    font: var(--font-s);
    background: #ffffff;
  }
  .app-input-inner.textarea {
    height: auto;
    padding: clamp(8px, 2vw, 12px) clamp(10px, 2.5vw, 14px);
    resize: vertical;
  }
  .app-input-inner::placeholder {
    font: var(--font-s);
    color: var(--secondary-text);
  }
  .app-input-inner:hover:not(.disabled),
  .app-input-inner:focus {
    border-color: var(--primary);
  }
  .app-input-inner.disabled {
    opacity: 0.7;
  }
</style>
