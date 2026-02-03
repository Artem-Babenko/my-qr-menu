<script setup lang="ts">
  import { UserInitials } from '@/components/other';
  import { AppText } from '@/components/shared';
  import { useUserStore } from '@/store/user';
  import { storeToRefs } from 'pinia';

  const userStore = useUserStore();
  const { user } = storeToRefs(userStore);
</script>

<template>
  <div class="nav-user-info" v-if="user">
    <user-initials :name="user.name" :surname="user.surname"></user-initials>
    <div class="info-inner">
      <app-text weight="500">{{ user.name }} {{ user.surname }}</app-text>
      <app-text size="xs" color="secondary">{{ user.email }}</app-text>
    </div>
  </div>
</template>

<style scoped>
  .nav-user-info {
    display: flex;
    align-items: center;
    gap: 15px;
    padding: 15px;
    border-top: 1px solid var(--border);
    overflow: hidden;
  }
  .info-inner {
    display: flex;
    flex-direction: column;
    gap: 4px;
    min-width: 0;
  }
  .app-text {
    overflow: hidden;
    text-overflow: ellipsis;
  }
</style>
