<script lang="ts" setup>
  import BaseCardList from './BaseCardList.vue';
  import { UserInvitaionCard } from '../cards';
  import { invitationApi } from '@/api/invitationApi';
  import { InvitationStatus } from '@/consts/invitations';
  import { useLoader, useUserLoader } from '@/composables';
  import { useUserStore } from '@/store/user';
  import { toRef } from 'vue';
  import { NoInvitationsCard } from '../cards';
  import { useRouter } from 'vue-router';
  import { ROUTES } from '@/router';

  const userStore = useUserStore();
  const userLoader = useUserLoader();
  const router = useRouter();

  const userId = toRef(() => userStore.user?.id);

  const { data: invitations } = useLoader({
    keys: ['userInvatations', userId],
    fn: async () => {
      const resp = await invitationApi.getByCurrentUser();
      return resp.data;
    },
    enabled: () => !!userId.value,
  });

  const acceptInvitation = async (id: string) => {
    const resp = await invitationApi.acceptByCurrentUser(id);
    if (!resp.success || !resp.data) throw resp.errorCode;
    await userLoader.loadUserData();
    await router.replace({ name: ROUTES.dashboard });
  };

  const cancelInvitation = async (id: string) => {
    const resp = await invitationApi.cancel(id);
    if (!resp.success) throw resp.errorCode;
    const inv = invitations.value?.find((i) => i.id === id);
    if (inv) inv.status = InvitationStatus.canceled;
  };

  const deleteInvitation = async (id: string) => {
    const resp = await invitationApi.delete(id);
    if (!resp.success) throw resp.errorCode;
    invitations.value = invitations.value!.filter((inv) => inv.id !== id);
  };
</script>

<template>
  <no-invitations-card v-if="!invitations?.length"></no-invitations-card>
  <base-card-list v-else>
    <user-invitaion-card
      v-for="inv in invitations"
      :invitation="inv"
      @accept="acceptInvitation(inv.id)"
      @cancel="cancelInvitation(inv.id)"
      @delete="deleteInvitation(inv.id)"
    ></user-invitaion-card>
  </base-card-list>
</template>
