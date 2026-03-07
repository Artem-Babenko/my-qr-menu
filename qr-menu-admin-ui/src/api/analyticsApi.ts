import { apiClient } from './api';
import type { AnalyticsOverview } from '@/types/analytics';

export const analyticsApi = {
  getOverview: (networkId: number, dateFrom: string, dateTo: string) =>
    apiClient.get<AnalyticsOverview>(`/analytics/network/${networkId}`, {
      params: { dateFrom, dateTo },
    }),
};
