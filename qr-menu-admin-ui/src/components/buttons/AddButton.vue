<script setup lang="ts">
  import { AppButton, AppIcon } from '../shared';
</script>

<template>
  <app-button>
    <app-icon name="Plus" :stroke-width="3"></app-icon>
    <slot></slot>
  </app-button>
</template>
