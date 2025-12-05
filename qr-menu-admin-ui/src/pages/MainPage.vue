<script setup lang="ts">
import { AppIcon, AppSelect, AppText } from '@/components/shared';
import { ref } from 'vue';
const options = ['323', '323243', '434324'];

const option = ref<string>(options[0]!);
</script>

<template>
  <div>
    <app-text>Головна сторінка</app-text>
    <app-select v-model="option" :options="options">
      <template #before>
        <app-icon name="Store" :size="16"></app-icon>
      </template>
    </app-select>
  </div>
</template>

<style scoped></style>
