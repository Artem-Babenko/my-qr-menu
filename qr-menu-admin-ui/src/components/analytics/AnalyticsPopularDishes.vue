<script setup lang="ts">
  import { AppCard, AppText } from '@/components/shared';
  import type { AnalyticsPopularDish } from '@/types/analytics';
  import { formatMoney } from '@/utils/numbers';

  defineProps<{ dishes: AnalyticsPopularDish[] }>();
</script>

<template>
  <app-card class="popular-card">
    <app-text size="m" weight="600">Популярні страви</app-text>
    <div v-if="dishes.length" class="popular-list">
      <div v-for="dish in dishes" :key="dish.name" class="popular-row">
        <app-text weight="600">{{ dish.name }}</app-text>
        <div class="popular-meta">
          <app-text size="xs" color="secondary">
            Замовлень: {{ dish.ordersCount }}
          </app-text>
          <app-text size="xs" color="secondary">
            Сума: {{ formatMoney(dish.totalSum) }} грн
          </app-text>
        </div>
      </div>
    </div>
    <app-text v-else color="secondary">
      Немає завершених замовлень у вибраному діапазоні
    </app-text>
  </app-card>
</template>

<style scoped>
  .popular-card {
    display: flex;
    flex-direction: column;
    gap: 14px;
  }

  .popular-list {
    display: flex;
    flex-direction: column;
    gap: 10px;
  }

  .popular-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 12px;
    border-bottom: 1px solid var(--outline-variant);
    padding-bottom: 10px;
  }
  .popular-row > :first-child {
    min-width: 0;
    overflow-wrap: anywhere;
    word-break: break-word;
  }

  .popular-meta {
    display: flex;
    gap: 12px;
    flex-wrap: wrap;
    justify-content: flex-end;
    min-width: 0;
  }

  @media (max-width: 768px) {
    .popular-row {
      flex-direction: column;
      align-items: flex-start;
    }
    .popular-meta {
      justify-content: flex-start;
    }
  }
</style>
