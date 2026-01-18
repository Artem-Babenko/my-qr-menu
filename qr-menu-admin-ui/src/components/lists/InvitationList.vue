<script lang="ts" setup>
  import { useLoader } from '@/composables';
  import BaseCardList from './BaseCardList.vue';
  import { useNetworkStore } from '@/store/network';
  import { toRef } from 'vue';
  import { invitationApi } from '@/api/invitationApi';
  import { InvitationCard } from '../cards';

  const networkStore = useNetworkStore();
  const networkId = toRef(() => networkStore.network?.id);

  const loader = useLoader({
    keys: ['invations', networkId],
    fn: async () => {
      if (!networkId.value) return;
      const resp = await invitationApi.getByNetwork(networkId.value);
      if (!resp.success || !resp.data) throw resp.errorCode;
      return resp.data;
    },
    enabled: () => !!networkId.value,
  });
</script>

<template>
  <base-card-list>
    <invitation-card
      v-for="inv in loader.data.value"
      :key="inv.id"
      :invitation="inv"
    ></invitation-card>
  </base-card-list>
</template>
