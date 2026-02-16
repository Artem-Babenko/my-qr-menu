<script setup lang="ts">
  import '@/css/app.scss';
  import '@/css/light.css';
  import '@/css/dark.css';

  import { AppToastContainer } from '@/components/shared';
  import { useToast } from '@/composables/useToast';
  import { useThemeStore } from '@/store/theme';
  import { watch } from 'vue';

  const { toasts, remove } = useToast();
  const themeStore = useThemeStore();

  watch(
    () => themeStore.theme,
    (theme) => {
      const root = document.documentElement;

      root.classList.remove('light', 'dark');
      root.classList.add(theme);

      root.classList.add('theme-transition');
      setTimeout(() => root.classList.remove('theme-transition'), 300);
    },
    { immediate: true },
  );
</script>

<template>
  <router-view></router-view>
  <app-toast-container :toasts="toasts" @close="remove" />
</template>

<style>
  .theme-transition *,
  .theme-transition *::before,
  .theme-transition *::after {
    transition:
      background-color 0.25s ease,
      color 0.25s ease,
      border-color 0.25s ease,
      fill 0.25s ease,
      stroke 0.25s ease;
  }
</style>
