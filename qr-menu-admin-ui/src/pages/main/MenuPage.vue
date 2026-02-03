<script setup lang="ts">
  import { AppTabs, AppText } from '@/components/shared';
  import { MenuPageTab } from '@/consts/tabs';
  import { ROUTES } from '@/router';
  import type { AppTab } from '@/types/components';
  import { computed, watch } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';

  const route = useRoute();
  const router = useRouter();
  const { hasAnyOf } = usePermissions();

  const canViewProducts = computed(() =>
    hasAnyOf([
      PermissionType.productsView,
      PermissionType.productsCreate,
      PermissionType.productsEdit,
      PermissionType.productsDelete,
    ]),
  );

  const canViewCategories = computed(() =>
    hasAnyOf([
      PermissionType.categoriesView,
      PermissionType.categoriesCreate,
      PermissionType.categoriesEdit,
      PermissionType.categoriesDelete,
    ]),
  );

  const canView = computed(
    () => canViewProducts.value || canViewCategories.value,
  );

  const tabs = computed<AppTab[]>(() => {
    const res: AppTab[] = [];
    if (canViewProducts.value)
      res.push({ id: MenuPageTab.products, title: 'Страви' });
    if (canViewCategories.value)
      res.push({ id: MenuPageTab.categories, title: 'Категорії' });
    return res;
  });

  const selectedTab = computed<MenuPageTab>({
    get() {
      const first =
        (tabs.value[0]?.id as MenuPageTab | undefined) ?? MenuPageTab.products;
      if (route.name === ROUTES.menuCategories)
        return canViewCategories.value ? MenuPageTab.categories : first;
      return canViewProducts.value ? MenuPageTab.products : first;
    },
    set(v) {
      router.push({
        name:
          v === MenuPageTab.categories
            ? ROUTES.menuCategories
            : ROUTES.menuProducts,
      });
    },
  });

  watch(
    canView,
    (v) => {
      if (v) return;
      router.replace({ name: ROUTES.dashboard });
    },
    { immediate: true },
  );
</script>

<template>
  <div v-if="canView" class="page">
    <div class="header">
      <app-text size="xxl" weight="600">Меню</app-text>
      <app-text color="secondary">Управління категоріями та стравами</app-text>
    </div>

    <app-tabs v-model:selected="selectedTab" :tabs="tabs"></app-tabs>

    <router-view></router-view>
  </div>
  <div v-else>
    <app-text color="secondary">Недостатньо прав для перегляду меню</app-text>
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
