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
    align-items: center;
    background-color: var(--background);
    min-height: 100dvh;
    padding: 16px;
    box-sizing: border-box;
    overflow-y: auto;

    .site-name {
      margin: 30px 0 24px;
      text-align: center;
    }

    .container {
      padding: 20px 40px;
      width: clamp(300px, 92vw, 420px);
      max-width: 100%;
      display: flex;
      flex-direction: column;
      align-items: center;
      box-sizing: border-box;

      .page-title {
        margin: 5px 0 20px;
        text-align: center;
      }
      :deep(.app-button) {
        margin-top: 10px;
      }
    }
  }

  @media (max-width: 640px) {
    .base-auth-page {
      justify-content: flex-start;
      padding: 12px;

      .site-name {
        margin: 14px 0 16px;
      }

      .container {
        width: 100%;
        padding: 16px 14px;

        :deep(.app-button) {
          width: 100%;
          justify-content: center;
        }

        :deep(.app-input) {
          width: 100%;
        }
      }
    }
  }
</style>
