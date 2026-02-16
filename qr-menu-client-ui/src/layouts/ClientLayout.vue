<script setup lang="ts">
  import { computed, watch } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { ROUTES } from '@/router';
  import { AppIcon, AppText, type IconName } from '@/components/shared';
  import { useTableStore } from '@/store/table';
  import { useBasketStore } from '@/store/basket';
  import { clientApi } from '@/api/clientApi';

  const route = useRoute();
  const router = useRouter();
  const tableStore = useTableStore();
  const basketStore = useBasketStore();

  const tableId = computed(() => Number(route.params.tableId));

  watch(
    tableId,
    async (id) => {
      if (!id || isNaN(id)) return;
      basketStore.init(id);
      if (tableStore.table?.tableId === id) return;
      tableStore.loading = true;
      const resp = await clientApi.getTable(id);
      if (resp.success && resp.data) {
        tableStore.setTable(resp.data);
      }
      tableStore.loading = false;
    },
    { immediate: true },
  );

  const tabs = computed<{ name: string; label: string; icon: IconName }[]>(
    () => [
      { name: ROUTES.home, label: 'Головна', icon: 'Home' },
      { name: ROUTES.menu, label: 'Меню', icon: 'UtensilsCrossed' },
      { name: ROUTES.basket, label: 'Кошик', icon: 'ShoppingCart' },
      { name: ROUTES.orders, label: 'Замовлення', icon: 'ClipboardList' },
    ],
  );

  function navigate(routeName: string) {
    router.push({ name: routeName, params: { tableId: tableId.value } });
  }

  function isActive(routeName: string) {
    return route.name === routeName;
  }
</script>

<template>
  <div class="client-layout">
    <main class="content">
      <router-view />
    </main>
    <nav class="bottom-nav">
      <button
        v-for="tab in tabs"
        :key="tab.name"
        class="nav-tab"
        :class="{ active: isActive(tab.name) }"
        @click="navigate(tab.name)"
      >
        <div class="tab-icon-wrapper">
          <app-icon :name="tab.icon" :size="22" />
          <span
            v-if="tab.name === 'basket' && basketStore.totalItems > 0"
            class="badge"
          >
            {{ basketStore.totalItems }}
          </span>
        </div>
        <app-text size="xxs" :weight="isActive(tab.name) ? '600' : '400'">
          {{ tab.label }}
        </app-text>
      </button>
    </nav>
  </div>
</template>

<style scoped>
  .client-layout {
    display: flex;
    flex-direction: column;
    min-height: 100dvh;
  }

  .content {
    flex: 1;
    padding: var(--page-padding);
    padding-bottom: calc(var(--bottom-nav-height) + var(--page-padding));
    overflow-y: auto;
  }

  .bottom-nav {
    position: fixed;
    bottom: 0;
    left: 0;
    right: 0;
    height: var(--bottom-nav-height);
    background: var(--surface-container-lowest);
    border-top: 1px solid var(--outline-variant);
    display: flex;
    align-items: center;
    justify-content: space-around;
    z-index: 100;
    padding-bottom: env(safe-area-inset-bottom, 0);
  }

  .nav-tab {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 2px;
    background: transparent;
    border: none;
    outline: none;
    cursor: pointer;
    padding: 6px 0;
    color: var(--on-surface-variant);
    transition: color 0.2s ease;
    min-width: 60px;
  }

  .nav-tab.active {
    color: var(--on-surface);
  }

  .tab-icon-wrapper {
    position: relative;
    display: flex;
    align-items: center;
    justify-content: center;
  }

  .badge {
    position: absolute;
    top: -6px;
    right: -10px;
    background: var(--primary);
    color: var(--on-primary);
    font: var(--font-xxs);
    font-weight: 600;
    min-width: 16px;
    height: 16px;
    border-radius: 8px;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 0 4px;
  }
</style>
