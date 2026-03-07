<script setup lang="ts">
  import { AnalyticsCharts, AnalyticsKpiGrid, AnalyticsPopularDishes } from '@/components/analytics';
  import { PageHeader } from '@/components/headers';
  import { AppTabs, AppText } from '@/components/shared';
  import { useAnalytics, usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';
  import { ROUTES } from '@/router';
  import { computed, watch } from 'vue';
  import { useRouter } from 'vue-router';

  const router = useRouter();
  const { hasAny } = usePermissions();
  const analytics = useAnalytics();
  const {
    tabs,
    selectedRange,
    selectedConfig,
    kpiCards,
    chartMinWidth,
    salesChartData,
    ordersChartData,
    salesChartOptions,
    ordersChartOptions,
    monthLayout,
    popularDishes,
    loading,
  } = analytics;

  const canViewAnalytics = computed(() => hasAny(PermissionType.analyticsView));

  watch(
    canViewAnalytics,
    (value) => {
      if (value) return;
      router.replace({ name: ROUTES.dashboard });
    },
    { immediate: true },
  );
</script>

<template>
  <div v-if="canViewAnalytics" class="page">
    <page-header section-name="Аналітика"></page-header>

    <app-tabs
      v-model:selected="selectedRange"
      :tabs="tabs"
    ></app-tabs>

    <analytics-kpi-grid :cards="kpiCards"></analytics-kpi-grid>

    <analytics-charts
      :selected-config="selectedConfig"
      :month-layout="monthLayout"
      :chart-min-width="chartMinWidth"
      :sales-chart-data="salesChartData"
      :orders-chart-data="ordersChartData"
      :sales-chart-options="salesChartOptions"
      :orders-chart-options="ordersChartOptions"
    ></analytics-charts>

    <analytics-popular-dishes :dishes="popularDishes"></analytics-popular-dishes>

    <app-text
      v-if="loading"
      size="xs"
      color="secondary"
    >
      Оновлюю дані...
    </app-text>
  </div>
  <div v-else>
    <app-text color="secondary">
      Недостатньо прав для перегляду аналітики
    </app-text>
  </div>
</template>

<style scoped>
  .page {
    display: flex;
    flex-direction: column;
    gap: 20px;
  }

  @media (max-width: 768px) {
    .page {
      gap: 12px;
    }
  }
</style>
