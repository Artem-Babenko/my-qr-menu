<script setup lang="ts">
  import { AppIcon, AppInput } from '.';

  interface AppSearchInputProps {
    placeholder?: string;
  }

  withDefaults(defineProps<AppSearchInputProps>(), {
    placeholder: 'Пошук...',
  });

  const model = defineModel<string>({ required: true });
</script>

<template>
  <app-input
    v-model="model"
    class="app-search-input"
    :placeholder="placeholder"
  >
    <template #before>
      <app-icon class="icon" name="Search"></app-icon>
    </template>
  </app-input>
</template>

<style scoped>
  .app-search-input {
    width: 200px;
  }
  .icon {
    color: var(--on-surface-variant);
  }
</style>
