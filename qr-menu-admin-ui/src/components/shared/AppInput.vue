<script setup lang="ts">
  interface AppInputProps {
    placeholder?: string;
    type?: 'text' | 'password';
    mask?: string | RegExp | object;
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
    <span v-if="$slots.before" class="slot before">
      <slot name="before"></slot>
    </span>
    <input
      v-model="model"
      :id="id"
      :placeholder="placeholder"
      :type="type"
      :disabled="disabled"
      v-maska="mask"
      class="app-input-inner"
      :class="{
        'has-before': !!$slots.before,
        'has-after': !!$slots.after,
        disabled: disabled,
      }"
    />
    <span v-if="$slots.after" class="slot after">
      <slot name="after"></slot>
    </span>
  </div>
</template>

<style scoped>
  .app-input {
    position: relative;
    width: 100%;
    display: flex;
  }
  .app-input-inner {
    padding: 0 12px;
    height: 34px;
    border-radius: 5px;
    flex: 1;
    border: 1px solid var(--border);
    outline: 0;
    transition: 0.2s ease;
  }
  .app-input-inner::placeholder {
    font: var(--font-s);
    color: var(--secondary-text);
  }
  .app-input-inner:hover:not(.disabled),
  .app-input-inner:focus {
    border-color: var(--primary);
  }
  .slot {
    position: absolute;
    top: 50%;
    transform: translateY(-50%);
    display: flex;
    align-items: center;
    color: var(--text-muted);
  }
  .slot.before {
    left: 10px;
  }
  .slot.after {
    right: 10px;
  }
  .app-input-inner.has-before {
    padding-left: 36px;
  }
  .app-input-inner.has-after {
    padding-right: 36px;
  }
  .app-input-inner.disabled {
    opacity: 0.7;
  }
</style>
