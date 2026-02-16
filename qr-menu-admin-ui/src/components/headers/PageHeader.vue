<script lang="ts" setup>
  import { AppFlex, AppText } from '../shared';
  import UserInfo from './UserInfo.vue';

  defineProps<{ sectionName: string }>();
</script>

<template>
  <app-flex class="header" justify="space-between">
    <app-text size="xl" weight="600">{{ sectionName }}</app-text>
    <user-info></user-info>
  </app-flex>
</template>

<style scoped>
  .header {
    height: 50px;
  }
</style>
