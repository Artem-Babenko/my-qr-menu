<script lang="ts" setup>
  import { networkApi } from '@/api/networkApi';
  import {
    AppButton,
    AppCard,
    AppInput,
    AppLabel,
    AppText,
  } from '@/components/shared';
  import { useUserLoader } from '@/composables';
  import { ROUTES } from '@/router';
  import type { CreateEstablishmentReq } from '@/types/network';
  import { reactive } from 'vue';
  import { useRouter } from 'vue-router';

  const userLoader = useUserLoader();
  const router = useRouter();

  const model = reactive({
    name: '',
    address: '',
  });

  const validate = () => {
    if (!model.name) return false;
    if (!model.address) return false;
    return true;
  };

  const create = async () => {
    if (!validate()) return;

    const req: CreateEstablishmentReq = { ...model };
    await networkApi.createEstablishment(req);

    await userLoader.loadUserData();
    await router.replace({ name: ROUTES.dashboard });
  };
</script>

<template>
  <div class="page">
    <app-card class="surface">
      <app-text weight="600" size="l" class="form-head">
        Створення закладу харчування
      </app-text>
      <div class="field">
        <app-label for="name" label="Назва"></app-label>
        <app-input
          v-model="model.name"
          placeholder="Введіть повну назву"
          id="name"
        ></app-input>
      </div>
      <div class="field">
        <app-label for="address" label="Адреса"></app-label>
        <app-input
          v-model="model.address"
          placeholder="Введіть фізичну адресу"
          id="address"
        ></app-input>
      </div>
      <app-button @click="create">Створити</app-button>
    </app-card>
  </div>
</template>

<style scoped>
  .page {
    display: flex;
    align-items: center;
    flex-direction: column;
    justify-content: center;
    height: 100vh;
    background-color: var(--background);
  }
  .surface {
    display: flex;
    flex-direction: column;
    align-items: center;
  }
  .form-head {
    margin: 10px 0 30px;
    text-align: center;
  }
  .app-input {
    width: 350px;
  }
  .field {
    margin-bottom: 20px;
    width: 100%;
  }
  .app-button {
    margin-top: 10px;
  }
</style>
