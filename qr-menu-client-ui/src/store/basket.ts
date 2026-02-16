import { defineStore } from 'pinia';
import { ref, computed, watch } from 'vue';
import type { BasketItem } from '@/types/basket';

const STORAGE_PREFIX = 'qr-basket-';

export const useBasketStore = defineStore('basket', () => {
  const items = ref<BasketItem[]>([]);
  const tableId = ref<number | null>(null);

  const totalItems = computed(() =>
    items.value.reduce((sum, i) => sum + i.quantity, 0),
  );

  const totalSum = computed(() =>
    items.value.reduce((sum, i) => sum + i.price * i.quantity, 0),
  );

  function init(id: number) {
    tableId.value = id;
    const stored = localStorage.getItem(STORAGE_PREFIX + id);
    if (stored) {
      try {
        items.value = JSON.parse(stored);
      } catch {
        items.value = [];
      }
    } else {
      items.value = [];
    }
  }

  function persist() {
    if (tableId.value !== null) {
      localStorage.setItem(
        STORAGE_PREFIX + tableId.value,
        JSON.stringify(items.value),
      );
    }
  }

  watch(items, persist, { deep: true });

  function add(product: { id: number; name: string; price: number }) {
    const existing = items.value.find((i) => i.productId === product.id);
    if (existing) {
      existing.quantity++;
    } else {
      items.value.push({
        productId: product.id,
        name: product.name,
        price: product.price,
        quantity: 1,
      });
    }
  }

  function increment(productId: number) {
    const item = items.value.find((i) => i.productId === productId);
    if (item) item.quantity++;
  }

  function decrement(productId: number) {
    const item = items.value.find((i) => i.productId === productId);
    if (!item) return;
    item.quantity--;
    if (item.quantity <= 0) {
      items.value = items.value.filter((i) => i.productId !== productId);
    }
  }

  function remove(productId: number) {
    items.value = items.value.filter((i) => i.productId !== productId);
  }

  function getQuantity(productId: number): number {
    return items.value.find((i) => i.productId === productId)?.quantity ?? 0;
  }

  function clear() {
    items.value = [];
  }

  return {
    items,
    totalItems,
    totalSum,
    init,
    add,
    increment,
    decrement,
    remove,
    getQuantity,
    clear,
  };
});
