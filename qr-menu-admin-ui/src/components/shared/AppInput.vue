<script setup lang="ts">
  import AppIcon from './AppIcon.vue';
  import { computed, ref } from 'vue';

  interface AppInputProps {
    placeholder?: string;
    type?: 'text' | 'password';
    mask?: string | RegExp | object;
    id?: string;
    disabled?: boolean;
    error?: boolean;
    variant?: 'filled' | 'outlined' | 'standard';
  }

  const props = defineProps<AppInputProps>();

  const model = defineModel<string | null>({
    required: true,
  });

  const isPasswordVisible = ref(false);

  const inputType = computed(() =>
    props.type === 'password' && isPasswordVisible.value ? 'text' : props.type,
  );

  const isPassword = computed(() => props.type === 'password');

  const togglePasswordVisibility = () => {
    if (!isPassword.value) return;
    isPasswordVisible.value = !isPasswordVisible.value;
  };
</script>

<template>
  <div class="app-input" :class="[variant, { disabled, error }]">
    <span v-if="$slots.before" class="slot before">
      <slot name="before"></slot>
    </span>

    <input
      v-model="model"
      :id="id"
      :placeholder="placeholder"
      :type="inputType"
      :disabled="disabled"
      v-maska="mask"
      class="app-input-inner"
      :class="{
        'has-before': !!$slots.before,
        'has-after': !!$slots.after || isPassword,
        disabled,
      }"
    />

    <span v-if="$slots.after" class="slot after">
      <slot name="after"></slot>
    </span>

    <button
      v-else-if="isPassword"
      type="button"
      class="slot after password-toggle"
      @click="togglePasswordVisibility"
    >
      <app-icon :name="isPasswordVisible ? 'EyeOff' : 'Eye'" />
    </button>
  </div>
</template>

<style scoped>
  .app-input {
    position: relative;
    width: 100%;
    display: flex;
  }

  .app-input-inner {
    flex: 1;
    height: 40px;
    padding: 0 12px;
    border-radius: 12px;
    outline: none;
    font: var(--font-s);
    color: var(--on-surface);
    background-color: var(--surface-container-low);
    border: 1px solid var(--outline-variant);
    transition: all 0.2s ease;
  }

  .app-input-inner::placeholder {
    color: var(--secondary-text);
  }

  .app-input-inner:hover:not(.disabled),
  .app-input-inner:focus {
    border-color: var(--primary);
  }

  .app-input.error .app-input-inner {
    border-color: var(--error);
  }

  .app-input.disabled .app-input-inner {
    opacity: 0.38;
    pointer-events: none;
  }

  .slot {
    position: absolute;
    top: 50%;
    transform: translateY(-50%);
    display: flex;
    align-items: center;
    color: var(--on-surface-variant);
  }

  .slot.before {
    left: 10px;
  }

  .slot.after {
    right: 10px;
  }

  .password-toggle {
    background: transparent;
    border: none;
    cursor: pointer;
  }

  .app-input-inner.has-before {
    padding-left: 36px;
  }

  .app-input-inner.has-after {
    padding-right: 36px;
  }
</style>
