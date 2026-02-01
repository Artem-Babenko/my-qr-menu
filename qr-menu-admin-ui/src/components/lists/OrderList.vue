<script setup lang="ts">
  import BaseCardList from '@/components/lists/BaseCardList.vue';
  import type { OrderListItem } from '@/types/orders';
  import OrderCard from '@/components/cards/OrderCard.vue';
  import { AppText } from '@/components/shared';

  const props = defineProps<{
    orders: OrderListItem[];
    loading?: boolean;
  }>();

  const emit = defineEmits<{
    open: [orderId: number];
    changed: [];
  }>();
</script>

<template>
  <div>
    <app-text v-if="loading" color="secondary">Завантаження...</app-text>
    <app-text v-else-if="!orders.length" color="secondary">
      Немає замовлень
    </app-text>

    <base-card-list v-if="orders.length">
      <order-card
        v-for="o in orders"
        :key="o.id"
        :order="o"
        @open="emit('open', o.id)"
        @changed="emit('changed')"
      ></order-card>
    </base-card-list>
  </div>
</template>

