<script setup lang="ts">
  import { UserModal } from '@/components/modals';
  import { UserList } from '@/components/lists';
  import { useRouter } from 'vue-router';
  import { ROUTES } from '@/router';
  import type { User } from '@/types/user';
  import { ref } from 'vue';

  const router = useRouter();
  const modalShowed = ref(false);
  const editingUser = ref<User | null>(null);

  const onAddClick = () => {
    router.push({
      name: ROUTES.usersInvitations,
      query: { openInviteModal: '1' },
    });
  };

  const onEdit = (user: User) => {
    editingUser.value = user;
    modalShowed.value = true;
  };

  defineExpose({ onAddClick });
</script>

<template>
  <user-list @edit="onEdit"></user-list>
  <user-modal v-model:showed="modalShowed" :user="editingUser"></user-modal>
</template>
