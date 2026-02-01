import { errorCodes } from './errorCodes';

const errorMessages: Record<string, string> = {
  [errorCodes.invalidCredentials]: 'Неправильний номер телефону або пароль',
  [errorCodes.duplicatePhone]: 'Користувач з таким номером вже існує',
  [errorCodes.invitationNotFound]:
    'Запрошення не знайдено або воно більше не дійсне',
  [errorCodes.userNotFound]: 'Користувача не знайдено',
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
};

const DEFAULT_ERROR = 'Сталася невідома помилка';

export function getErrorMessage(code?: string | null): string {
  if (!code) return DEFAULT_ERROR;
  return errorMessages[code] ?? DEFAULT_ERROR;
}
