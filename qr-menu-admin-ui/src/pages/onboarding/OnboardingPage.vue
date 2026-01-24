<script lang="ts" setup>
  import { invitationApi } from '@/api/invitationApi';
  import { NoInvitationsCard } from '@/components/cards';
  import { UserInvitationList } from '@/components/lists';
  import { AppButton, AppText } from '@/components/shared';
  import { useLoader } from '@/composables';
  import { ROUTES } from '@/router';
  import { useUserStore } from '@/store/user';
  import { toRef } from 'vue';

  const userStore = useUserStore();
  const userId = toRef(() => userStore.user?.id);

  const { data: invations } = useLoader({
    keys: ['userInvatations', userId],
    fn: async () => {
      const resp = await invitationApi.getByCurrentUser();
      return resp.data;
    },
    enabled: () => !!userId.value,
  });
</script>

<template>
  <div class="page">
    <app-text weight="600" size="l">
      Вітаємо, {{ userStore.user?.name }}!
    </app-text>
    <app-text class="description" color="secondary">
      Ваш акаунт успішно створено. Наразі ви не додані до жодного закладу
      харчування. Очікуйте, поки адміністратор вас додасть, або створіть власний
      заклад.
    </app-text>
    <router-link :to="{ name: ROUTES.createEstablishment }">
      <app-button>Створити заклад харчування</app-button>
    </router-link>
    <no-invitations-card v-if="!invations?.length"></no-invitations-card>
    <user-invitation-list
      v-else
      :invitations="invations"
    ></user-invitation-list>
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
