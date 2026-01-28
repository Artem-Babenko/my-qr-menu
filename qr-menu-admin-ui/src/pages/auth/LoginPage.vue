<script setup lang="ts">
  import { authApi } from '@/api/authApi';
  import {
    AppButton,
    AppErrorText,
    AppInput,
    AppLabel,
  } from '@/components/shared';
  import { useFieldValidator, useToast } from '@/composables';
  import { PHONE_MASK } from '@/consts/masks';
  import { nullOrEmptyStringRule, phoneLengthRule } from '@/consts/rules';
  import { getErrorMessage } from '@/consts/errorMessages';
  import { ROUTES } from '@/router';
  import { useAuthStore } from '@/store/auth';
  import type { LoginReq } from '@/types/auth';
  import { reactive } from 'vue';
  import { useRouter } from 'vue-router';
  import BaseAuthPage from './BaseAuthPage.vue';

  const authStore = useAuthStore();
  const router = useRouter();
  const toast = useToast();

  const model = reactive({
    phone: '',
    password: '',
  });

  const errors = reactive({
    phone: '',
    password: '',
  });

  const phoneValidator = useFieldValidator({
    value: () => model.phone,
    rules: [nullOrEmptyStringRule, phoneLengthRule],
    errorName: 'phone',
    errors,
  });

  const passwordValidator = useFieldValidator({
    value: () => model.password,
    rules: [nullOrEmptyStringRule],
    errorName: 'password',
    errors,
  });

  const validate = () => {
    const results = [phoneValidator.validate(), passwordValidator.validate()];
    return results.every((x) => x);
  };

  const login = async () => {
    if (!validate()) return;
    const req: LoginReq = { ...model };
    const resp = await authApi.login(req);
    if (!resp.success || !resp.data) {
      toast.error(getErrorMessage(resp.errorCode));
      return;
    }
    authStore.setToken(resp.data.token);
    router.replace({ name: ROUTES.dashboard });
  };
</script>

<template>
  <base-auth-page page-title="Вхід">
    <div class="field">
      <app-label for="phone" label="Телефон"></app-label>
      <app-input
        v-model="model.phone"
        placeholder="Введіть номер телефону"
        :mask="PHONE_MASK"
        id="phone"
      ></app-input>
      <app-error-text :message="errors.phone" />
    </div>
    <div class="field">
      <app-label for="password" label="Пароль"></app-label>
      <app-input
        v-model="model.password"
        placeholder="Введіть пароль"
        type="password"
        id="password"
      ></app-input>
      <app-error-text :message="errors.password" />
    </div>
    <app-button @click="login">Увійти</app-button>
    <router-link :to="{ name: ROUTES.registration }">
      <app-button type="text">Реєстрація</app-button>
    </router-link>
  </base-auth-page>
</template>

<style scoped>
  .field {
    margin-bottom: 12px;
    width: 100%;
  }
</style>
