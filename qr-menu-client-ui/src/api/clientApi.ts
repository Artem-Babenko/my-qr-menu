import { apiClient } from './api';
import type { TableInfo } from '@/types/table';
import type { MenuCategory } from '@/types/menu';
import type { CreateOrderPayload, CreateOrderResult, OrderView } from '@/types/orders';

export const clientApi = {
  getTable: (tableId: number) =>
    apiClient.get<TableInfo>(`/table/${tableId}`),

  getMenu: (establishmentId: number) =>
    apiClient.get<MenuCategory[]>(`/menu/${establishmentId}`),

  createOrder: (payload: CreateOrderPayload) =>
    apiClient.post<CreateOrderResult>('/orders/create', payload),

  getOrdersByTable: (tableId: number) =>
    apiClient.get<OrderView[]>(`/orders/by-table/${tableId}`),
};
