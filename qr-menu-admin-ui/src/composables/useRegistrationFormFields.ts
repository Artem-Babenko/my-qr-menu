import { useFieldValidator } from '@/composables';
import {
  emailRule,
  nullOrEmptyStringRule,
  passwordCharsetRule,
  passwordLengthRule,
} from '@/consts/rules';
import type {
  RegistrationFormErrors,
  RegistrationFormFields,
} from '@/types/auth';
import { reactive } from 'vue';

export function useRegistrationFormFields() {
  const model = reactive<RegistrationFormFields>({
    name: '',
    surname: '',
    email: '',
    password: '',
    passwordConfirm: '',
  });

  const errors = reactive<RegistrationFormErrors>({
    name: '',
    surname: '',
    email: '',
    password: '',
    passwordConfirm: '',
  });

  const nameValidator = useFieldValidator({
    value: () => model.name,
    rules: [nullOrEmptyStringRule],
    errorName: 'name',
    errors,
  });

  const surnameValidator = useFieldValidator({
    value: () => model.surname,
    rules: [nullOrEmptyStringRule],
    errorName: 'surname',
    errors,
  });

  const emailValidator = useFieldValidator({
    value: () => model.email,
    rules: [nullOrEmptyStringRule, emailRule],
    errorName: 'email',
    errors,
  });

  const passwordValidator = useFieldValidator({
    value: () => model.password,
    rules: [nullOrEmptyStringRule, passwordLengthRule, passwordCharsetRule],
    errorName: 'password',
    errors,
  });

  const passwordConfirmValidator = useFieldValidator({
    value: () => model.passwordConfirm,
    rules: [
      nullOrEmptyStringRule,
      (val) => val === model.password || 'Паролі не співпадають',
    ],
    errorName: 'passwordConfirm',
    errors,
  });

  const validateFields = () => {
    const results = [
      nameValidator.validate(),
      surnameValidator.validate(),
      emailValidator.validate(),
      passwordValidator.validate(),
      passwordConfirmValidator.validate(),
    ];
    return results.every((x) => x);
  };

  return {
    model,
    errors,
    validateFields,
  };
}
