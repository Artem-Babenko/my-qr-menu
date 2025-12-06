<script lang="ts" setup>
import { networkApi } from '@/api/networkApi';
import { AppButton, AppInput, AppText } from '@/components/shared';
import { ROUTES } from '@/router';
import { useUserStore } from '@/store/user';
import type { CreateEstablishmentReq } from '@/types/network';
import { reactive } from 'vue';
import { useRouter } from 'vue-router';

const userStore = useUserStore();
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
  const resp = await networkApi.createEstablishment(req);

  userStore.user!.networkId = resp.data.networkId;
  router.replace({ name: ROUTES.dashboard });
};
</script>

<template>
  <div class="page">
    <app-text weight="600" size="m">Створення закладу харчування</app-text>
    <app-input
      v-model="model.name"
      label="Назва"
      placeholder="Введіть повну назву"
    ></app-input>
    <app-input
      v-model="model.address"
      label="Адреса"
      placeholder="Введіть фізичну адресу"
    ></app-input>
    <app-button @click="create">Створити</app-button>
  </div>
</template>

<style scoped>
.page {
  display: flex;
  align-items: center;
  flex-direction: column;
  justify-content: center;
  gap: 10px;
  max-width: 500px;
  margin: 0 auto;
  height: 100vh;
}
.app-input {
  width: 250px;
}
</style>
