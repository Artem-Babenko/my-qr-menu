<script setup lang="ts">
  import { ordersApi } from '@/api/ordersApi';
  import { CardDropdown } from '@/components/dropdowns';
  import { AppCard, AppText } from '@/components/shared';
  import { getErrorMessage } from '@/consts/errorMessages';
  import { useToastsStore } from '@/store/toasts';
  import { type OrderListItem } from '@/types/orders';
  import { computed } from 'vue';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';
  import { getOrderStatusText, OrderStatus } from '@/consts/orders';
  import type { ApiResponse } from '@/types/api';

  const props = defineProps<{ order: OrderListItem }>();
  const emit = defineEmits<{ open: []; changed: [] }>();

  const toasts = useToastsStore();
  const { has } = usePermissions();

  const minutesSinceCreate = computed(() => {
    const created = new Date(props.order.createdAt).getTime();
    const diff = Math.max(0, Date.now() - created);
    return Math.floor(diff / 60000);
  });

  const lines = computed(() => {
    const items = props.order.items ?? [];
    const firstTwo = items.slice(0, 2).map((i) => `${i.quantity}x ${i.name}`);
    if (items.length <= 2) return firstTwo;
    const rest = items.length - 2;
    return [...firstTwo, `+${rest} Ще`];
  });

  const can = (p: PermissionType) => has(p, props.order.establishmentId);

  const onCancel = async () => {
    const confirmed = window.confirm(
      `Скасувати замовлення №${props.order.orderNumber}?`,
    );
    if (!confirmed) return;

    const resp = await ordersApi.cancel(props.order.id);
    handleChangeStatusResp(resp);
  };

  const onDelete = async () => {
    const confirmed = window.confirm(
      `Видалити замовлення №${props.order.orderNumber}?`,
    );
    if (!confirmed) return;

    const resp = await ordersApi.delete(props.order.id);
    handleChangeStatusResp(resp);
  };

  const onTakeInWork = async () => {
    const resp = await ordersApi.takeInWork(props.order.id);
    handleChangeStatusResp(resp);
  };

  const onSendToKitchen = async () => {
    const resp = await ordersApi.sendToKitchen(props.order.id);
    handleChangeStatusResp(resp);
  };

  const onStartCooking = async () => {
    const resp = await ordersApi.startCooking(props.order.id);
    handleChangeStatusResp(resp);
  };

  const onMarkReady = async () => {
    const resp = await ordersApi.markReady(props.order.id);
    handleChangeStatusResp(resp);
  };

  const onReturn = async () => {
    const resp = await ordersApi.return(props.order.id);
    handleChangeStatusResp(resp);
  };

  const onComplete = async () => {
    const resp = await ordersApi.complete(props.order.id);
    handleChangeStatusResp(resp);
  };

  const handleChangeStatusResp = (resp: ApiResponse<void>) => {
    if (!resp.success) {
      toasts.error(getErrorMessage(resp.errorCode));
      return;
    }
    emit('changed');
  };

  const statusText = computed(() => getOrderStatusText(props.order.status));

  const actions = computed(() => {
    const s = props.order.status;
    const a: any[] = [];

    a.push({
      title: 'Переглянути / редагувати',
      icon: 'Pencil',
      click: () => emit('open'),
      disabled: () => !can(PermissionType.ordersEdit),
    });

    if (s === OrderStatus.new && can(PermissionType.ordersTakeInWork)) {
      a.push({ title: 'Взяти у роботу', icon: 'Hand', click: onTakeInWork });
    }
    if (s === OrderStatus.accepted && can(PermissionType.ordersSendToKitchen)) {
      a.push({
        title: 'Відправити на кухню',
        icon: 'Send',
        click: onSendToKitchen,
      });
    }
    if (s === OrderStatus.inKitchen && can(PermissionType.ordersStartCooking)) {
      a.push({
        title: 'Розпочати готування',
        icon: 'ChefHat',
        click: onStartCooking,
      });
      if (can(PermissionType.ordersReturn)) {
        a.push({ title: 'Повернути', icon: 'Undo2', click: onReturn });
      }
    }
    if (s === OrderStatus.cooking && can(PermissionType.ordersMarkReady)) {
      a.push({ title: 'Приготовлено', icon: 'Check', click: onMarkReady });
      if (can(PermissionType.ordersReturn)) {
        a.push({ title: 'Повернути', icon: 'Undo2', click: onReturn });
      }
    }
    if (s === OrderStatus.ready && can(PermissionType.ordersComplete)) {
      a.push({ title: 'Віднести', icon: 'PackageCheck', click: onComplete });
      if (can(PermissionType.ordersReturn)) {
        a.push({ title: 'Повернути', icon: 'Undo2', click: onReturn });
      }
    }

    if (
      s !== OrderStatus.completed &&
      s !== OrderStatus.cancelled &&
      can(PermissionType.ordersCancel)
    ) {
      a.push({ title: 'Скасувати', icon: 'X', click: onCancel });
    }
    if (s === OrderStatus.cancelled && can(PermissionType.ordersDelete)) {
      a.push({ title: 'Видалити', icon: 'Trash2', click: onDelete });
    }

    return a;
  });
</script>

<template>
  <app-card class="order-card" @click="emit('open')">
    <div class="head" @click.stop>
      <div class="head-left">
        <app-text weight="600">№{{ order.orderNumber }}</app-text>
        <app-text color="secondary">{{ statusText }}</app-text>
      </div>
      <card-dropdown :buttons="actions"></card-dropdown>
    </div>

    <div class="info">
      <app-text v-if="order.tableNumber">
        Стіл №{{ order.tableNumber }}
      </app-text>
      <app-text v-if="order.waiterFullName">
        Офіціант: {{ order.waiterFullName }}
      </app-text>
      <app-text color="secondary">{{ minutesSinceCreate }} хв тому</app-text>
      <div class="items">
        <app-text v-for="(l, idx) in lines" :key="idx" color="secondary">
          {{ l }}
        </app-text>
      </div>
    </div>
  </app-card>
</template>

<style scoped>
  .order-card {
    cursor: pointer;
  }
  .head {
    display: flex;
    justify-content: space-between;
    align-items: start;
  }
  .head-left {
    display: flex;
    flex-direction: column;
    gap: 2px;
  }
  .info {
    display: flex;
    flex-direction: column;
    gap: 6px;
    margin-top: 10px;
  }
  .items {
    display: flex;
    flex-direction: column;
    gap: 3px;
  }
</style>
