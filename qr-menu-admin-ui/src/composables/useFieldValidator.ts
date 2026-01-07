import type { ValidationRule } from '@/types/rules';
import { computed } from 'vue';

export function useFieldValidator<
  TValue,
  TErrors extends Record<string, string | undefined>,
  TKey extends keyof TErrors,
>(props: {
  value: () => TValue;
  rules: ValidationRule<TValue>[];
  errorName: TKey;
  errors: TErrors;
}) {
  type ErrorValue = TErrors[TKey];

  const error = computed<ErrorValue>({
    get: () => props.errors[props.errorName],
    set: (err) => {
      props.errors[props.errorName] = err;
    },
  });

  const validate = () => {
    for (const rule of props.rules) {
      const res = rule(props.value());
      if (typeof res === 'string') {
        error.value = res as ErrorValue;
        return false;
      }
    }
    error.value = undefined as ErrorValue;
    return true;
  };

  return { validate };
}
