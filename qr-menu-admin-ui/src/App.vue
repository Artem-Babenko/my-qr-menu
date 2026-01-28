<script setup lang="ts">
  import '@/css/app.scss';
  import { AppToastContainer } from '@/components/shared';
  import { useToast } from '@/composables';

  const { toasts, remove } = useToast();
</script>

<template>
  <router-view></router-view>
  <app-toast-container :toasts="toasts" @close="remove" />
</template>
