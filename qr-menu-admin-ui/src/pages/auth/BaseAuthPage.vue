<script setup lang="ts">
  import { AppCard, AppFlex, AppText } from '@/components/shared';

  defineProps<{ pageTitle: string }>();
</script>

<template>
  <app-flex class="base-auth-page" direction="column">
    <app-text weight="600" size="xl" class="site-name">My QR-Menu</app-text>
    <app-card class="container" direction="column">
      <app-text class="page-title" weight="600" size="l">
        {{ pageTitle }}
      </app-text>
      <slot></slot>
    </app-card>
  </app-flex>
</template>

<style scoped lang="scss">
  .base-auth-page {
    justify-content: center;
    background-color: var(--background);
    height: 100vh;

    .site-name {
      margin: 80px 0 40px;
    }

    .container {
      padding: 20px 40px;
      width: 300px;
      display: flex;
      flex-direction: column;
      align-items: center;

      .page-title {
        margin: 5px 0 20px;
      }
      :deep(.app-button) {
        margin-top: 10px;
      }
    }
  }
</style>
