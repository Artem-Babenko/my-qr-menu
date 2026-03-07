<script setup lang="ts">
  import { AppCard, AppText } from '@/components/shared';
  import type { AnalyticsRangeConfig } from '@/types/analytics';
  import {
    CategoryScale,
    Chart as ChartJS,
    LinearScale,
    BarElement,
    LineElement,
    PointElement,
    Tooltip,
    Legend,
    type ChartData,
    type ChartOptions,
  } from 'chart.js';
  import { Bar, Line } from 'vue-chartjs';

  ChartJS.register(
    CategoryScale,
    LinearScale,
    BarElement,
    LineElement,
    PointElement,
    Tooltip,
    Legend,
  );

  defineProps<{
    selectedConfig: AnalyticsRangeConfig;
    monthLayout: boolean;
    chartMinWidth: number;
    salesChartData: ChartData<'bar'>;
    ordersChartData: ChartData<'line'>;
    salesChartOptions: ChartOptions<'bar'>;
    ordersChartOptions: ChartOptions<'line'>;
  }>();
</script>

<template>
  <section class="charts-grid" :class="{ month: monthLayout }">
    <app-card class="chart-card">
      <app-text size="m" weight="600">
        {{ selectedConfig.salesChartTitle }}
      </app-text>
      <div class="chart-body chart-scroll">
        <div class="chart-canvas" :style="{ minWidth: `${chartMinWidth}px` }">
          <Bar :data="salesChartData" :options="salesChartOptions"></Bar>
        </div>
      </div>
    </app-card>

    <app-card class="chart-card">
      <app-text size="m" weight="600">
        {{ selectedConfig.ordersChartTitle }}
      </app-text>
      <div class="chart-body chart-scroll">
        <div class="chart-canvas" :style="{ minWidth: `${chartMinWidth}px` }">
          <Line :data="ordersChartData" :options="ordersChartOptions"></Line>
        </div>
      </div>
    </app-card>
  </section>
</template>

<style scoped>
  .charts-grid {
    display: grid;
    grid-template-columns: repeat(2, minmax(0, 1fr));
    gap: 16px;
  }
  .charts-grid > * {
    min-width: 0;
  }
  .charts-grid.month {
    grid-template-columns: 1fr;
  }

  .chart-card {
    display: flex;
    flex-direction: column;
    gap: 14px;
    min-width: 0;
  }

  .chart-body {
    width: 100%;
    max-width: 100%;
    overflow-x: auto;
    padding-bottom: 4px;
  }
  .chart-scroll {
    scrollbar-width: thin;
  }

  .chart-canvas {
    position: relative;
    height: 260px;
  }
  .chart-canvas :deep(canvas) {
    width: 100% !important;
    height: 100% !important;
  }

  @media (max-width: 1100px) {
    .charts-grid {
      grid-template-columns: 1fr;
    }
  }

  @media (max-width: 768px) {
    .chart-card {
      gap: 10px;
    }
    .chart-canvas {
      height: 220px;
    }
  }
</style>
