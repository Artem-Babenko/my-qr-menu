<script setup lang="ts">
  import { UserInitials } from '@/components/other';
  import { AppText } from '@/components/shared';

  const user = {
    initials: 'АД',
    name: 'Артем',
    surname: 'Бабенко',
    email: 'admin@restaurant.com',
  };
</script>

<template>
  <div class="nav-user-info">
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
  }
  .info-inner {
    display: flex;
    flex-direction: column;
    gap: 4px;
  }
</style>
