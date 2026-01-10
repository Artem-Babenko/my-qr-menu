<script setup lang="ts">
import { authApi } from '@/api/authApi';
import { AppButton, AppInput } from '@/components/shared';
import { PHONE_MASK } from '@/consts/masks';
import { ROUTES } from '@/router';
import { useAuthStore } from '@/store/auth';
import { useUserStore } from '@/store/user';
import type { LoginReq, RegistrationReq } from '@/types/auth';
import { reactive } from 'vue';
import { useRouter } from 'vue-router';
import BaseAuthPage from './BaseAuthPage.vue';

const userStore = useUserStore();
const authStore = useAuthStore();
const router = useRouter();

const model = reactive({
  phone: '',
  name: '',
  surname: '',
  password: '',
  email: '',
});

const validate = () => {
  if (!model.phone) return false;
  if (!model.name) return false;
  if (!model.surname) return false;
  if (!model.password) return false;
  if (!model.email) return false;
  return true;
};

const registrate = async () => {
  if (!validate()) return;

  const regReq: RegistrationReq = {
    ...model,
  };
  const regResp = await authApi.reg(regReq);

  const logingReq: LoginReq = {
    phone: regReq.phone,
    password: regReq.password,
  };
  const logingResp = await authApi.login(logingReq);

  authStore.setToken(logingResp.data!.token);
  userStore.user = { ...regReq, id: regResp.data!.userId, networkId: null };
  router.replace({ name: ROUTES.dashboard });
};
</script>

<template>
  <base-auth-page page-title="Реєстрація">
    <app-input
      v-model="model.phone"
      label="Телефон"
      placeholder="Введіть номер телефону"
      :mask="PHONE_MASK"
    ></app-input>
    <app-input
      v-model="model.name"
      label="Ім'я"
      placeholder="Введіть ім'я"
    ></app-input>
    <app-input
      v-model="model.surname"
      label="Прізвище"
      placeholder="Введіть прізвище"
    ></app-input>
    <app-input
      v-model="model.email"
      label="Пошта"
      placeholder="Введіть електронну пошту"
    ></app-input>
    <app-input
      v-model="model.password"
      label="Пароль"
      placeholder="Введіть пароль"
      type="password"
    ></app-input>
    <app-button @click="registrate()">Зареєструватись</app-button>
    <router-link :to="{ name: ROUTES.login }">
      <app-button type="text">Вхід</app-button>
    </router-link>
  </base-auth-page>
</template>

<style scoped></style>
