<script lang="ts" setup>
  import { UserInvitationList } from '@/components/lists';
  import { AppButton, AppText, AppCard } from '@/components/shared';
  import { ROUTES } from '@/router';
  import { useUserStore } from '@/store/user';

  const userStore = useUserStore();
</script>

<template>
  <div class="page">
    <app-card class="surface">
      <app-text weight="600" size="l">
        Вітаємо, {{ userStore.user?.name }}!
      </app-text>
      <app-text class="description" color="secondary">
        Ваш акаунт успішно створено. Наразі ви не додані до жодного закладу
        харчування. Очікуйте, поки адміністратор вас додасть, або створіть
        власний заклад.
      </app-text>
      <router-link :to="{ name: ROUTES.createEstablishment }">
        <app-button>Створити заклад харчування</app-button>
      </router-link>
    </app-card>
    <user-invitation-list></user-invitation-list>
  </div>
</template>

<style scoped>
  .page {
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: column;
    background-color: var(--background);
    gap: 20px;
    min-height: 100dvh;
    padding: 16px;
    box-sizing: border-box;
  }
  .surface {
    padding: 30px 20px;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 20px;
    width: min(100%, 720px);
    box-sizing: border-box;
  }
  .app-text {
    max-width: 450px;
  }
  .description {
    line-height: 1.5;
    text-align: center;
  }

  @media (max-width: 768px) {
    .page {
      align-items: stretch;
      justify-content: flex-start;
      gap: 12px;
      padding: 12px;
    }
    .surface {
      padding: 18px 14px;
      gap: 14px;
      width: 100%;
      align-items: flex-start;
    }
    .app-text {
      max-width: 100%;
    }
    .description {
      text-align: left;
    }
    .surface :deep(.app-button) {
      width: 100%;
      justify-content: center;
    }
  }
</style>
