<script setup lang="ts">
  import { productsApi } from '@/api/productsApi';
  import {
    AppButton,
    AppCheckbox,
    AppFlex,
    AppModal,
    AppSearchInput,
    AppText,
  } from '@/components/shared';
  import { getErrorMessage } from '@/consts/errorMessages';
  import { useToastsStore } from '@/store/toasts';
  import { ref, watch } from 'vue';

  interface ProductItem {
    id: number;
    name: string;
    categoryName: string;
    price: number;
  }

  const showed = defineModel<boolean>('showed', { required: true });
  const props = defineProps<{ establishmentId: number | null }>();
  const emit = defineEmits<{
    picked: [items: ProductItem[]];
  }>();

  const toasts = useToastsStore();

  const query = ref('');
  const loading = ref(false);
  const items = ref<ProductItem[]>([]);
  const selected = ref<Record<number, boolean>>({});

  const reset = () => {
    query.value = '';
    loading.value = false;
    items.value = [];
    selected.value = {};
  };

  watch(showed, (v) => {
    if (v) return;
    reset();
  });

  const search = async () => {
    const estId = props.establishmentId;
    if (!estId) return;

    const q = query.value.trim();
    if (q.length < 2) {
      items.value = [];
      selected.value = {};
      return;
    }

    loading.value = true;
    try {
      const resp = await productsApi.lookup(estId, q);
      if (!resp.success) {
        toasts.error(getErrorMessage(resp.errorCode));
        items.value = [];
        selected.value = {};
        return;
      }
      items.value = resp.data ?? [];
      selected.value = Object.fromEntries(
        items.value.map((it) => [it.id, false]),
      );
    } finally {
      loading.value = false;
    }
  };

  watch(query, async () => {
    await search();
  });

  const onChoose = () => {
    const picked = items.value.filter((x) => selected.value[x.id]);
    emit('picked', picked);
    showed.value = false;
  };
</script>

<template>
  <app-modal v-model:showed="showed" title="Додати товар" :width="560">
    <div class="top">
      <app-search-input
        v-model="query"
        placeholder="Пошук товарів..."
      ></app-search-input>
      <app-text color="secondary">
        Пошук від 2 символів, до 10 результатів
      </app-text>
    </div>

    <app-text v-if="loading" color="secondary">Завантаження...</app-text>
    <app-text v-else-if="!items.length && query.length >= 2" color="secondary">
      Нічого не знайдено
    </app-text>

    <div v-else class="list">
      <div v-for="it in items" :key="it.id" class="row">
        <app-checkbox v-model="selected[it.id]!"></app-checkbox>
        <div class="main">
          <app-text>{{ it.name }}</app-text>
          <app-text color="secondary">{{ it.categoryName }}</app-text>
        </div>
        <div class="price">
          <app-text weight="600">{{ it.price }}</app-text>
        </div>
      </div>
    </div>

    <app-flex justify="flex-end" gap="10" class="buttons">
      <app-button type="outline" @click="showed = false">Скасувати</app-button>
      <app-button
        :disabled="!Object.values(selected).some(Boolean)"
        @click="onChoose"
      >
        Обрати
      </app-button>
    </app-flex>
  </app-modal>
</template>

<style scoped>
  .top {
    display: flex;
    flex-direction: column;
    gap: 8px;
    margin-bottom: 12px;
  }
  .list {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }
  .row {
    display: grid;
    grid-template-columns: 28px 1fr auto;
    align-items: center;
    gap: 10px;
    border: 1px solid var(--border);
    border-radius: 12px;
    padding: 10px 12px;
  }
  .main {
    display: flex;
    flex-direction: column;
    gap: 2px;
  }
  .price {
    min-width: 80px;
    text-align: right;
  }
  .buttons {
    margin-top: 16px;
  }
</style>
