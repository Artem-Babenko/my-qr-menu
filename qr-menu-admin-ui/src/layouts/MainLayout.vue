<script setup lang="ts">
  import { networkApi } from '@/api/networkApi';
  import type { IconName } from '@/components/shared';
  import { AppText } from '@/components/shared';
  import { ROUTES } from '@/router';
  import { useNetworkStore } from '@/store/network';
  import { useUserStore } from '@/store/user';
  import { watchEffect } from 'vue';
  import { useRouter } from 'vue-router';
  import { NavButton } from '@/components/buttons';
  import NavUserInfo from './NavUserInfo.vue';
  import { useRolesStore } from '@/store/roles';
  import { rolesApi } from '@/api/rolesApi';

  interface PageButton {
    icon: IconName;
    name: string;
    routeName: string;
    disabled?: boolean;
  }

  const pageButtons: PageButton[] = [
    { icon: 'LayoutDashboard', name: 'Головна', routeName: ROUTES.dashboard },
    { icon: 'UtensilsCrossed', name: 'Меню', routeName: ROUTES.menuProducts },
    { icon: 'ShoppingBag', name: 'Замовлення', routeName: ROUTES.orders },
    { icon: 'BarChart3', name: 'Аналітика', routeName: '', disabled: true },
    { icon: 'Store', name: 'Заклади', routeName: ROUTES.establishments },
    { icon: 'Users', name: 'Користувачі', routeName: ROUTES.users },
    { icon: 'Settings', name: 'Налаштування', routeName: '', disabled: true },
  ];

  const userStore = useUserStore();
  const networkStore = useNetworkStore();
  const rolesStore = useRolesStore();
  const router = useRouter();

  const goToPage = (routeName: string) => {
    router.push({ name: routeName });
  };

  const loadNetworkData = async (networkId: number) => {
    const promises = await Promise.all([
      networkApi.getNetwork(networkId),
      rolesApi.all(networkId),
    ]);
    networkStore.network = promises[0].data;
    rolesStore.roles = promises[1].data;
  };

  watchEffect(() => {
    const id = userStore.user?.networkId;
    if (id) loadNetworkData(id);
  });
</script>

<template>
  <div class="main-layout">
    <aside class="nav">
      <app-text size="l" weight="600" class="site-name">QR Menu</app-text>
      <div class="nav-buttons">
        <nav-button
          v-for="btn in pageButtons"
          :key="btn.name"
          :label="btn.name"
          :icon="btn.icon"
          :disabled="!!btn.disabled"
          :selected="router.currentRoute.value.name === btn.routeName"
          @click="!btn.disabled && goToPage(btn.routeName)"
        ></nav-button>
      </div>
      <nav-user-info></nav-user-info>
    </aside>
    <main class="page-content">
      <router-view></router-view>
    </main>
  </div>
</template>

<style scoped>
  .main-layout {
    height: 100%;
    overflow: hidden;
    display: flex;
  }
  .site-name {
    font-size: 22px;
    padding: 20px 25px;
    border-bottom: 1px solid var(--border);
  }
  .nav {
    width: 250px;
    display: flex;
    flex-direction: column;
    border-right: 1px solid var(--border);
    height: 100vh;
  }
  .nav-user-info {
    margin-top: auto;
  }
  .nav-buttons {
    display: flex;
    flex-direction: column;
    gap: 5px;
    padding: 20px 10px;
  }
  .page-content {
    width: 100%;
    padding: 30px 25px;
  }
</style>
