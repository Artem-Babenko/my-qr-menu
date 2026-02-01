<script setup lang="ts">
  import { ordersApi } from '@/api/ordersApi';
  import { tablesApi } from '@/api/tablesApi';
  import { usersApi } from '@/api/usersApi';
  import {
    AppButton,
    AppFlex,
    AppInput,
    AppLabel,
    AppModal,
    AppSelect,
  } from '@/components/shared';
  import { getErrorMessage } from '@/consts/errorMessages';
  import { useNetworkStore } from '@/store/network';
  import { useToastsStore } from '@/store/toasts';
  import type { Establishment } from '@/types/network';
  import type { TableView } from '@/types/tables';
  import type { CreateOrderRequest } from '@/types/orders';
  import type { User } from '@/types/user';
  import { computed, ref, watch } from 'vue';
  import OrderItemsEditor, {
    type EditableOrderItem,
  } from '@/components/other/OrderItemsEditor.vue';

  const showed = defineModel<boolean>('showed', { required: true });
  const emit = defineEmits<{ saved: [] }>();

  const networkStore = useNetworkStore();
  const toasts = useToastsStore();

  const establishments = computed<Establishment[]>(
    () => networkStore.network?.establishments ?? [],
  );

  type SelectOption = { id: number; title: string };

  const establishment = ref<SelectOption | null>(null);
  const table = ref<SelectOption | null>(null);
  const waiter = ref<SelectOption | null>(null);
  const customerFullName = ref('');
  const comment = ref('');
  const items = ref<EditableOrderItem[]>([]);
  const tables = ref<TableView[]>([]);
  const users = ref<User[]>([]);
  const tablesLoading = ref(false);
  const usersLoading = ref(false);

  const loadTables = async (estId: number) => {
    tablesLoading.value = true;
    try {
      const resp = await tablesApi.byEstablishment(estId);
      if (!resp.success) {
        toasts.error(getErrorMessage(resp.errorCode));
        tables.value = [];
        return;
      }
      tables.value = resp.data ?? [];
    } finally {
      tablesLoading.value = false;
    }
  };

  const loadUsers = async () => {
    const networkId = networkStore.network?.id;
    if (!networkId) return;
    usersLoading.value = true;
    try {
      const resp = await usersApi.byNetwork(networkId);
      if (!resp.success) {
        toasts.error(getErrorMessage(resp.errorCode));
        users.value = [];
        return;
      }
      users.value = resp.data ?? [];
    } finally {
      usersLoading.value = false;
    }
  };

  const waiterOptions = computed(() => {
    const estId = establishment.value?.id;
    if (!estId) return [];
    return (users.value ?? [])
      .filter((u) => u.accesses?.some((a) => a.establishmentId === estId))
      .map((u) => ({ id: u.id, title: `${u.name} ${u.surname}` }));
  });

  const tableOptions = computed(() => {
    return (tables.value ?? []).map((t) => ({
      id: t.id,
      title: `№${t.number}`,
    }));
  });

  const saveDisabled = computed(() => {
    if (!establishment.value) return true;
    if (!items.value.length) return true;
    return false;
  });

  const reset = () => {
    establishment.value = null;
    table.value = null;
    waiter.value = null;
    customerFullName.value = '';
    comment.value = '';
    items.value = [];
    tables.value = [];
    users.value = [];
    tablesLoading.value = false;
    usersLoading.value = false;
  };

  watch(showed, async (v) => {
    if (!v) return;
    reset();
    await loadUsers();
  });

  watch(establishment, async (newValue) => {
    table.value = null;
    waiter.value = null;
    items.value = [];
    tables.value = [];
    if (!newValue) return;
    await loadTables(newValue.id);
  });

  const save = async () => {
    if (saveDisabled.value) return;
    const payload: CreateOrderRequest = {
      establishmentId: establishment.value!.id,
      tableId: table.value?.id ?? null,
      waiterId: waiter.value?.id ?? null,
      customerFullName: customerFullName.value.trim() || null,
      comment: comment.value.trim() || null,
      items: items.value.map((x) => ({
        productId: x.productId,
        quantity: x.quantity,
      })),
    };

    const resp = await ordersApi.create(payload);
    if (!resp.success) {
      toasts.error(getErrorMessage(resp.errorCode));
      return;
    }

    showed.value = false;
    emit('saved');
  };

  const close = () => (showed.value = false);
</script>

<template>
  <app-modal v-model:showed="showed" title="Створити замовлення" :width="600">
    <div class="form">
      <div class="row">
        <app-label label="Заклад" for="est"></app-label>
        <app-select
          id="est"
          v-model="establishment"
          :options="establishments.map((e) => ({ id: e.id, title: e.name }))"
          option-label="title"
          placeholder="Оберіть заклад"
        ></app-select>
      </div>

      <div v-if="establishment" class="row">
        <app-label label="Стіл (опційно)" for="table"></app-label>
        <app-select
          id="table"
          v-model="table"
          :options="tableOptions"
          option-label="title"
          placeholder="Оберіть стіл"
        ></app-select>
      </div>

      <div v-if="establishment" class="row">
        <app-label label="Виконавець (опційно)" for="waiter"></app-label>
        <app-select
          id="waiter"
          v-model="waiter"
          :options="waiterOptions"
          option-label="title"
          placeholder="Оберіть офіціанта"
        ></app-select>
      </div>

      <div class="row">
        <app-label label="Клієнт (опційно)" for="customer"></app-label>
        <app-input
          id="customer"
          v-model="customerFullName"
          placeholder="ПІБ клієнта"
        ></app-input>
      </div>

      <div class="row">
        <app-label label="Коментар (опційно)" for="comment"></app-label>
        <app-input
          id="comment"
          v-model="comment"
          placeholder="Коментар до замовлення"
        ></app-input>
      </div>

      <order-items-editor
        v-model="items"
        :establishment-id="establishment?.id ?? null"
      ></order-items-editor>
    </div>

    <app-flex justify="flex-end" gap="10" class="buttons">
      <app-button type="outline" @click="close">Скасувати</app-button>
      <app-button :disabled="saveDisabled" @click="save">Створити</app-button>
    </app-flex>
  </app-modal>
</template>

<style scoped>
  .form {
    display: flex;
    flex-direction: column;
    gap: 16px;
  }
  .row {
    width: 100%;
  }
  .buttons {
    margin-top: 16px;
  }
</style>
