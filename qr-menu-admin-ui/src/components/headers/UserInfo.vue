<script setup lang="ts">
  import {
    AppCard,
    AppText,
  } from '@/components/shared';
  import { useUserStore } from '@/store/user';
  import { storeToRefs } from 'pinia';
  import { CardDropdown, type ActionButton } from '../dropdowns';
  import { computed } from 'vue';
  import { useThemeStore } from '@/store/theme';
  import { useRouter } from 'vue-router';
  import { ROUTES } from '@/router';
  import { useAuthStore } from '@/store/auth';
  import { UserInitials } from '../other';

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
  <template v-if="user">
    <app-card class="desktop-info">
      <app-text weight="500">{{ user.name }} {{ user.surname }}</app-text>
      <card-dropdown :buttons="buttons"></card-dropdown>
    </app-card>

    <div class="mobile-info">
      <card-dropdown :buttons="buttons">
        <template #target>
          <button
            class="avatar-btn"
            type="button"
            aria-label="Відкрити меню користувача"
          >
            <user-initials
              :name="user.name"
              :surname="user.surname"
            ></user-initials>
          </button>
        </template>
      </card-dropdown>
    </div>
  </template>
</template>

<style scoped>
  .desktop-info {
    display: flex;
    align-items: center;
    gap: 15px;
    padding: 5px 15px;
    overflow: hidden;
    max-width: 320px;
    flex-shrink: 0;
  }
  .desktop-info .app-text {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .mobile-info {
    display: none;
  }
  .avatar-btn {
    border: 0;
    background: transparent;
    padding: 0;
    display: inline-flex;
    border-radius: 50%;
    cursor: pointer;
  }
  .avatar-btn:focus-visible {
    outline: 2px solid var(--primary);
    outline-offset: 2px;
  }

  @media (max-width: 640px) {
    .desktop-info {
      display: none;
    }
    .mobile-info {
      display: block;
    }
  }
</style>
