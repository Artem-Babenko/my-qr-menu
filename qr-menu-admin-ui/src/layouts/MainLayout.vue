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

  interface PageButton {
    icon: IconName;
    name: string;
    routeName: string;
  }

  const pageButtons: PageButton[] = [
    { icon: 'LayoutDashboard', name: 'Головна', routeName: ROUTES.dashboard },
    { icon: 'UtensilsCrossed', name: 'Меню', routeName: '' },
    { icon: 'ShoppingBag', name: 'Замовлення', routeName: '' },
    { icon: 'BarChart3', name: 'Аналітика', routeName: '' },
    { icon: 'Store', name: 'Заклади', routeName: '' },
    { icon: 'Users', name: 'Користувачі', routeName: ROUTES.users },
    { icon: 'Settings', name: 'Налаштування', routeName: '' },
  ];

  const userStore = useUserStore();
  const networkStore = useNetworkStore();
  const router = useRouter();

  const goToPage = (routeName: string) => {
    router.push({ name: routeName });
  };

  const loadNetwork = async (networkId: number) => {
    const resp = await networkApi.getNetwork(networkId);
    networkStore.network = resp.data;
  };

  watchEffect(() => {
    const id = userStore.user?.networkId;
    if (id) loadNetwork(id);
  });
</script>

<template>
  <div v-if="networkStore.network" class="main-layout">
    <aside class="nav">
      <app-text size="l" weight="600" class="site-name">QR Menu</app-text>
      <div class="nav-buttons">
        <nav-button
          v-for="btn in pageButtons"
          :key="btn.name"
          :label="btn.name"
          :icon="btn.icon"
          :selected="router.currentRoute.value.name === btn.routeName"
          @click="goToPage(btn.routeName)"
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
