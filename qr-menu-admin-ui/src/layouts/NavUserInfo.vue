<script setup lang="ts">
import { AppText } from '@/components/shared';

const user = {
  initials: 'АД',
  name: 'Адміністратор',
  email: 'admin@restaurant.com',
};
</script>

<template>
  <div class="nav-user-info">
    <app-text class="user-img" weight="500">{{ user.initials }}</app-text>
    <div class="info-inner">
      <app-text weight="500">{{ user.name }}</app-text>
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

.user-img {
  border-radius: 50%;
  width: 40px;
  height: 40px;
  background-color: var(--hover-on-secondary);
  display: flex;
  align-items: center;
  justify-content: center;
}

.info-inner {
  display: flex;
  flex-direction: column;
  gap: 4px;
}
</style>
