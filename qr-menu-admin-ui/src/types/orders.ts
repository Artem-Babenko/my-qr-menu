import type { OrderSource, OrderStaffRole, OrderStatus } from '@/consts/orders';

export type OrderStatusGroup = 'new' | 'inWork' | 'completed' | 'cancelled';

export interface OrderItemShort {
  name: string;
  quantity: number;
}

export interface OrderListItem {
  id: number;
  orderNumber: number;
  status: OrderStatus;
  createdAt: string;
  closedAt: string | null;
  establishmentId: number;
  establishmentName: string;
  tableId: number | null;
  tableNumber: string | null;
  waiterId: number | null;
  waiterFullName: string | null;
  items: OrderItemShort[];
}

export interface OrderItemRequest {
  productId: number;
  quantity: number;
}

export interface CreateOrderRequest {
  establishmentId: number;
  tableId?: number | null;
  waiterId?: number | null;
  customerFullName?: string | null;
  comment?: string | null;
  items: OrderItemRequest[];
}

export interface UpdateOrderRequest {
  tableId?: number | null;
  customerFullName?: string | null;
  comment?: string | null;
  items: OrderItemRequest[];
}

export interface OrderItemDetails {
  id: number;
  productId: number | null;
  productName: string;
  categoryName: string;
  price: number;
  quantity: number;
  lineTotal: number;
}

export interface OrderStaff {
  userId: number;
  fullName: string;
  role: OrderStaffRole;
  assignedAt: string;
}

export interface OrderStatusHistory {
  id: number;
  fromStatus: OrderStatus;
  toStatus: OrderStatus;
  changedAt: string;
  changedByUserId: number;
  changedByUserFullName: string;
}

export interface OrderDetails {
  id: number;
  orderNumber: number;
  status: OrderStatus;
  source: OrderSource;
  totalSum: number;
  isPaid: boolean;
  customerFullName: string | null;
  comment: string | null;
  createdAt: string;
  closedAt: string | null;
  establishmentId: number;
  establishmentName: string;
  tableId: number | null;
  tableNumber: string | null;
  createdByUserId: number | null;
  createdByUserFullName: string | null;
  items: OrderItemDetails[];
  staff: OrderStaff[];
  statusHistory: OrderStatusHistory[];
}
