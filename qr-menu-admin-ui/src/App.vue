<script setup lang="ts">
  import '@/css/app.css';
  import '@/css/light.css';
  import '@/css/dark.css';

  import { AppToastContainer } from '@/components/shared';
  import { useToast } from '@/composables';
  import { useThemeStore } from './store/theme';
  import { watch } from 'vue';

  const { toasts, remove } = useToast();
  const themeStore = useThemeStore();

  const ensureThemeMeta = (): HTMLMetaElement => {
    const existing = document.querySelector(
      'meta[name="theme-color"]',
    ) as HTMLMetaElement | null;
    if (existing) return existing;

    const created = document.createElement('meta');
    created.name = 'theme-color';
    document.head.appendChild(created);
    return created;
  };

  const syncBrowserThemeColor = (theme: 'light' | 'dark') => {
    const meta = ensureThemeMeta();
    meta.content = theme === 'dark' ? '#111318' : '#f9f9ff';
    document.documentElement.style.colorScheme = theme;
  };

  watch(
    () => themeStore.theme,
    (theme) => {
      const root = document.documentElement;

      root.classList.remove('light', 'dark');
      root.classList.add(theme);
      syncBrowserThemeColor(theme);

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
