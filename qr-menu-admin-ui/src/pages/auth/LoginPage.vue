<script setup lang="ts">
import { authApi } from '@/api/authApi';
import { AppButton, AppInput } from '@/components/shared';
import { PHONE_MASK } from '@/consts/masks';
import { ROUTES } from '@/router';
import { useAuthStore } from '@/store/auth';
import type { LoginReq } from '@/types/auth';
import { reactive } from 'vue';
import { useRouter } from 'vue-router';
import BaseAuthPage from './BaseAuthPage.vue';

const authStore = useAuthStore();
const router = useRouter();

const model = reactive({
  phone: '',
  password: '',
});

const login = async () => {
  const req: LoginReq = { ...model };
  const resp = await authApi.login(req);
  authStore.setToken(resp.data.token);
  router.replace({ name: ROUTES.dashboard });
};
</script>

<template>
  <base-auth-page page-title="Вхід">
    <app-input
      v-model="model.phone"
      label="Телефон"
      placeholder="Введіть номер телефону"
      :mask="PHONE_MASK"
    ></app-input>
    <app-input
      v-model="model.password"
      label="Пароль"
      placeholder="Введіть пароль"
      type="password"
    ></app-input>
    <app-button @click="login">Увійти</app-button>
    <router-link :to="{ name: ROUTES.registration }">
      <app-button type="text">Реєстрація</app-button>
    </router-link>
  </base-auth-page>
</template>

<style scoped></style>
