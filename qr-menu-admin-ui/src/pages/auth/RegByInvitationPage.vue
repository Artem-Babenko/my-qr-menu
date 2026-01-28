<script lang="ts" setup>
  import {
    useLoader,
    useRegistrationFormFields,
    useRegistrationFlow,
    useToast,
  } from '@/composables';
  import BaseAuthPage from './BaseAuthPage.vue';
  import { useRoute } from 'vue-router';
  import { computed, watchEffect } from 'vue';
  import { invitationApi } from '@/api/invitationApi';
  import {
    AppButton,
    AppErrorText,
    AppInput,
    AppLabel,
  } from '@/components/shared';
  import { getErrorMessage } from '@/consts/errorMessages';

  const route = useRoute();
  const toast = useToast();
  const { register } = useRegistrationFlow();

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

  const { model, errors, validateFields } = useRegistrationFormFields();

  watchEffect(() => {
    if (invitation.value) {
      model.name = invitation.value.name || '';
      model.surname = invitation.value.surname || '';
    }
  });

  const validate = () => {
    if (!invitation.value) return false;
    if (!invitation.value.phone) return false;
    return validateFields();
  };

  const registerByInvitation = async () => {
    if (!validate() || !invitation.value || !invitationId.value) return;

    await register({
      phone: invitation.value.phone,
      formFields: model,
      onSuccess: async () => {
        const acceptResp = await invitationApi.acceptByCurrentUser(
          invitationId.value!,
        );
        if (!acceptResp.success || !acceptResp.data) {
          toast.error(getErrorMessage(acceptResp.errorCode));
          throw new Error('Failed to accept invitation');
        }
        return { networkId: acceptResp.data.networkId };
      },
    });
  };
</script>

<template>
  <base-auth-page page-title="Реєстрація за запрошенням" v-if="invitation">
    <div class="field">
      <app-label for="phone" label="Номер телефону"></app-label>
      <app-input
        id="phone"
        :model-value="invitation.phone"
        disabled
      ></app-input>
    </div>
    <div class="field">
      <app-label for="name" label="Ім'я"></app-label>
      <app-input
        id="name"
        v-model="model.name"
        placeholder="Введіть ім'я"
      ></app-input>
      <app-error-text :message="errors.name" />
    </div>
    <div class="field">
      <app-label for="surname" label="Прізвище"></app-label>
      <app-input
        id="surname"
        v-model="model.surname"
        placeholder="Введіть прізвище"
      ></app-input>
      <app-error-text :message="errors.surname" />
    </div>
    <div class="field">
      <app-label for="email" label="Пошта"></app-label>
      <app-input
        id="email"
        v-model="model.email"
        placeholder="Введіть електронну пошту"
      ></app-input>
      <app-error-text :message="errors.email" />
    </div>
    <div class="field">
      <app-label for="password" label="Пароль"></app-label>
      <app-input
        id="password"
        v-model="model.password"
        type="password"
        placeholder="Введіть пароль"
      ></app-input>
      <app-error-text :message="errors.password" />
    </div>
    <div class="field">
      <app-label for="passwordConfirm" label="Повторіть пароль"></app-label>
      <app-input
        id="passwordConfirm"
        v-model="model.passwordConfirm"
        type="password"
        placeholder="Повторіть пароль"
      ></app-input>
      <app-error-text :message="errors.passwordConfirm" />
    </div>
    <div class="field">
      <app-label for="role" label="Роль"></app-label>
      <app-input
        id="role"
        :model-value="invitation.roleName"
        disabled
      ></app-input>
    </div>
    <div class="field">
      <app-label for="establishment" label="Заклад"></app-label>
      <app-input
        id="establishment"
        :model-value="invitation.establishmentName"
        disabled
      ></app-input>
    </div>
    <app-button @click="registerByInvitation()">Зареєструватись</app-button>
  </base-auth-page>
</template>

<style scoped>
  .field {
    margin-bottom: 12px;
    width: 100%;
  }
</style>
