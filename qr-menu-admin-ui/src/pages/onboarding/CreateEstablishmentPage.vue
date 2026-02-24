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
    min-height: 100dvh;
    background-color: var(--background);
    padding: 16px;
    box-sizing: border-box;
  }
  .surface {
    display: flex;
    flex-direction: column;
    align-items: center;
    width: min(100%, 520px);
    box-sizing: border-box;
  }
  .form-head {
    margin: 10px 0 30px;
    text-align: center;
  }
  .field {
    margin-bottom: 20px;
    width: 100%;
  }
  :deep(.field .app-input) {
    width: 100%;
  }
  :deep(.app-button) {
    margin-top: 10px;
  }

  @media (max-width: 768px) {
    .page {
      justify-content: flex-start;
      padding: 12px;
    }
    .surface {
      width: 100%;
      padding: 14px;
    }
    .form-head {
      margin: 4px 0 18px;
      font-size: 18px;
    }
    .field {
      margin-bottom: 12px;
    }
    :deep(.app-button) {
      width: 100%;
      justify-content: center;
    }
  }
</style>
