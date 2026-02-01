export enum OrderStatus {
  new = 1,
  accepted,
  inKitchen,
  cooking,
  ready,
  completed,
  cancelled,
}

export enum OrderStaffRole {
  waiter = 1,
  cook,
}

export enum OrderSource {
  qrMenu = 1,
  adminPanel,
}

const orderStatusLabels: Record<OrderStatus, string> = {
  [OrderStatus.new]: 'Нове',
  [OrderStatus.accepted]: 'У роботі',
  [OrderStatus.inKitchen]: 'На кухні',
  [OrderStatus.cooking]: 'Готує',
  [OrderStatus.ready]: 'Готове',
  [OrderStatus.completed]: 'Виконано',
  [OrderStatus.cancelled]: 'Скасовано',
};

export function getOrderStatusText(status: OrderStatus): string {
  return orderStatusLabels[status as OrderStatus] ?? '—';
}
