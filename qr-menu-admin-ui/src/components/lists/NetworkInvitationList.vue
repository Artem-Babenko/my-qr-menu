<script lang="ts" setup>
  import { useLoader } from '@/composables';
  import BaseCardList from './BaseCardList.vue';
  import { useNetworkStore } from '@/store/network';
  import { toRef } from 'vue';
  import { invitationApi } from '@/api/invitationApi';
  import { InvitationCard } from '../cards';
  import { InvitationModal } from '../modals';

  const modalShowed = defineModel<boolean>('modalShowed', { required: true });
  const networkStore = useNetworkStore();
  const networkId = toRef(() => networkStore.network?.id);

  const { data: invitations, refetch } = useLoader({
    keys: ['invations', networkId],
    fn: async () => {
      if (!networkId.value) return;
      const resp = await invitationApi.getByNetwork(networkId.value);
      if (!resp.success || !resp.data) throw resp.errorCode;
      return resp.data;
    },
    enabled: () => !!networkId.value,
  });

  const deleteInvitation = async (id: string) => {
    if (!invitations.value) return;
    const resp = await invitationApi.delete(id);
    if (!resp.success) throw new Error('Error while deliting invation');
    invitations.value = invitations.value.filter((inv) => inv.id !== id);
  };
</script>

<template>
  <base-card-list>
    <invitation-card
      v-for="inv in invitations"
      :key="inv.id"
      :invitation="inv"
      @delete="deleteInvitation(inv.id)"
    ></invitation-card>

    <invitation-modal
      v-model:showed="modalShowed"
      @save="refetch()"
    ></invitation-modal>
  </base-card-list>
</template>
