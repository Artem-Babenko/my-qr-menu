<script lang="ts" setup>
import { AppSearchInput, AppText } from '@/components/shared';
import { ref } from 'vue';

const search = ref('');
</script>

<template>
  <div class="page">
    <div class="header">
      <app-text size="xl" weight="600">Користувачі</app-text>
      <app-text color="secondary">
        Управління ролями та правами доступу
      </app-text>
      <app-search-input v-model="search"></app-search-input>
    </div>
    <div class="main"></div>
  </div>
</template>

<style scoped></style>
