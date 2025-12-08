<script setup lang="ts">
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
  <app-input v-model="model" :placeholder="placeholder">
    <!-- TODO: add search icon -->
  </app-input>
</template>
