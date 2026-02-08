<script setup lang="ts">
  import { computed, ref } from 'vue';
  import { useRoute } from 'vue-router';
  import { AppText, AppCard, AppIcon } from '@/components/shared';
  import PageHeader from '@/components/headers/PageHeader.vue';
  import { clientApi } from '@/api/clientApi';
  import { useLoader } from '@/composables/useLoader';
  import type { OrderView } from '@/types/orders';

  const route = useRoute();
  const tableId = computed(() => Number(route.params.tableId));

  const { data: orders, loading, refetch } = useLoader<OrderView[] | null>({
    keys: [tableId],
    fn: async () => {
      const resp = await clientApi.getOrdersByTable(tableId.value);
      return resp.success ? resp.data : null;
    },
    enabled: () => tableId.value > 0,
  });

  const statusLabels: Record<number, string> = {
    1: 'Нове',
    2: 'Прийнято',
    3: 'На кухні',
    4: 'Готується',
    5: 'Готове',
    6: 'Завершено',
    7: 'Скасовано',
  };

  const statusColors: Record<number, string> = {
    1: '#666666',
    2: '#1b7b33',
    3: '#b9ae5a',
    4: '#d4820a',
    5: '#1b7b33',
    6: '#000000',
    7: '#7b1b1b',
  };

  function formatTime(dateStr: string) {
    const d = new Date(dateStr);
    return d.toLocaleTimeString('uk-UA', {
      hour: '2-digit',
      minute: '2-digit',
    });
  }
</script>

<template>
  <div class="orders-page">
    <div class="orders-header-row">
      <page-header
        title="Замовлення"
        subtitle="Історія замовлень за цим столиком"
      />
      <button class="refresh-btn" @click="refetch">
        <app-icon name="RefreshCw" :size="18" />
      </button>
    </div>

    <div v-if="loading" class="loading">
      <app-text size="s" color="secondary">Завантаження...</app-text>
    </div>

    <div v-else-if="!orders || orders.length === 0" class="empty">
      <app-icon name="ClipboardList" :size="48" color="var(--border)" />
      <app-text size="m" color="secondary">Замовлень ще немає</app-text>
    </div>

    <div v-else class="orders-list">
      <app-card v-for="order in orders" :key="order.id" class="order-card">
        <div class="order-top">
          <app-text size="m" weight="600">
            Замовлення #{{ order.orderNumber }}
          </app-text>
          <span
            class="status-badge"
            :style="{ color: statusColors[order.status] ?? '#666', borderColor: statusColors[order.status] ?? '#666' }"
          >
            {{ statusLabels[order.status] ?? 'Невідомо' }}
          </span>
        </div>

        <app-text size="xs" color="secondary">
          {{ formatTime(order.createdAt) }}
        </app-text>

        <div class="order-items">
          <div
            v-for="(item, idx) in order.items"
            :key="idx"
            class="order-item-row"
          >
            <app-text size="s" class="item-name">{{ item.name }}</app-text>
            <app-text size="xs" color="secondary">
              {{ item.quantity }} x {{ item.price }} ₴
            </app-text>
            <app-text size="s" weight="500">{{ item.lineTotal.toFixed(2) }} ₴</app-text>
          </div>
        </div>

        <div class="order-total">
          <app-text size="m" weight="600">Разом:</app-text>
          <app-text size="m" weight="600">
            {{ order.totalSum.toFixed(2) }} ₴
          </app-text>
        </div>

        <div
          v-if="order.customerFullName || order.comment"
          class="order-meta"
        >
          <app-text v-if="order.customerFullName" size="xs" color="secondary" line="m">
            <strong>ПІБ:</strong> {{ order.customerFullName }}
          </app-text>
          <app-text v-if="order.comment" size="xs" color="secondary" line="m">
            <strong>Коментар:</strong> {{ order.comment }}
          </app-text>
        </div>
      </app-card>
    </div>
  </div>
</template>

<style scoped>
  .orders-page {
    display: flex;
    flex-direction: column;
  }

  .orders-header-row {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
  }

  .refresh-btn {
    background: transparent;
    border: 1px solid var(--border);
    border-radius: var(--radius-sm);
    cursor: pointer;
    padding: 8px;
    display: flex;
    align-items: center;
    justify-content: center;
    color: var(--secondary-text);
    transition: all 0.2s ease;
    outline: none;
    flex-shrink: 0;
  }

  .refresh-btn:hover {
    border-color: var(--primary);
    color: var(--primary);
  }

  .loading,
  .empty {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: var(--gap);
    padding: clamp(40px, 10vmin, 60px) 0;
  }

  .orders-list {
    display: flex;
    flex-direction: column;
    gap: var(--gap);
  }

  .order-card {
    display: flex;
    flex-direction: column;
    gap: clamp(6px, 1.5vw, 10px);
  }

  .order-top {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .status-badge {
    font: var(--font-xxs);
    font-weight: 600;
    padding: 3px 8px;
    border-radius: 10px;
    border: 1px solid;
    white-space: nowrap;
  }

  .order-items {
    display: flex;
    flex-direction: column;
    gap: clamp(4px, 1vmin, 6px);
    padding: clamp(6px, 1.5vw, 10px) 0;
    border-top: 1px solid var(--border);
    border-bottom: 1px solid var(--border);
  }

  .order-item-row {
    display: flex;
    align-items: center;
    gap: var(--gap);
  }

  .item-name {
    flex: 1;
    min-width: 0;
  }

  .order-total {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .order-meta {
    display: flex;
    flex-direction: column;
    gap: 2px;
    padding-top: clamp(4px, 1vmin, 6px);
    border-top: 1px dashed var(--border);
  }
</style>
