<script lang="ts" setup>
  import UserCard from '../cards/UserCard.vue';
  import { useLoader } from '@/composables';
  import { useNetworkStore } from '@/store/network';
  import { toRef } from 'vue';
  import { usersApi } from '@/api/usersApi';
  import type { User } from '@/types/user';
  import BaseCardList from './BaseCardList.vue';
  import { AppText } from '@/components/shared';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';

  const emit = defineEmits<{ edit: [user: User] }>();

  const networkStore = useNetworkStore();
  const networkId = toRef(() => networkStore.network?.id);

  const { hasAny } = usePermissions();
  const canView = () => hasAny(PermissionType.usersView);

  const loader = useLoader({
    keys: ['users', networkId],
    fn: async () => {
      if (!networkId.value) return;
      const resp = await usersApi.byNetwork(networkId.value);
      if (!resp.success) return [];
      return resp.data ?? [];
    },
    enabled: () => !!networkId.value && canView(),
  });
</script>

<template>
  <div v-if="!canView()">
    <app-text color="secondary">
      Недостатньо прав для перегляду користувачів
    </app-text>
  </div>
  <base-card-list>
    <user-card
      v-for="user in loader.data.value"
      :key="user.id"
      :user="user"
      @edit="emit('edit', user)"
    ></user-card>
  </base-card-list>
</template>
