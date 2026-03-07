<script setup lang="ts">
  import { AppCard, AppIcon, AppText } from '@/components/shared';
  import type { AnalyticsKpiCard } from '@/types/analytics';

  defineProps<{ cards: AnalyticsKpiCard[] }>();
</script>

<template>
  <section class="kpi-grid">
    <app-card v-for="card in cards" :key="card.title" class="kpi-card">
      <div class="icon-box">
        <app-icon :name="card.icon" :size="20"></app-icon>
      </div>
      <div class="kpi-texts">
        <app-text size="xs" color="secondary">{{ card.title }}</app-text>
        <app-text size="xl" weight="600">{{ card.value }}</app-text>
      </div>
    </app-card>
  </section>
</template>

<style scoped>
  .kpi-grid {
    display: grid;
    grid-template-columns: repeat(4, minmax(0, 1fr));
    gap: 16px;
  }

  .kpi-card {
    display: flex;
    align-items: center;
    gap: 12px;
  }

  .icon-box {
    width: 36px;
    height: 36px;
    border-radius: 10px;
    background-color: var(--secondary-container);
    color: var(--on-secondary-container);
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
  }

  .kpi-texts {
    min-width: 0;
    display: flex;
    flex-direction: column;
    gap: 4px;
  }

  @media (max-width: 1100px) {
    .kpi-grid {
      grid-template-columns: repeat(2, minmax(0, 1fr));
    }
  }

  @media (max-width: 768px) {
    .kpi-grid {
      grid-template-columns: 1fr;
      gap: 12px;
    }
  }
</style>
