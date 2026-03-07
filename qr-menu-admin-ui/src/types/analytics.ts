import type { IconName } from '@/components/shared';

export type AnalyticsRange = 'today' | 'week' | 'month';

export interface AnalyticsRangeConfig {
  title: string;
  salesChartTitle: string;
  ordersChartTitle: string;
  bucketType: 'hour' | 'day';
  points: number;
}

export interface AnalyticsKpiCard {
  icon: IconName;
  title: string;
  value: string;
}

export interface AnalyticsChartPoint {
  key: string;
  label: string;
  value: number;
}

export interface AnalyticsPopularDish {
  name: string;
  ordersCount: number;
  totalSum: number;
}

export interface AnalyticsOverview {
  dateFrom: string;
  dateTo: string;
  totalSales: number;
  totalOrders: number;
  averageCheck: number;
  averageTimeMinutes: number;
  salesByHours: AnalyticsChartPoint[];
  ordersByHours: AnalyticsChartPoint[];
  salesByDays: AnalyticsChartPoint[];
  ordersByDays: AnalyticsChartPoint[];
  popularDishes: AnalyticsPopularDish[];
}
