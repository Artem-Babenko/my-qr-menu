<script setup lang="ts">
  import { AppText, AppCard } from '@/components/shared';
  import { useUserStore } from '@/store/user';
  import { storeToRefs } from 'pinia';
  import { CardDropdown, type ActionButton } from '../dropdowns';
  import { computed } from 'vue';
  import { useThemeStore } from '@/store/theme';
  import { useRouter } from 'vue-router';
  import { ROUTES } from '@/router';
  import { useAuthStore } from '@/store/auth';

  const userStore = useUserStore();
  const authStore = useAuthStore();
  const themeStore = useThemeStore();
  const router = useRouter();

  const { user } = storeToRefs(userStore);
  const isDarkTheme = computed(() => themeStore.theme === 'dark');

  const buttons = computed<ActionButton[]>(() => [
    {
      icon: isDarkTheme.value ? 'Sun' : 'Moon',
      title: isDarkTheme.value ? 'Світла тема' : 'Темна тема',
      click: () => {
        themeStore.theme = isDarkTheme.value ? 'light' : 'dark';
      },
    },
    {
      icon: 'LogOut',
      title: 'Вийти',
      click: async () => {
        authStore.setToken(null);
        await router.replace({ name: ROUTES.login });
      },
    },
  ]);
</script>

<template>
  <app-card class="nav-user-info" v-if="user">
    <app-text weight="500">{{ user.name }} {{ user.surname }}</app-text>
    <card-dropdown :buttons="buttons"></card-dropdown>
  </app-card>
</template>

<style scoped>
  .nav-user-info {
    display: flex;
    align-items: center;
    gap: 15px;
    padding: 5px 15px;
    overflow: hidden;
  }
  .app-text {
    overflow: hidden;
    text-overflow: ellipsis;
  }
</style>
