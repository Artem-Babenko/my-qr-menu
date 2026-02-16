<script setup lang="ts">
  import { ordersApi } from '@/api/ordersApi';
  import { AppButton, AppFlex, AppModal, AppText } from '@/components/shared';
  import { getErrorMessage } from '@/consts/errorMessages';
  import { useToastsStore } from '@/store/toasts';
  import type { OrderDetails } from '@/types/orders';
  import { computed, ref, watch } from 'vue';
  import OrderItemsEditor, {
    type EditableOrderItem,
  } from '@/components/other/OrderItemsEditor.vue';
  import { getOrderStatusText } from '@/consts/orders';
  import { formatDate } from '@/utils/dates';

  const showed = defineModel<boolean>('showed', { required: true });
  const props = defineProps<{ orderId: number | null }>();
  const emit = defineEmits<{ saved: [] }>();

  const toasts = useToastsStore();

  const loading = ref(false);
  const saving = ref(false);
  const data = ref<OrderDetails | null>(null);
  const items = ref<EditableOrderItem[]>([]);

  const load = async () => {
    if (!props.orderId) return;
    loading.value = true;
    try {
      const resp = await ordersApi.byId(props.orderId);
      if (!resp.success) {
        toasts.error(getErrorMessage(resp.errorCode));
        data.value = null;
        return;
      }
      data.value = resp.data;
      items.value =
        resp.data?.items
          ?.filter((x) => !!x.productId)
          .map((x) => ({
            productId: x.productId!,
            quantity: x.quantity,
            name: x.productName,
            categoryName: x.categoryName,
            price: x.price,
          })) ?? [];
    } finally {
      loading.value = false;
    }
  };

  watch(
    () => showed.value,
    async (v) => {
      if (!v) return;
      await load();
    },
  );

  const close = () => (showed.value = false);

  const title = computed(() =>
    data.value ? `Замовлення №${data.value.orderNumber}` : 'Замовлення',
  );

  const saveDisabled = computed(() => {
    if (!data.value) return true;
    if (!items.value.length) return true;
    return saving.value;
  });

  const save = async () => {
    if (!props.orderId || !data.value) return;
    if (!items.value.length) return;

    saving.value = true;
    try {
      const resp = await ordersApi.update(props.orderId, {
        tableId: data.value.tableId,
        customerFullName: data.value.customerFullName,
        comment: data.value.comment,
        items: items.value.map((x) => ({
          productId: x.productId,
          quantity: x.quantity,
        })),
      });

      if (!resp.success) {
        toasts.error(getErrorMessage(resp.errorCode));
        return;
      }

      emit('saved');
      await load();
    } finally {
      saving.value = false;
    }
  };

  const statusName = computed(() =>
    data.value ? getOrderStatusText(data.value.status) : null,
  );
</script>

<template>
  <app-modal v-model:showed="showed" :title="title" :width="760">
    <app-text v-if="loading" color="secondary">Завантаження...</app-text>
    <div v-else-if="data" class="content">
      <div class="grid">
        <div>
          <app-text color="secondary">Заклад</app-text>
          <app-text>{{ data.establishmentName }}</app-text>
        </div>
        <div>
          <app-text color="secondary">Стіл</app-text>
          <app-text>{{
            data.tableNumber ? `№${data.tableNumber}` : '—'
          }}</app-text>
        </div>
        <div>
          <app-text color="secondary">Статус</app-text>
          <app-text>{{ statusName }}</app-text>
        </div>
        <div>
          <app-text color="secondary">Створено</app-text>
          <app-text>{{ formatDate(data.createdAt) }}</app-text>
        </div>
      </div>

      <div class="block">
        <app-text weight="600">Товари</app-text>
        <order-items-editor
          v-model="items"
          :establishment-id="data.establishmentId"
          :disabled="saving"
        ></order-items-editor>
      </div>

      <div class="block">
        <app-text weight="600">Історія статусів</app-text>
        <div class="history">
          <div v-for="h in data.statusHistory" :key="h.id" class="history-row">
            <app-text color="secondary">
              {{ formatDate(h.changedAt) }} — {{ h.changedByUserFullName }}
            </app-text>
            <app-text>
              {{ getOrderStatusText(h.fromStatus) }} →
              {{ getOrderStatusText(h.toStatus) }}
            </app-text>
          </div>
        </div>
      </div>
    </div>

    <app-flex justify="flex-end" gap="10" class="buttons">
      <app-button type="outline" @click="close">Закрити</app-button>
      <app-button :disabled="saveDisabled" @click="save">
        {{ saving ? 'Збереження...' : 'Зберегти' }}
      </app-button>
    </app-flex>
  </app-modal>
</template>

<style scoped>
  .content {
    display: flex;
    flex-direction: column;
    gap: 16px;
  }
  .grid {
    display: grid;
    grid-template-columns: repeat(2, minmax(0, 1fr));
    gap: 12px;
  }
  .block {
    border-top: 1px solid var(--outline-variant);
    padding-top: 12px;
    display: flex;
    flex-direction: column;
    gap: 10px;
  }
  .history {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }
  .history-row {
    display: flex;
    justify-content: space-between;
    gap: 10px;
  }
  .buttons {
    margin-top: 16px;
  }
</style>
