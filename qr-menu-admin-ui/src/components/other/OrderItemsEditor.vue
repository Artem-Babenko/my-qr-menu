<script setup lang="ts">
  import { AppButton, AppFlex, AppLabel, AppText } from '@/components/shared';
  import { computed, ref, watch } from 'vue';
  import ProductPickerModal from '@/components/modals/ProductPickerModal.vue';

  export interface EditableOrderItem {
    productId: number;
    quantity: number;
    name?: string;
    categoryName?: string;
    price?: number;
  }

  const props = defineProps<{
    establishmentId: number | null;
    disabled?: boolean;
  }>();

  const model = defineModel<EditableOrderItem[]>({ required: true });

  const pickerShowed = ref(false);

  const openPicker = () => {
    if (!props.establishmentId || props.disabled) return;
    pickerShowed.value = true;
  };

  const hasItems = computed(() => (model.value?.length ?? 0) > 0);

  const normalizeQty = (n: number) => Math.min(99, Math.max(1, Number(n) || 1));

  const setQty = (productId: number, qty: number) => {
    const next = normalizeQty(qty);
    const item = model.value.find((x) => x.productId === productId);
    if (item) item.quantity = next;
  };

  const removeItem = (productId: number) => {
    model.value = model.value.filter((x) => x.productId !== productId);
  };

  const onPicked = (
    picked: { id: number; name: string; categoryName: string; price: number }[],
  ) => {
    const byId = new Map(model.value.map((x) => [x.productId, x]));
    for (const p of picked) {
      const existing = byId.get(p.id);
      if (existing) {
        existing.quantity = normalizeQty(existing.quantity + 1);
        existing.name ??= p.name;
        existing.categoryName ??= p.categoryName;
        existing.price ??= p.price;
      } else {
        model.value.push({
          productId: p.id,
          quantity: 1,
          name: p.name,
          categoryName: p.categoryName,
          price: p.price,
        });
      }
    }
  };

  watch(
    () => props.establishmentId,
    (newId, oldId) => {
      if (newId && oldId && newId !== oldId) model.value = [];
    },
  );
</script>

<template>
  <div class="items-block">
    <app-flex justify="space-between" align="center">
      <app-label label="Список товарів"></app-label>
      <app-button
        type="outline"
        :disabled="!establishmentId || disabled"
        @click="openPicker"
      >
        Додати товар
      </app-button>
    </app-flex>

    <div v-if="!establishmentId" class="hint">
      Спочатку оберіть заклад, щоб додати товари
    </div>
    <div v-else-if="!hasItems" class="hint">Товари не додані</div>

    <div v-else class="items-list">
      <div v-for="it in model" :key="it.productId" class="item-row">
        <div class="left">
          <div class="name">{{ it.name ?? `Товар #${it.productId}` }}</div>
          <div class="cat">{{ it.categoryName ?? '' }}</div>
        </div>
        <div class="right">
          <div class="price">{{ it.price ?? '' }}</div>
          <input
            class="qty"
            type="number"
            min="1"
            max="99"
            :disabled="disabled"
            :value="it.quantity"
            @input="
              setQty(
                it.productId,
                Number(($event.target as HTMLInputElement).value),
              )
            "
          />
          <button
            class="remove"
            type="button"
            :disabled="disabled"
            @click="removeItem(it.productId)"
          >
            ✕
          </button>
        </div>
      </div>
    </div>

    <product-picker-modal
      v-model:showed="pickerShowed"
      :establishment-id="establishmentId"
      @picked="onPicked"
    ></product-picker-modal>
  </div>
</template>

<style scoped>
  .items-block {
    border: 1px solid var(--outline-variant);
    border-radius: 10px;
    padding: 12px;
    display: flex;
    flex-direction: column;
    gap: 10px;
  }
  .hint {
    color: var(--on-surface-variant);
    font-size: 14px;
  }
  .items-list {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }
  .item-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 8px 10px;
    border: 1px solid var(--outline-variant);
    border-radius: 10px;
  }
  .left {
    display: flex;
    flex-direction: column;
    gap: 2px;
  }
  .cat {
    color: var(--on-surface-variant);
    font-size: 13px;
  }
  .right {
    display: flex;
    align-items: center;
    gap: 10px;
  }
  .price {
    min-width: 70px;
    text-align: right;
    color: var(--on-surface-variant);
  }
  .qty {
    width: 90px;
    padding: 6px 8px;
    border: 1px solid var(--outline-variant);
    border-radius: 8px;
    background: var(--surface-container);
    color: var(--on-surface);
  }
  .remove {
    border: none;
    background: transparent;
    color: var(--on-surface-variant);
    cursor: pointer;
    font-size: 16px;
    padding: 2px 6px;
    line-height: 1;
  }
  .remove:disabled {
    opacity: 0.6;
    cursor: not-allowed;
  }
</style>
