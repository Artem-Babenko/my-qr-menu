import type { ValidationRule } from '@/types/rules';

export const nullOrEmptyStringRule: ValidationRule<string> = (val) =>
  (!!val && !!val.trim()) || 'Поле не може бути пустим';

export const passwordLengthRule: ValidationRule<string> = (val) => {
  const length = val?.length ?? 0;
  if (length === 0) return true;
  if (length < 8) return 'Пароль має містити щонайменше 8 символів';
  if (length > 64) return 'Пароль має містити не більше 64 символів';
  return true;
};

const passwordCharsetRegex =
  /^[A-Za-z0-9!@#$%^&*()_\-+=\[{\]};:'",.<>/?\\|`~]+$/;

export const passwordCharsetRule: ValidationRule<string> = (val) => {
  if (!val) return true;
  return (
    passwordCharsetRegex.test(val) ||
    'Пароль може містити тільки латинські літери, цифри та спеціальні символи'
  );
};

const emailRegex = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/;

export const emailRule: ValidationRule<string> = (val) => {
  if (!val) return true;
  return emailRegex.test(val) || 'Введіть коректну електронну адресу';
};

// Перевірка повної довжини телефону у форматі "+38 (###) ###-##-##" (17 символів)
export const phoneLengthRule: ValidationRule<string> = (val) => {
  if (!val) return true;
  // Видаляємо всі нецифрові символи для перевірки кількості цифр
  const digitsOnly = val.replace(/\D/g, '');
  // Маска "+38 (###) ###-##-##" має 13 цифр (38 + 9 цифр)
  if (digitsOnly.length !== 12) {
    return 'Введіть коректний номер телефону';
  }
  // Перевіряємо, що номер починається з 38
  if (!digitsOnly.startsWith('38')) {
    return 'Введіть коректний номер телефону';
  }
  return true;
};
