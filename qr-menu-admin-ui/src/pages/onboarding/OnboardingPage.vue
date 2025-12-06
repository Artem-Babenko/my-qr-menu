<script lang="ts" setup>
import { AppButton, AppText } from '@/components/shared';
import { ROUTES } from '@/router';
import { useUserStore } from '@/store/user';

const userStore = useUserStore();
</script>

<template>
  <div class="page">
    <app-text weight="600" size="m">
      Вітаємо, {{ userStore.user?.name }}!
    </app-text>
    <app-text class="description">
      Ваш акаунт успішно створено. Наразі ви не додані до жодного закладу
      харчування. Очікуйте, поки адміністратор вас додасть, або створіть власний
      заклад.
    </app-text>
    <router-link :to="{ name: ROUTES.createEstablishment }">
      <app-button>Створити заклад харчування</app-button>
    </router-link>
  </div>
</template>

<style scoped>
.page {
  display: flex;
  align-items: center;
  justify-content: center;
  flex-direction: column;
  gap: 20px;
  height: 100vh;
}
.app-text {
  max-width: 450px;
}
.description {
  line-height: 1.5;
  text-align: center;
}
</style>
