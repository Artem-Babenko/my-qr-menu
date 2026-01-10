<script setup lang="ts">
import { AppFlex, AppIcon } from '.';
import AppInput from './AppInput.vue';

interface AppSearchInputProps {
  placeholder?: string;
}

withDefaults(defineProps<AppSearchInputProps>(), {
  placeholder: 'Пошук...',
});

const model = defineModel<string>({ required: true });
</script>

<template>
  <app-flex class="app-search-input" :gap="7">
    <app-icon name="Search" :size="16"></app-icon>
    <input v-model="model" :placeholder="placeholder">
  </input>
</app-flex>
</template>

<style scoped>
  .app-search-input {
    border: 1px solid var(--border);
    padding: 8px 12px;
    border-radius: 5px;
    width: 200px;
  }
  .app-icon {
    color: var(--secondary-text);
  }
  input {
    border: 0;
    outline: 0;
  }  
  input::placeholder{
    font: var(--font-s);
    color: var(--secondary-text);
  }
</style>