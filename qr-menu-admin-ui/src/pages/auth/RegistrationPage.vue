<script setup lang="ts">
  import {
    AppButton,
    AppErrorText,
    AppInput,
    AppLabel,
  } from '@/components/shared';
  import {
    useRegistrationFormFields,
    useRegistrationFlow,
    useFieldValidator,
  } from '@/composables';
  import { PHONE_MASK } from '@/consts/masks';
  import { nullOrEmptyStringRule, phoneLengthRule } from '@/consts/rules';
  import { ROUTES } from '@/router';
  import { reactive } from 'vue';
  import BaseAuthPage from './BaseAuthPage.vue';

  const { model, errors, validateFields } = useRegistrationFormFields();
  const { register } = useRegistrationFlow();

  const phoneModel = reactive({ phone: '' });
  const phoneErrors = reactive({ phone: '' });

  const phoneValidator = useFieldValidator({
    value: () => phoneModel.phone,
    rules: [nullOrEmptyStringRule, phoneLengthRule],
    errorName: 'phone',
    errors: phoneErrors,
  });

  const validate = () => {
    return phoneValidator.validate() && validateFields();
  };

  const registrate = async () => {
    if (!validate()) return;

    await register({
      phone: phoneModel.phone,
      formFields: model,
    });
  };
</script>

<template>
  <base-auth-page page-title="Реєстрація">
    <div class="field">
      <app-label for="phone" label="Телефон"></app-label>
      <app-input
        v-model="phoneModel.phone"
        placeholder="Введіть номер телефону"
        :mask="PHONE_MASK"
        id="phone"
      ></app-input>
      <app-error-text :message="phoneErrors.phone" />
    </div>
    <div class="field">
      <app-label for="name" label="Ім'я"></app-label>
      <app-input
        v-model="model.name"
        label="Ім'я"
        placeholder="Введіть ім'я"
        id="name"
      ></app-input>
      <app-error-text :message="errors.name" />
    </div>
    <div class="field">
      <app-label for="surname" label="Прізвище"></app-label>
      <app-input
        v-model="model.surname"
        label="Прізвище"
        placeholder="Введіть прізвище"
        id="surname"
      ></app-input>
      <app-error-text :message="errors.surname" />
    </div>
    <div class="field">
      <app-label for="email" label="Пошта"></app-label>
      <app-input
        v-model="model.email"
        label="Пошта"
        placeholder="Введіть електронну пошту"
        id="email"
      ></app-input>
      <app-error-text :message="errors.email" />
    </div>
    <div class="field">
      <app-label for="password" label="Пароль"></app-label>
      <app-input
        v-model="model.password"
        label="Пароль"
        placeholder="Введіть пароль"
        type="password"
        id="password"
      ></app-input>
      <app-error-text :message="errors.password" />
    </div>
    <div class="field">
      <app-label for="passwordConfirm" label="Повторіть пароль"></app-label>
      <app-input
        v-model="model.passwordConfirm"
        label="Повторіть пароль"
        placeholder="Повторіть пароль"
        type="password"
        id="passwordConfirm"
      ></app-input>
      <app-error-text :message="errors.passwordConfirm" />
    </div>
    <app-button @click="registrate()">Зареєструватись</app-button>
    <router-link :to="{ name: ROUTES.login }">
      <app-button type="text">Вхід</app-button>
    </router-link>
  </base-auth-page>
</template>

<style scoped>
  .field {
    margin-bottom: 12px;
    width: 100%;
  }
</style>
