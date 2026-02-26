<script setup lang="ts">
  import { ordersApi } from '@/api/ordersApi';
  import {
    AppIcon,
    AppCard,
    AppFlex,
    AppSearchInput,
    AppText,
    type IconName,
  } from '@/components/shared';
  import { getErrorMessage } from '@/consts/errorMessages';
  import { useNetworkStore } from '@/store/network';
  import { useToastsStore } from '@/store/toasts';
  import type { OrderStatusGroup } from '@/types/orders';
  import { computed, onMounted, ref, toRef, watch } from 'vue';
  import { OrderList } from '@/components/lists';
  import { OrderCreateModal, OrderModal } from '@/components/modals';
  import { useLoader, usePermissions } from '@/composables';
  import { OrderStatus } from '@/consts/orders';
  import { PermissionType } from '@/consts/roles';
  import { ROUTES } from '@/router';
  import { useRoute, useRouter } from 'vue-router';
  import { PageHeader } from '@/components/headers';
  import { AddButton } from '@/components/buttons';

  interface OrderStatusGroupView {
    id: OrderStatusGroup;
    title: string;
    icon: IconName;
  }

  const networkStore = useNetworkStore();
  const toasts = useToastsStore();
  const router = useRouter();
  const route = useRoute();
  const { hasAnyOf } = usePermissions();

  const search = ref('');
  const selectedGroup = ref<OrderStatusGroup | null>(null);
  const createShowed = ref(false);
  const detailsShowed = ref(false);
  const selectedOrderId = ref<number | null>(null);
  const loading = ref(false);
  const networkId = toRef(() => networkStore.network?.id);

  const canViewOrders = computed(() =>
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

  const groups: OrderStatusGroupView[] = [
    { id: 'new', title: 'Нові', icon: 'Clock' },
    { id: 'inWork', title: 'У роботі', icon: 'ClipboardList' },
    { id: 'completed', title: 'Завершені', icon: 'CheckCircle' },
    { id: 'cancelled', title: 'Скасовані', icon: 'XCircle' },
  ];

  const groupStatuses: Record<OrderStatusGroup, OrderStatus[]> = {
    new: [OrderStatus.new],
    inWork: [
      OrderStatus.accepted,
      OrderStatus.inKitchen,
      OrderStatus.cooking,
      OrderStatus.ready,
    ],
    completed: [OrderStatus.completed],
    cancelled: [OrderStatus.cancelled],
  };

  const toggleGroup = (g: OrderStatusGroup) => {
    selectedGroup.value = selectedGroup.value === g ? null : g;
  };

  const load = async () => {
    const networkId = networkStore.network?.id;
    if (!networkId) return [];

    loading.value = true;
    try {
      const resp = await ordersApi.byNetwork(networkId);
      if (!resp.success) {
        toasts.error(getErrorMessage(resp.errorCode));
        return [];
      }
      return resp.data ?? [];
    } finally {
      loading.value = false;
    }
  };

  const { data: orders, refetch } = useLoader({
    keys: ['orders', networkId],
    fn: load,
    enabled: () => !!networkId.value && canViewOrders.value,
  });

  const openCreate = () => {
    if (!canViewOrders.value) return;
    createShowed.value = true;
  };

  const openCreateFromQuery = () => {
    if (!canViewOrders.value) return;
    if (route.query.openCreateModal !== '1') return;
    createShowed.value = true;
    router.replace({ query: {} });
  };

  const openDetails = (orderId: number) => {
    if (!canViewOrders.value) return;
    selectedOrderId.value = orderId;
    detailsShowed.value = true;
  };

  watch(
    canViewOrders,
    (v) => {
      if (v) return;
      router.replace({ name: ROUTES.dashboard });
    },
    { immediate: true },
  );

  onMounted(() => {
    openCreateFromQuery();
  });

  const filtered = computed(() => {
    if (!orders.value) return [];

    const q = search.value.trim().toLowerCase();
    const group = selectedGroup.value;
    const statuses = group ? new Set(groupStatuses[group]) : null;

    return orders.value.filter((o) => {
      if (statuses && !statuses.has(o.status)) return false;
      if (!q) return true;

      const waiter = o.waiterFullName?.toLowerCase() ?? '';
      const table = o.tableNumber?.toLowerCase() ?? '';
      const number = String(o.orderNumber);

      return (
        number.includes(q) ||
        waiter.includes(q) ||
        table.includes(q) ||
        o.items.some((it) => it.name.toLowerCase().includes(q))
      );
    });
  });

  const counts = computed(() => {
    const res: Record<OrderStatusGroup, number> = {
      new: 0,
      inWork: 0,
      completed: 0,
      cancelled: 0,
    };

    for (const ord of orders.value ?? []) {
      for (const [group, status] of Object.entries(groupStatuses)) {
        if (status.includes(ord.status)) res[group as OrderStatusGroup] += 1;
      }
    }

    return res;
  });
</script>

<template>
  <div v-if="canViewOrders" class="page">
    <page-header section-name="Замовлення"></page-header>

    <div class="status-groups">
      <app-card
        v-for="g in groups"
        :key="g.id"
        :type="selectedGroup === g.id ? 'outlined' : 'default'"
        @click="toggleGroup(g.id)"
        class="status-card"
      >
        <div class="left">
          <app-icon :name="g.icon" :size="20"></app-icon>
        </div>
        <div class="right">
          <app-text size="xs" color="secondary">{{ g.title }}</app-text>
          <app-text size="l" weight="600">{{ counts[g.id] }}</app-text>
        </div>
      </app-card>
    </div>

    <app-flex class="controls" align="center" gap="20">
      <div class="search">
        <app-search-input
          v-model="search"
          placeholder="Пошук замовлень..."
        ></app-search-input>
      </div>
      <add-button @click="openCreate">Створити замовлення</add-button>
    </app-flex>

    <order-list
      :orders="filtered"
      :loading="loading"
      @open="openDetails"
      @changed="refetch"
    ></order-list>

    <order-create-modal
      v-model:showed="createShowed"
      @saved="refetch"
    ></order-create-modal>

    <order-modal
      v-model:showed="detailsShowed"
      :order-id="selectedOrderId"
      @saved="refetch"
    ></order-modal>
  </div>
  <div v-else>
    <app-text color="secondary">
      Недостатньо прав для перегляду замовлень
    </app-text>
  </div>
</template>

<style scoped>
  .page {
    display: flex;
    flex-direction: column;
    gap: 20px;
  }

  .status-groups {
    display: grid;
    grid-template-columns: repeat(4, minmax(0, 1fr));
    gap: 20px;
  }
  .status-card {
    padding: 15px;
    display: flex;
    gap: 12px;
    cursor: pointer;
    user-select: none;
    transition: all 0.2s ease;
  }
  .left {
    width: 34px;
    height: 34px;
    border-radius: 10px;
    background-color: var(--secondary-container);
    color: var(--on-secondary-container);
    display: flex;
    align-items: center;
    justify-content: center;
  }
  .right {
    display: flex;
    flex-direction: column;
    gap: 4px;
  }

  .controls {
    width: 100%;
    flex-wrap: wrap;
  }
  .search {
    flex: 1;
  }
  :deep(.search .app-search-input) {
    width: 100%;
  }

  @media (max-width: 1100px) {
    .status-groups {
      grid-template-columns: repeat(2, minmax(0, 1fr));
    }
  }

  @media (max-width: 768px) {
    .page {
      gap: 12px;
    }
    .status-groups {
      grid-template-columns: repeat(2, minmax(0, 1fr));
      gap: 12px;
    }
    .controls {
      gap: 12px;
    }
    .search {
      width: 100%;
      flex: 1 0 100%;
    }
    .controls :deep(.app-button) {
      width: 100%;
      justify-content: center;
    }
  }
</style>
