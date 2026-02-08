export interface OrderView {
  id: number;
  orderNumber: number;
  status: number;
  totalSum: number;
  customerFullName: string | null;
  comment: string | null;
  createdAt: string;
  items: OrderItemView[];
}

export interface OrderItemView {
  name: string;
  quantity: number;
  price: number;
  lineTotal: number;
}

export interface CreateOrderPayload {
  tableId: number;
  customerFullName?: string | null;
  comment?: string | null;
  items: { productId: number; quantity: number }[];
}

export interface CreateOrderResult {
  orderId: number;
  orderNumber: number;
}
