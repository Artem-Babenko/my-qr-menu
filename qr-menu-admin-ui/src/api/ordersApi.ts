import { apiClient } from './api';
import type {
  CreateOrderRequest,
  OrderDetails,
  OrderListItem,
  UpdateOrderRequest,
} from '@/types/orders';

export const ordersApi = {
  byNetwork: (networkId: number) =>
    apiClient.get<OrderListItem[]>(`/orders/by-network/${networkId}`),

  byId: (orderId: number) => apiClient.get<OrderDetails>(`/orders/${orderId}`),

  create: (payload: CreateOrderRequest) =>
    apiClient.post<{ orderId: number }>(`/orders/create`, payload),

  update: (orderId: number, payload: UpdateOrderRequest) =>
    apiClient.put<void>(`/orders/${orderId}`, payload),

  takeInWork: (orderId: number) =>
    apiClient.post<void>(`/orders/${orderId}/take-in-work`),

  sendToKitchen: (orderId: number) =>
    apiClient.post<void>(`/orders/${orderId}/send-to-kitchen`),

  startCooking: (orderId: number) =>
    apiClient.post<void>(`/orders/${orderId}/start-cooking`),

  markReady: (orderId: number) =>
    apiClient.post<void>(`/orders/${orderId}/mark-ready`),

  return: (orderId: number) =>
    apiClient.post<void>(`/orders/${orderId}/return`),

  complete: (orderId: number) =>
    apiClient.post<void>(`/orders/${orderId}/complete`),

  cancel: (orderId: number) =>
    apiClient.post<void>(`/orders/${orderId}/cancel`),

  delete: (orderId: number) => apiClient.delete<void>(`/orders/${orderId}`),
};
