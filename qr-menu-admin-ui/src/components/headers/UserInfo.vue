<script setup lang="ts">
  import {
    AppButton,
    AppCard,
    AppDropdown,
    AppIcon,
    AppText,
  } from '@/components/shared';
  import { useUserStore } from '@/store/user';
  import { storeToRefs } from 'pinia';
  import { CardDropdown, type ActionButton } from '../dropdowns';
  import { computed, ref } from 'vue';
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
  const menuOpen = ref(false);

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

  const onClick = async (button: ActionButton) => {
    if (button.disabled) {
      if (typeof button.disabled === 'function' && button.disabled()) return;
      if (typeof button.disabled === 'boolean' && button.disabled) return;
    }
    button.click();
    menuOpen.value = false;
  };
</script>

<template>
  <template v-if="user">
    <app-card class="desktop-info">
      <app-text weight="500">{{ user.name }} {{ user.surname }}</app-text>
      <card-dropdown :buttons="buttons"></card-dropdown>
    </app-card>

    <div class="mobile-info">
      <app-dropdown
        v-model:open="menuOpen"
        anchor="bottom right"
        self="top right"
        :offset="[0, 8]"
      >
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
        <template #default>
          <div class="menu-content">
            <app-button
              v-for="button in buttons"
              :key="button.title"
              type="text"
              @click="onClick(button)"
            >
              <app-icon :name="button.icon" :size="14"></app-icon>
              {{ button.title }}
            </app-button>
          </div>
        </template>
      </app-dropdown>
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
  .menu-content {
    display: flex;
    flex-direction: column;
    gap: 3px;
  }
  .menu-content :deep(.app-button) {
    text-align: left;
    padding: 5px 10px;
    gap: 10px;
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
