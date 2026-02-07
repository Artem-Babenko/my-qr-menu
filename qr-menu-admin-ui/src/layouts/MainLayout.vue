<script setup lang="ts">
  import { networkApi } from '@/api/networkApi';
  import { AppFlex, type IconName } from '@/components/shared';
  import { AppText } from '@/components/shared';
  import { ROUTES } from '@/router';
  import { useNetworkStore } from '@/store/network';
  import { useUserStore } from '@/store/user';
  import { computed, watchEffect } from 'vue';
  import { useRouter } from 'vue-router';
  import { NavButton } from '@/components/buttons';
  import NavUserInfo from './NavUserInfo.vue';
  import { useRolesStore } from '@/store/roles';
  import { rolesApi } from '@/api/rolesApi';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';

  interface PageButton {
    icon: IconName;
    name: string;
    routeName: string;
    disabled?: boolean;
  }

  const userStore = useUserStore();
  const networkStore = useNetworkStore();
  const rolesStore = useRolesStore();
  const router = useRouter();
  const { hasAnyOf } = usePermissions();

  const canMenu = computed(() =>
    hasAnyOf([
      PermissionType.productsView,
      PermissionType.productsCreate,
      PermissionType.productsEdit,
      PermissionType.productsDelete,
      PermissionType.categoriesView,
      PermissionType.categoriesCreate,
      PermissionType.categoriesEdit,
      PermissionType.categoriesDelete,
    ]),
  );

  const canOrders = computed(() =>
    hasAnyOf([
      PermissionType.ordersView,
      PermissionType.ordersCreate,
      PermissionType.ordersEdit,
      PermissionType.ordersTakeInWork,
      PermissionType.ordersSendToKitchen,
      PermissionType.ordersStartCooking,
      PermissionType.ordersMarkReady,
      PermissionType.ordersReturn,
      PermissionType.ordersComplete,
      PermissionType.ordersCancel,
      PermissionType.ordersDelete,
    ]),
  );

  const canUsers = computed(() =>
    hasAnyOf([
      PermissionType.usersView,
      PermissionType.usersEdit,
      PermissionType.usersDelete,
      PermissionType.invitationsView,
      PermissionType.invitationsCreate,
      PermissionType.invitationsDelete,
      PermissionType.rolesView,
      PermissionType.rolesUpdate,
      PermissionType.rolesCreate,
      PermissionType.rolesDelete,
    ]),
  );

  const canEstablishments = computed(() =>
    hasAnyOf([
      PermissionType.establishmentsCreate,
      PermissionType.establishmentsUpdate,
      PermissionType.establishmentsDelete,
      PermissionType.networkEdit,
      PermissionType.tablesView,
      PermissionType.tablesCreate,
      PermissionType.tablesEdit,
      PermissionType.tablesDelete,
    ]),
  );

  const pageButtons = computed<PageButton[]>(() => [
    { icon: 'LayoutDashboard', name: 'Головна', routeName: ROUTES.dashboard },
    {
      icon: 'UtensilsCrossed',
      name: 'Меню',
      routeName: ROUTES.menuProducts,
      disabled: !canMenu.value,
    },
    {
      icon: 'ShoppingBag',
      name: 'Замовлення',
      routeName: ROUTES.orders,
      disabled: !canOrders.value,
    },
    { icon: 'BarChart3', name: 'Аналітика', routeName: '', disabled: true },
    {
      icon: 'Store',
      name: 'Заклади',
      routeName: ROUTES.establishments,
      disabled: !canEstablishments.value,
    },
    {
      icon: 'Users',
      name: 'Користувачі',
      routeName: ROUTES.users,
      disabled: !canUsers.value,
    },
    { icon: 'Settings', name: 'Налаштування', routeName: '', disabled: true },
  ]);

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

  const networkName = computed(
    () =>
      networkStore.network?.name ||
      networkStore.network?.establishments[0]?.name,
  );

  watchEffect(() => {
    const id = userStore.user?.networkId;
    if (id) loadNetworkData(id);
  });
</script>

<template>
  <div class="main-layout">
    <aside class="nav">
      <app-flex gap="5" class="nav-head">
        <app-text size="l" weight="600">{{ networkName }}</app-text>
        <app-text color="secondary">My Qr Menu</app-text>
      </app-flex>
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
    display: flex;
  }
  .nav {
    width: 250px;
    display: flex;
    flex-direction: column;
    border-right: 1px solid var(--border);
    height: 100vh;
  }
  .nav-head {
    flex-direction: column !important;
    align-items: start !important;
    padding: 20px;
    border-bottom: 1px solid var(--border);
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
