<script lang="ts" setup>
  import { useLoader } from '@/composables';
  import BaseAuthPage from './BaseAuthPage.vue';
  import { useRoute, useRouter } from 'vue-router';
  import { computed, reactive, watch, watchEffect } from 'vue';
  import { invitationApi } from '@/api/invitationApi';
  import { authApi } from '@/api/authApi';
  import { AppInput, AppLabel, AppButton } from '@/components/shared';
  import { ROUTES } from '@/router';
  import { useAuthStore } from '@/store/auth';
  import { useUserStore } from '@/store/user';

  const route = useRoute();
  const router = useRouter();
  const authStore = useAuthStore();
  const userStore = useUserStore();

  const invitationId = computed<string | null>(() => {
    const id = route.params.invitationId;
    if (typeof id !== 'string') return null;
    return id;
  });

  const { data: invitation } = useLoader({
    keys: ['invitation', invitationId],
    fn: async () => {
      if (!invitationId.value) return;
      const resp = await invitationApi.get(invitationId.value);
      if (!resp.success || !resp.data) throw resp.errorCode;
      return resp.data;
    },
    enabled: () => !!invitationId.value,
  });

  const model = reactive({
    name: '',
    surname: '',
    email: '',
    password: '',
  });

  watchEffect(() => {
    if (invitation.value) {
      model.name = invitation.value.name || '';
      model.surname = invitation.value.surname || '';
    }
  });

  const validate = () => {
    if (!invitation.value) return false;
    if (!invitation.value.phone) return false;
    if (!model.name) return false;
    if (!model.surname) return false;
    if (!model.email) return false;
    if (!model.password) return false;
    return true;
  };

  const registerByInvitation = async () => {
    if (!validate() || !invitation.value || !invitationId.value) return;

    const regResp = await authApi.reg({
      phone: invitation.value.phone,
      name: model.name,
      surname: model.surname,
      email: model.email,
      password: model.password,
    });
    if (!regResp.success || !regResp.data) return;

    const loginResp = await authApi.login({
      phone: invitation.value.phone,
      password: model.password,
    });
    if (!loginResp.success || !loginResp.data) return;

    authStore.setToken(loginResp.data.token);

    const acceptResp = await invitationApi.acceptByCurrentUser(
      invitationId.value,
    );
    if (!acceptResp.success || !acceptResp.data) return;

    userStore.user = {
      id: regResp.data.userId,
      name: model.name,
      surname: model.surname,
      email: model.email,
      phone: invitation.value.phone,
      networkId: acceptResp.data.networkId,
    };

    router.replace({ name: ROUTES.dashboard });
  };
</script>

<template>
  <base-auth-page page-title="Реєстрація за запрошенням" v-if="invitation">
    <app-label for="phone" label="Номер телефону"></app-label>
    <app-input id="phone" :model-value="invitation.phone" disabled></app-input>
    <app-label for="name" label="Ім'я"></app-label>
    <app-input
      id="name"
      v-model="model.name"
      placeholder="Введіть ім'я"
    ></app-input>
    <app-label for="surname" label="Прізвище"></app-label>
    <app-input
      id="surname"
      v-model="model.surname"
      placeholder="Введіть прізвище"
    ></app-input>
    <app-label for="email" label="Пошта"></app-label>
    <app-input
      id="email"
      v-model="model.email"
      placeholder="Введіть електронну пошту"
    ></app-input>
    <app-label for="password" label="Пароль"></app-label>
    <app-input
      id="password"
      v-model="model.password"
      type="password"
      placeholder="Введіть пароль"
    ></app-input>
    <app-label for="role" label="Роль"></app-label>
    <app-input
      id="role"
      :model-value="invitation.roleName"
      disabled
    ></app-input>
    <app-label for="establishment" label="Заклад"></app-label>
    <app-input
      id="establishment"
      :model-value="invitation.establishmentName"
      disabled
    ></app-input>
    <app-button @click="registerByInvitation()">Зареєструватись</app-button>
  </base-auth-page>
</template>
