import type { AppTab } from '@/types/components';
import type { AnalyticsRange, AnalyticsRangeConfig } from '@/types/analytics';

export const ANALYTICS_RANGE_TABS: AppTab[] = [
  { id: 'today', title: 'Сьогодні' },
  { id: 'week', title: 'Тиждень' },
  { id: 'month', title: 'Місяць' },
];

export const ANALYTICS_RANGE_CONFIGS: Record<AnalyticsRange, AnalyticsRangeConfig> = {
  today: {
    title: 'сьогодні',
    salesChartTitle: 'Продажі по годинах',
    ordersChartTitle: 'Замовлення по годинах',
    bucketType: 'hour',
    points: 24,
  },
  week: {
    title: 'за тиждень',
    salesChartTitle: 'Продажі по днях',
    ordersChartTitle: 'Замовлення по днях',
    bucketType: 'day',
    points: 7,
  },
  month: {
    title: 'за місяць',
    salesChartTitle: 'Продажі по днях',
    ordersChartTitle: 'Замовлення по днях',
    bucketType: 'day',
    points: 30,
  },
};
