<script setup lang="ts">
  import { ordersApi } from '@/api/ordersApi';
  import {
    AppButton,
    AppIcon,
    AppSearchInput,
    AppText,
    type IconName,
  } from '@/components/shared';
  import { getErrorMessage } from '@/consts/errorMessages';
  import { useNetworkStore } from '@/store/network';
  import { useToastsStore } from '@/store/toasts';
  import type { OrderStatusGroup } from '@/types/orders';
  import { computed, ref, toRef, watch } from 'vue';
  import { OrderList } from '@/components/lists';
  import { OrderCreateModal, OrderModal } from '@/components/modals';
  import { useLoader, usePermissions } from '@/composables';
  import { OrderStatus } from '@/consts/orders';
  import { PermissionType } from '@/consts/roles';
  import { ROUTES } from '@/router';
  import { useRouter } from 'vue-router';

  interface OrderStatusGroupView {
    id: OrderStatusGroup;
    title: string;
    icon: IconName;
  }

  const networkStore = useNetworkStore();
  const toasts = useToastsStore();
  const router = useRouter();
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
    <div class="header">
      <app-text size="xxl" weight="600">Замовлення</app-text>
      <app-text color="secondary">Перегляд та управління замовленнями</app-text>
    </div>

    <div class="status-groups">
      <div
        v-for="g in groups"
        :key="g.id"
        class="status-card"
        :class="{ selected: selectedGroup === g.id }"
        @click="toggleGroup(g.id)"
      >
        <div class="left">
          <app-icon :name="g.icon" :size="18"></app-icon>
        </div>
        <div class="right">
          <app-text color="secondary">{{ g.title }}</app-text>
          <app-text size="xl" weight="600">{{ counts[g.id] }}</app-text>
        </div>
      </div>
    </div>

    <div class="toolbar">
      <app-search-input
        v-model="search"
        placeholder="Пошук замовлень..."
      ></app-search-input>
      <app-button @click="openCreate">
        <app-icon name="Plus" :size="15"></app-icon>
        Створити замовлення
      </app-button>
    </div>

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

  .header {
    display: flex;
    flex-direction: column;
    gap: 10px;
  }

  .status-groups {
    display: grid;
    grid-template-columns: repeat(4, minmax(0, 1fr));
    gap: 20px;
  }

  .status-card {
    border: 1px solid var(--border);
    border-radius: 12px;
    background: var(--background);
    padding: 16px;
    display: flex;
    gap: 12px;
    cursor: pointer;
    user-select: none;
    transition:
      border-color 120ms ease,
      box-shadow 120ms ease;
  }

  .status-card.selected {
    border-color: var(--primary);
    box-shadow: 0 0 0 3px color-mix(in srgb, var(--primary) 20%, transparent);
  }

  .left {
    width: 34px;
    height: 34px;
    border-radius: 10px;
    border: 1px solid var(--border);
    display: flex;
    align-items: center;
    justify-content: center;
  }

  .right {
    display: flex;
    flex-direction: column;
    gap: 4px;
  }

  .toolbar {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 20px;
  }
</style>
