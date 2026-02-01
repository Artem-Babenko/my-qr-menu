<script setup lang="ts">
  import { AppTabs, AppText } from '@/components/shared';
  import { MenuPageTab } from '@/consts/tabs';
  import { ROUTES } from '@/router';
  import type { AppTab } from '@/types/components';
  import { computed } from 'vue';
  import { useRoute, useRouter } from 'vue-router';

  const route = useRoute();
  const router = useRouter();

  const tabs: AppTab[] = [
    { id: MenuPageTab.products, title: 'Страви' },
    { id: MenuPageTab.categories, title: 'Категорії' },
  ];

  const selectedTab = computed<MenuPageTab>({
    get() {
      return route.name === ROUTES.menuCategories
        ? MenuPageTab.categories
        : MenuPageTab.products;
    },
    set(v) {
      router.push({
        name: v === MenuPageTab.categories ? ROUTES.menuCategories : ROUTES.menuProducts,
      });
    },
  });
</script>

<template>
  <div class="page">
    <div class="header">
      <app-text size="xxl" weight="600">Меню</app-text>
      <app-text color="secondary">Управління категоріями та стравами</app-text>
    </div>

    <app-tabs v-model:selected="selectedTab" :tabs="tabs"></app-tabs>

    <router-view></router-view>
  </div>
</template>

<style scoped>
  .page {
    display: flex;
    flex-direction: column;
    gap: 20px;
  }
  .header {
    display: flex;
    flex-direction: column;
    gap: 10px;
  }
</style>

