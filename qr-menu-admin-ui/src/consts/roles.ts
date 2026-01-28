import type { PermissionGroup } from '@/types/roles';

export enum PermissionType {
  menuView = 1,
  menuEdit,
  ordersView,
  ordersEdit,
  ordersChangeStatus,
  analyticsView,
  establishmentsView,
  establishmentsEdit,
  usersView,
  usersEdit,
  usersInvite,
  usersRolesEdit,
  settingsView,
  settingsEdit,
}

export const PERMISSION_GROUPS: PermissionGroup[] = [
  {
    name: 'Меню',
    permissions: [PermissionType.menuView, PermissionType.menuEdit],
  },
  {
    name: 'Замовлення',
    permissions: [
      PermissionType.ordersView,
      PermissionType.ordersEdit,
      PermissionType.ordersChangeStatus,
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
  [PermissionType.ordersView]: 'Перегляд замовлень',
  [PermissionType.ordersEdit]: 'Редагування замовлень',
  [PermissionType.ordersChangeStatus]: 'Зміна статусу замовлень',
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
