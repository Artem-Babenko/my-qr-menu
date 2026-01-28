<script setup lang="ts">
  import { AppFlex, AppText } from '@/components/shared';

  defineProps<{ pageTitle: string }>();
</script>

<template>
  <app-flex class="base-auth-page" direction="column">
    <app-text weight="600" size="xl" class="site-name">My QR-Menu</app-text>
    <app-flex class="container" direction="column">
      <app-text class="page-title" weight="600" size="l">
        {{ pageTitle }}
      </app-text>
      <slot></slot>
    </app-flex>
  </app-flex>
</template>

<style scoped lang="scss">
  .base-auth-page {
    justify-content: center;

    .site-name {
      margin: 80px 0 40px;
    }

    .container {
      border: 1px solid var(--border);
      border-radius: 10px;
      padding: 20px;
      width: 300px;

      .page-title {
        margin-bottom: 20px;
      }
      :deep(.app-button) {
        margin-top: 10px;
      }
    }
  }
</style>
