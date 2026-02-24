<script lang="ts" setup>
  import type { Ref } from 'vue';
  import { inject } from 'vue';
  import { AppButton, AppFlex, AppIcon, AppText } from '../shared';
  import UserInfo from './UserInfo.vue';

  defineProps<{ sectionName: string }>();

  const navControl = inject<{
    canToggleNav: Ref<boolean>;
    toggleNav: () => void;
  } | null>('layoutNavControl', null);
</script>

<template>
  <app-flex class="header" justify="space-between">
    <app-flex class="title-wrap" align="center" :gap="8">
      <app-button
        v-if="navControl?.canToggleNav.value"
        class="burger-btn"
        type="text"
        @click="navControl.toggleNav"
      >
        <app-icon name="Menu" :size="20"></app-icon>
      </app-button>
      <app-text size="xl" weight="600">{{ sectionName }}</app-text>
    </app-flex>
    <user-info></user-info>
  </app-flex>
</template>

<style scoped>
  .header {
    min-height: 50px;
    align-items: center;
    gap: 10px;
  }
  .title-wrap {
    flex: 1;
    min-width: 0;
  }
  .title-wrap :deep(.app-text) {
    line-height: 1.2;
  }
  .burger-btn {
    width: 34px;
    height: 34px;
    padding: 0;
    justify-content: center;
    flex-shrink: 0;
  }

  @media (max-width: 900px) {
    .header {
      align-items: flex-start;
    }
  }

  @media (max-width: 640px) {
    .header {
      flex-wrap: wrap;
      align-items: flex-start;
    }
  }
</style>
