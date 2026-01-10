import type { ValidationRule } from '@/types/rules';

export const nullOrEmptyStringRule: ValidationRule<string> = (val) =>
  (!!val && !!val.trim()) || 'Поле не може бути пустим';
