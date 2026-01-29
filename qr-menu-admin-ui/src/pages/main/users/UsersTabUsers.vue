<script setup lang="ts">
  import { UserList } from '@/components/lists';
  import { useRouter } from 'vue-router';
  import { ROUTES } from '@/router';

  const router = useRouter();

  const onAddClick = () => {
    router.push({
      name: ROUTES.usersInvitations,
      query: { openInviteModal: '1' },
    });
  };

  defineExpose({ onAddClick });
</script>

<template>
  <user-list></user-list>
</template>
