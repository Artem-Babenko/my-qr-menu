import type { PermissionGroup } from '@/types/roles';

export enum PermissionType {
  menuView = 1,
  menuEdit,
  analyticsView,
  establishmentsView,
  establishmentsEdit,
  usersView,
  usersEdit,
  usersInvite,
  usersRolesEdit,
  settingsView,
  settingsEdit,
  ordersCreate,
  ordersEdit,
  ordersTakeInWork,
  ordersSendToKitchen,
  ordersStartCooking,
  ordersMarkReady,
  ordersReturn,
  ordersComplete,
  ordersCancel,
  ordersDelete,
}

export const PERMISSION_GROUPS: PermissionGroup[] = [
  {
    name: 'Меню',
    permissions: [PermissionType.menuView, PermissionType.menuEdit],
  },
  {
    name: 'Замовлення',
    permissions: [
      PermissionType.ordersCreate,
      PermissionType.ordersEdit,
      PermissionType.ordersTakeInWork,
      PermissionType.ordersSendToKitchen,
      PermissionType.ordersStartCooking,
      PermissionType.ordersMarkReady,
      PermissionType.ordersReturn,
      PermissionType.ordersComplete,
      PermissionType.ordersCancel,
      PermissionType.ordersDelete,
    ],
  },
  {
    name: 'Аналітика',
    permissions: [PermissionType.analyticsView],
  },
  {
    name: 'Заклади',
    permissions: [
      PermissionType.establishmentsView,
      PermissionType.establishmentsEdit,
    ],
  },
  {
    name: 'Користувачі',
    permissions: [
      PermissionType.usersView,
      PermissionType.usersEdit,
      PermissionType.usersInvite,
      PermissionType.usersRolesEdit,
    ],
  },
  {
    name: 'Налаштування',
    permissions: [PermissionType.settingsView, PermissionType.settingsEdit],
  },
];

export const PERMISSION_LABELS: Record<PermissionType, string> = {
  [PermissionType.menuView]: 'Перегляд меню',
  [PermissionType.menuEdit]: 'Редагування меню',
  [PermissionType.ordersCreate]: 'Створення замовлення',
  [PermissionType.ordersEdit]: 'Редагування замовлень',
  [PermissionType.ordersTakeInWork]: 'Взяти у роботу',
  [PermissionType.ordersSendToKitchen]: 'Відправити на кухню',
  [PermissionType.ordersStartCooking]: 'Розпочати готування',
  [PermissionType.ordersMarkReady]: 'Позначити як приготовлене',
  [PermissionType.ordersReturn]: 'Повернути замовлення на попередній статус',
  [PermissionType.ordersComplete]: 'Завершити замовлення',
  [PermissionType.ordersCancel]: 'Скасувати замовлення',
  [PermissionType.ordersDelete]: 'Видалити замовлення',
  [PermissionType.analyticsView]: 'Перегляд аналітики',
  [PermissionType.establishmentsView]: 'Перегляд закладів',
  [PermissionType.establishmentsEdit]: 'Редагування закладів',
  [PermissionType.usersView]: 'Перегляд користувачів',
  [PermissionType.usersEdit]: 'Редагування користувачів',
  [PermissionType.usersInvite]: 'Запрошення користувачів',
  [PermissionType.usersRolesEdit]: 'Редагування ролей користувачів',
  [PermissionType.settingsView]: 'Перегляд налаштувань',
  [PermissionType.settingsEdit]: 'Редагування налаштувань',
};
