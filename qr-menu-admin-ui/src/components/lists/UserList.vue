<script lang="ts" setup>
  import UserCard from '../cards/UserCard.vue';
  import { useLoader } from '@/composables';
  import { useNetworkStore } from '@/store/network';
  import { toRef } from 'vue';
  import { usersApi } from '@/api/usersApi';
  import type { User } from '@/types/user';
  import BaseCardList from './BaseCardList.vue';

  const emit = defineEmits<{ edit: [user: User] }>();

  const networkStore = useNetworkStore();
  const networkId = toRef(() => networkStore.network?.id);

  const loader = useLoader({
    keys: ['users', networkId],
    fn: async () => {
      if (!networkId.value) return;
      const resp = await usersApi.byNetwork(networkId.value);
      return resp.data;
    },
    enabled: () => !!networkId.value,
  });
</script>

<template>
  <base-card-list>
    <user-card
      v-for="user in loader.data.value"
      :key="user.id"
      :user="user"
      @edit="emit('edit', user)"
    ></user-card>
  </base-card-list>
</template>
