<script setup lang="ts">
  import { ref } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { ROUTES } from '@/router';
  import { AppButton, AppIcon, AppText, AppCard, AppInput } from '@/components/shared';
  import PageHeader from '@/components/headers/PageHeader.vue';
  import { useBasketStore } from '@/store/basket';
  import { useToast } from '@/composables/useToast';
  import { clientApi } from '@/api/clientApi';

  const route = useRoute();
  const router = useRouter();
  const basketStore = useBasketStore();
  const toast = useToast();

  const tableId = Number(route.params.tableId);

  const customerName = ref<string | null>(null);
  const comment = ref<string | null>(null);
  const submitting = ref(false);

  async function placeOrder() {
    if (basketStore.items.length === 0) return;

    submitting.value = true;
    const resp = await clientApi.createOrder({
      tableId,
      customerFullName: customerName.value || null,
      comment: comment.value || null,
      items: basketStore.items.map((i) => ({
        productId: i.productId,
        quantity: i.quantity,
      })),
    });
    submitting.value = false;

    if (resp.success && resp.data) {
      toast.success(`Замовлення #${resp.data.orderNumber} створено!`);
      basketStore.clear();
      customerName.value = null;
      comment.value = null;
      router.push({ name: ROUTES.orders, params: { tableId } });
    } else {
      toast.error('Не вдалося створити замовлення');
    }
  }
</script>

<template>
  <div class="basket-page">
    <page-header
      title="Кошик"
      subtitle="Перевірте замовлення перед відправкою"
    />

    <div v-if="basketStore.items.length === 0" class="empty">
      <app-icon name="ShoppingCart" :size="48" color="var(--outline-variant)" />
      <app-text size="m" color="secondary">Кошик порожній</app-text>
      <app-text size="xs" color="secondary" line="m">
        Додайте страви з меню
      </app-text>
    </div>

    <template v-else>
      <div class="basket-items">
        <app-card
          v-for="item in basketStore.items"
          :key="item.productId"
          class="basket-item"
        >
          <div class="item-top">
            <div class="item-info">
              <app-text size="s" weight="600" line="m">
                {{ item.name }}
              </app-text>
              <app-text size="s" color="secondary">
                {{ item.price }} ₴ / шт
              </app-text>
            </div>
            <button class="remove-btn" @click="basketStore.remove(item.productId)">
              <app-icon name="X" :size="16" color="var(--on-surface-variant)" />
            </button>
          </div>

          <div class="item-bottom">
            <div class="qty-controls">
              <button
                class="qty-btn"
                @click="basketStore.decrement(item.productId)"
              >
                <app-icon name="Minus" :size="14" />
              </button>
              <app-text size="s" weight="600">{{ item.quantity }}</app-text>
              <button
                class="qty-btn"
                @click="basketStore.increment(item.productId)"
              >
                <app-icon name="Plus" :size="14" />
              </button>
            </div>
            <app-text size="m" weight="600">
              {{ (item.price * item.quantity).toFixed(2) }} ₴
            </app-text>
          </div>
        </app-card>
      </div>

      <div class="total-row">
        <app-text size="l" weight="600">Разом:</app-text>
        <app-text size="l" weight="600">
          {{ basketStore.totalSum.toFixed(2) }} ₴
        </app-text>
      </div>

      <div class="order-fields">
        <div class="field">
          <app-text size="xs" color="secondary" weight="500">ПІБ (не обов'язково)</app-text>
          <app-input v-model="customerName" placeholder="Ваше ім'я" />
        </div>
        <div class="field">
          <app-text size="xs" color="secondary" weight="500">
            Коментар (не обов'язково)
          </app-text>
          <app-input
            v-model="comment"
            type="textarea"
            placeholder="Побажання до замовлення"
          />
        </div>
      </div>

      <div class="actions">
        <button class="clear-btn" @click="basketStore.clear()">
          <app-icon name="Trash2" :size="20" />
        </button>
        <app-button
          type="filled"
          :disabled="submitting"
          class="order-btn"
          @click="placeOrder"
        >
          <app-icon name="Send" :size="16" />
          {{ submitting ? 'Відправка...' : 'Замовити' }}
        </app-button>
      </div>
    </template>
  </div>
</template>

<style scoped>
  .basket-page {
    display: flex;
    flex-direction: column;
  }

  .empty {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: var(--gap);
    padding: clamp(40px, 10vmin, 60px) 0;
  }

  .basket-items {
    display: flex;
    flex-direction: column;
    gap: var(--gap);
  }

  .basket-item {
    display: flex;
    flex-direction: column;
    gap: clamp(8px, 2vw, 12px);
  }

  .item-top {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    gap: var(--gap);
  }

  .item-info {
    display: flex;
    flex-direction: column;
    gap: 2px;
    flex: 1;
    min-width: 0;
  }

  .remove-btn {
    background: transparent;
    border: none;
    cursor: pointer;
    padding: 4px;
    display: flex;
    align-items: center;
    outline: none;
  }

  .item-bottom {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .qty-controls {
    display: flex;
    align-items: center;
    gap: clamp(8px, 2vw, 12px);
  }

  .qty-btn {
    width: clamp(30px, 7vmin, 36px);
    height: clamp(30px, 7vmin, 36px);
    border-radius: var(--radius-sm);
    border: 1px solid var(--outline-variant);
    background: var(--surface-container-lowest);
    color: var(--on-surface);
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    transition: all 0.2s ease;
    outline: none;
    position: relative;
    overflow: hidden;
  }

  .qty-btn::after {
    content: '';
    position: absolute;
    inset: 0;
    background-color: currentColor;
    opacity: 0;
    transition: opacity 0.2s ease;
    pointer-events: none;
  }
  .qty-btn:hover::after {
    opacity: 0.08;
  }

  .qty-btn:hover {
    border-color: var(--primary);
  }

  .total-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: var(--gap) 0;
    margin-top: var(--gap);
    border-top: 1px solid var(--outline-variant);
  }

  .order-fields {
    display: flex;
    flex-direction: column;
    gap: var(--gap);
    margin-top: var(--gap);
  }

  .field {
    display: flex;
    flex-direction: column;
    gap: clamp(4px, 1vmin, 6px);
  }

  .actions {
    display: flex;
    align-items: center;
    gap: var(--gap);
    margin-top: var(--gap-lg);
  }

  .clear-btn {
    width: clamp(40px, 10vmin, 48px);
    height: clamp(40px, 10vmin, 48px);
    border-radius: var(--radius-sm);
    border: 1px solid var(--outline-variant);
    background: var(--surface-container-lowest);
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    transition: all 0.2s ease;
    color: var(--on-surface-variant);
    outline: none;
    flex-shrink: 0;
  }

  .clear-btn:hover {
    border-color: var(--error);
    color: var(--error);
  }

  .order-btn {
    flex: 1;
  }
</style>
