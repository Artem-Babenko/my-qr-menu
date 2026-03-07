import { analyticsApi } from '@/api/analyticsApi';
import {
  ANALYTICS_RANGE_CONFIGS,
  ANALYTICS_RANGE_TABS,
} from '@/consts/analytics';
import { getErrorMessage } from '@/consts/errorMessages';
import { useNetworkStore } from '@/store/network';
import { useThemeStore } from '@/store/theme';
import { useToastsStore } from '@/store/toasts';
import type {
  AnalyticsChartPoint,
  AnalyticsKpiCard,
  AnalyticsOverview,
  AnalyticsRange,
} from '@/types/analytics';
import type { ChartData, ChartOptions } from 'chart.js';
import { computed, ref, toRef } from 'vue';
import { useLoader } from './useLoader';
import {
  formatDuration,
  getChartMinWidth,
  getMaxTicksLimit,
  getRangeStart,
  readCssVar,
} from '@/utils/analytics';
import { formatMoney } from '@/utils/numbers';

const EMPTY_OVERVIEW: AnalyticsOverview = {
  dateFrom: '',
  dateTo: '',
  totalSales: 0,
  totalOrders: 0,
  averageCheck: 0,
  averageTimeMinutes: 0,
  salesByHours: [],
  ordersByHours: [],
  salesByDays: [],
  ordersByDays: [],
  popularDishes: [],
};

export function useAnalytics() {
  const networkStore = useNetworkStore();
  const themeStore = useThemeStore();
  const toasts = useToastsStore();
  const networkId = toRef(() => networkStore.network?.id);

  const selectedRange = ref<AnalyticsRange>('today');
  const selectedConfig = computed(() => ANALYTICS_RANGE_CONFIGS[selectedRange.value]);

  const dateFrom = computed(() =>
    getRangeStart(new Date(), selectedRange.value, selectedConfig.value.points),
  );
  const dateTo = computed(() => new Date());

  const { data: overviewData, loading } = useLoader<AnalyticsOverview>({
    keys: [
      'analytics-overview',
      networkId,
      selectedRange,
      () => dateFrom.value.toISOString(),
      () => dateTo.value.toISOString(),
    ],
    fn: async () => {
      if (!networkId.value) return EMPTY_OVERVIEW;

      const response = await analyticsApi.getOverview(
        networkId.value,
        dateFrom.value.toISOString(),
        dateTo.value.toISOString(),
      );

      if (!response.success || !response.data) {
        toasts.error(getErrorMessage(response.errorCode));
        return EMPTY_OVERVIEW;
      }

      return response.data;
    },
    enabled: () => !!networkId.value,
  });

  const overview = computed(() => overviewData.value ?? EMPTY_OVERVIEW);

  const chartColors = computed(() => {
    void themeStore.theme;
    if (typeof window === 'undefined') {
      return {
        sales: '#3b82f6',
        orders: '#14b8a6',
        grid: '#d4d4d8',
        axis: '#71717a',
      };
    }
    const styles = getComputedStyle(document.documentElement);
    return {
      sales: readCssVar(styles, '--primary', '#3b82f6'),
      orders: readCssVar(styles, '--tertiary', '#14b8a6'),
      grid: readCssVar(styles, '--outline-variant', '#d4d4d8'),
      axis: readCssVar(styles, '--on-surface-variant', '#71717a'),
    };
  });

  const salesSeries = computed<AnalyticsChartPoint[]>(() =>
    selectedRange.value === 'today'
      ? overview.value.salesByHours
      : overview.value.salesByDays,
  );
  const ordersSeries = computed<AnalyticsChartPoint[]>(() =>
    selectedRange.value === 'today'
      ? overview.value.ordersByHours
      : overview.value.ordersByDays,
  );

  const kpiCards = computed<AnalyticsKpiCard[]>(() => [
    {
      icon: 'CircleDollarSign',
      title: `Продажі ${selectedConfig.value.title}`,
      value: `${formatMoney(overview.value.totalSales)} грн`,
    },
    {
      icon: 'ShoppingBag',
      title: `Замовлень ${selectedConfig.value.title}`,
      value: String(overview.value.totalOrders),
    },
    {
      icon: 'Receipt',
      title: 'Середній чек',
      value: `${formatMoney(overview.value.averageCheck)} грн`,
    },
    {
      icon: 'Clock3',
      title: 'Середній час',
      value: formatDuration(overview.value.averageTimeMinutes),
    },
  ]);

  const salesChartData = computed<ChartData<'bar'>>(() => ({
    labels: salesSeries.value.map((point) => point.label),
    datasets: [
      {
        data: salesSeries.value.map((point) => point.value),
        backgroundColor: chartColors.value.sales,
        borderRadius: 8,
        maxBarThickness: 24,
      },
    ],
  }));

  const ordersChartData = computed<ChartData<'line'>>(() => ({
    labels: ordersSeries.value.map((point) => point.label),
    datasets: [
      {
        data: ordersSeries.value.map((point) => point.value),
        borderColor: chartColors.value.orders,
        backgroundColor: chartColors.value.orders,
        pointBackgroundColor: chartColors.value.orders,
        pointRadius: 4,
        pointHoverRadius: 5,
        borderWidth: 3,
        tension: 0.35,
      },
    ],
  }));

  const salesChartOptions = computed<ChartOptions<'bar'>>(() => ({
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false },
      tooltip: {
        callbacks: {
          label: (context) => `${formatMoney(Number(context.raw ?? 0))} грн`,
        },
      },
    },
    scales: {
      x: {
        grid: { display: false },
        ticks: {
          color: chartColors.value.axis,
          maxRotation: 0,
          autoSkip: true,
          maxTicksLimit: getMaxTicksLimit(selectedConfig.value.points),
        },
      },
      y: {
        beginAtZero: true,
        ticks: {
          color: chartColors.value.axis,
          callback: (value) => formatMoney(Number(value)),
        },
        grid: { color: chartColors.value.grid },
      },
    },
  }));

  const ordersChartOptions = computed<ChartOptions<'line'>>(() => ({
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false },
      tooltip: {
        callbacks: {
          label: (context) => `${Number(context.raw ?? 0)} зам.`,
        },
      },
    },
    scales: {
      x: {
        grid: { display: false },
        ticks: {
          color: chartColors.value.axis,
          maxRotation: 0,
          autoSkip: true,
          maxTicksLimit: getMaxTicksLimit(selectedConfig.value.points),
        },
      },
      y: {
        beginAtZero: true,
        ticks: {
          color: chartColors.value.axis,
          precision: 0,
        },
        grid: { color: chartColors.value.grid },
      },
    },
  }));

  const chartMinWidth = computed(() => getChartMinWidth(selectedConfig.value.points));
  const monthLayout = computed(() => selectedRange.value === 'month');

  return {
    tabs: ANALYTICS_RANGE_TABS,
    selectedRange,
    selectedConfig,
    kpiCards,
    chartMinWidth,
    salesChartData,
    ordersChartData,
    salesChartOptions,
    ordersChartOptions,
    monthLayout,
    popularDishes: computed(() => overview.value.popularDishes),
    loading,
  };
}
