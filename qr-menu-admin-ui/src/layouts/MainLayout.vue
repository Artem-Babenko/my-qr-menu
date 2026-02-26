<script setup lang="ts">
  import { AppFlex, AppIcon, type IconName } from '@/components/shared';
  import { AppText } from '@/components/shared';
  import { ROUTES } from '@/router';
  import {
    computed,
    onBeforeUnmount,
    onMounted,
    provide,
    ref,
    watch,
  } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { NavButton } from '@/components/buttons';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';
  import { useThemeStore } from '@/store/theme';
  import { useAuthStore } from '@/store/auth';

  interface PageButton {
    icon: IconName;
    name: string;
    routeName: string;
    disabled?: boolean;
  }

  const MOBILE_BREAKPOINT = 768;
  const TABLET_BREAKPOINT = 1200;

  const route = useRoute();
  const router = useRouter();
  const { hasAnyOf } = usePermissions();
  const themeStore = useThemeStore();
  const authStore = useAuthStore();

  const initialViewportWidth =
    typeof window === 'undefined' ? TABLET_BREAKPOINT : window.innerWidth;

  const viewportWidth = ref(initialViewportWidth);
  const isNavOpen = ref(initialViewportWidth >= MOBILE_BREAKPOINT);

  const isPhone = computed(() => viewportWidth.value < MOBILE_BREAKPOINT);
  const isTablet = computed(
    () =>
      viewportWidth.value >= MOBILE_BREAKPOINT &&
      viewportWidth.value < TABLET_BREAKPOINT,
  );
  const isDesktop = computed(() => viewportWidth.value >= TABLET_BREAKPOINT);
  const canToggleNav = computed(() => !isDesktop.value);
  const isDarkTheme = computed(() => themeStore.theme === 'dark');

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

  const getTopLevelSegment = (path: string): string => {
    return path.split('/').filter(Boolean)[0] ?? '';
  };

  const isPageButtonSelected = (routeName: string): boolean => {
    if (!routeName) return false;
    const currentTopLevel = getTopLevelSegment(route.path);
    const targetTopLevel = getTopLevelSegment(
      router.resolve({ name: routeName }).path,
    );
    if (!currentTopLevel || !targetTopLevel) return false;
    return currentTopLevel === targetTopLevel;
  };

  const goToPage = (routeName: string) => {
    router.push({ name: routeName });
    if (isPhone.value) isNavOpen.value = false;
  };

  const syncViewport = () => {
    viewportWidth.value = window.innerWidth;
    if (isDesktop.value) {
      isNavOpen.value = true;
      return;
    }
    if (isPhone.value) isNavOpen.value = false;
  };

  const toggleNav = () => {
    if (!canToggleNav.value) return;
    isNavOpen.value = !isNavOpen.value;
  };

  const closeNav = () => {
    isNavOpen.value = false;
  };

  const toggleTheme = () => {
    themeStore.theme = isDarkTheme.value ? 'light' : 'dark';
  };

  const logout = async () => {
    authStore.setToken(null);
    await router.replace({ name: ROUTES.login });
  };

  provide('layoutNavControl', {
    canToggleNav,
    isNavOpen,
    toggleNav,
  });

  watch(
    () => route.fullPath,
    () => {
      if (isPhone.value) closeNav();
    },
  );

  onMounted(() => {
    syncViewport();
    window.addEventListener('resize', syncViewport);
  });

  onBeforeUnmount(() => {
    window.removeEventListener('resize', syncViewport);
  });
</script>

<template>
  <div
    class="main-layout"
    :class="{ phone: isPhone, tablet: isTablet, desktop: isDesktop }"
  >
    <div
      v-if="isPhone && isNavOpen"
      class="nav-backdrop"
      @click="closeNav"
    ></div>
    <aside class="nav" :class="{ open: isDesktop || isNavOpen }">
      <app-flex class="nav-head" gap="10">
        <app-icon
          name="ScanQrCode"
          :size="25"
          color="var(--primary)"
        ></app-icon>
        <app-text size="l" weight="600">My QR Menu</app-text>
      </app-flex>
      <div class="nav-buttons">
        <nav-button
          v-for="btn in pageButtons"
          :key="btn.name"
          :label="btn.name"
          :icon="btn.icon"
          :disabled="!!btn.disabled"
          :selected="isPageButtonSelected(btn.routeName)"
          @click="!btn.disabled && goToPage(btn.routeName)"
        ></nav-button>
      </div>
      <div v-if="isPhone" class="nav-footer">
        <nav-button
          :label="isDarkTheme ? 'Світла тема' : 'Темна тема'"
          :icon="isDarkTheme ? 'Sun' : 'Moon'"
          :selected="false"
          :disabled="false"
          @click="toggleTheme"
        ></nav-button>
        <nav-button
          label="Вийти"
          icon="LogOut"
          :selected="false"
          :disabled="false"
          @click="logout"
        ></nav-button>
      </div>
    </aside>
    <main class="page-content">
      <router-view></router-view>
    </main>
  </div>
</template>

<style scoped>
  .main-layout {
    min-height: 100dvh;
    display: flex;
    position: relative;
  }
  .nav {
    width: 225px;
    min-width: 0;
    flex-shrink: 0;
    display: flex;
    flex-direction: column;
    height: 100dvh;
    background-color: var(--surface-container-low);
    z-index: 20;
    transition:
      width 0.2s ease,
      transform 0.2s ease;
    overflow: hidden;
  }
  .nav-head {
    padding: 27px 20px 15px;
    flex-wrap: nowrap;
    white-space: nowrap;
    min-width: max-content;
  }
  .nav-head :deep(.app-icon) {
    flex-shrink: 0;
  }
  .nav-head :deep(.app-text) {
    white-space: nowrap;
    word-break: keep-all;
  }
  .nav-buttons {
    display: flex;
    flex-direction: column;
    gap: 10px;
    padding: 20px 10px;
    flex: 1;
    min-height: 0;
    overflow-y: auto;
  }
  .nav-footer {
    margin-top: auto;
    border-top: 1px solid var(--outline-variant);
    padding: 10px;
    display: flex;
    flex-direction: column;
    gap: 8px;
  }
  .page-content {
    width: 100%;
    min-width: 0;
    padding: 15px 15px 10px 25px;
    background-color: var(--background);
    transition: padding 0.2s ease;
  }
  .nav-backdrop {
    position: fixed;
    inset: 0;
    background-color: rgba(0, 0, 0, 0.35);
    z-index: 15;
  }

  .main-layout.tablet .nav:not(.open) {
    width: 0;
  }
  .main-layout.tablet .page-content {
    padding-left: 20px;
  }
  .main-layout.tablet .nav:not(.open) + .page-content {
    padding-left: 15px;
  }

  .main-layout.phone .nav {
    position: fixed;
    top: 0;
    left: 0;
    bottom: 0;
    width: min(82vw, 280px);
    min-width: min(82vw, 280px);
    transform: translateX(-100%);
    box-shadow: 0 6px 30px rgba(0, 0, 0, 0.25);
  }
  .main-layout.phone .nav.open {
    transform: translateX(0);
  }
  .main-layout.phone .page-content {
    padding: 12px;
  }
</style>
