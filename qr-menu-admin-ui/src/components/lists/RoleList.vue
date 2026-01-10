<script lang="ts" setup>
  import { rolesApi } from '@/api/rolesApi';
  import { useLoader } from '@/composables';
  import { useNetworkStore } from '@/store/network';
  import { toRef } from 'vue';
  import { RoleCard } from '../cards';
  import BaseCardList from './BaseCardList.vue';

  const networkStore = useNetworkStore();
  const networkId = toRef(() => networkStore.network?.id);

  const loader = useLoader({
    keys: ['roles', networkId],
    fn: async () => {
      if (!networkId.value) return;
      const resp = await rolesApi.all(networkId.value);
      return resp.data;
    },
    enabled: () => !!networkId.value,
  });
</script>

<template>
  <base-card-list>
    <role-card v-for="role in loader.data.value" :role="role"></role-card>
  </base-card-list>
</template>
