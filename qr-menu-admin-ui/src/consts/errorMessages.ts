import { errorCodes } from './errorCodes';

const errorMessages: Record<string, string> = {
  [errorCodes.invalidCredentials]: 'Неправильний номер телефону або пароль',
  [errorCodes.duplicatePhone]: 'Користувач з таким номером вже існує',
  [errorCodes.invitationNotFound]:
    'Запрошення не знайдено або воно більше не дійсне',
  [errorCodes.userNotFound]: 'Користувача не знайдено',
  [errorCodes.permissionDenied]: 'Недостатньо прав для виконання цієї дії',
  [errorCodes.duplicateEstablishment]: 'Заклад з такою назвою вже існує',
  [errorCodes.duplicateNetwork]: 'Мережа з такою назвою вже існує',
  [errorCodes.networkNotFound]: 'Мережу не знайдено',
  [errorCodes.establishmentNotFound]: 'Заклад не знайдено',
  [errorCodes.tableNotFound]: 'Стіл не знайдено',
  [errorCodes.duplicateTableNumber]:
    'Стіл з таким номером вже існує у цьому закладі',
  [errorCodes.categoryNotFound]: 'Категорію не знайдено',
  [errorCodes.duplicateCategoryName]: 'Категорія з такою назвою вже існує',
  [errorCodes.categoryDeleteForbidden]:
    'Неможливо видалити категорію, якщо в ній є страви',
  [errorCodes.productNotFound]: 'Страву не знайдено',
  [errorCodes.duplicateProductName]: 'Страва з такою назвою вже існує',
  [errorCodes.productNotAvailable]:
    'Деякі страви недоступні для обраного закладу',
  [errorCodes.orderNotFound]: 'Замовлення не знайдено',
  [errorCodes.orderInvalidStatus]: 'Дію неможливо виконати для цього статусу',
  [errorCodes.orderItemsRequired]: 'Додайте хоча б одну страву до замовлення',
  [errorCodes.orderDeleteForbidden]:
    'Видаляти можна лише скасовані замовлення',
};

const DEFAULT_ERROR = 'Сталася невідома помилка';

export function getErrorMessage(code?: string | null): string {
  if (!code) return DEFAULT_ERROR;
  return errorMessages[code] ?? DEFAULT_ERROR;
}
