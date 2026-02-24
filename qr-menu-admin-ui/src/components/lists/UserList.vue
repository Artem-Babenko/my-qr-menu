<script lang="ts" setup>
  import UserCard from '../cards/UserCard.vue';
  import { useLoader } from '@/composables';
  import { useNetworkStore } from '@/store/network';
  import { computed, toRef } from 'vue';
  import { usersApi } from '@/api/usersApi';
  import type { User } from '@/types/user';
  import BaseCardList from './BaseCardList.vue';
  import { AppText } from '@/components/shared';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';

  const props = withDefaults(defineProps<{ search?: string }>(), {
    search: '',
  });

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

  const filteredUsers = computed(() => {
    const list = loader.data.value ?? [];
    const q = props.search.trim().toLowerCase();
    if (!q) return list;
    return list.filter((user) => {
      const hay =
        `${user.name} ${user.surname} ${user.email} ${user.phone}`.toLowerCase();
      return hay.includes(q);
    });
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
      v-for="user in filteredUsers"
      :key="user.id"
      :user="user"
      @edit="emit('edit', user)"
    ></user-card>
    <app-text v-if="canView() && filteredUsers.length === 0" color="secondary">
      Нічого не знайдено
    </app-text>
  </base-card-list>
</template>
