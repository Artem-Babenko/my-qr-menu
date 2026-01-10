<script lang="ts" setup>
  import { AppButton, AppIcon } from '@/components/shared';
</script>

<template>
  <app-button class="dots-button" type="text">
    <app-icon name="EllipsisVertical" :size="15"></app-icon>
  </app-button>
</template>

<style scoped>
  .dots-button {
    padding: 6px 5px 3px;
  }
</style>
