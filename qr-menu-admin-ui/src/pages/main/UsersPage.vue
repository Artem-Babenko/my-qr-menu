<script lang="ts" setup>
  import { computed, ref, useTemplateRef } from 'vue';
  import { UsersPageHeader } from '@/components/headers';
  import { UserPageTab } from '@/consts/tabs';
  import { useRoute, useRouter } from 'vue-router';
  import { ROUTES } from '@/router';

  const route = useRoute();
  const router = useRouter();

  const activeTabRef = useTemplateRef<{ onAddClick?: () => unknown }>(
    'activeTabRef',
  );

  const searchWord = ref('');

  const routeToTabMap: Record<string, UserPageTab> = {
    [ROUTES.users]: UserPageTab.users,
    [ROUTES.usersRoles]: UserPageTab.roles,
    [ROUTES.usersInvitations]: UserPageTab.invites,
  };

  const tabToRouteMap: Record<UserPageTab, string> = {
    [UserPageTab.users]: ROUTES.users,
    [UserPageTab.roles]: ROUTES.usersRoles,
    [UserPageTab.invites]: ROUTES.usersInvitations,
  };

  const selectedTab = computed<UserPageTab>({
    get() {
      return routeToTabMap[route.name as string] ?? UserPageTab.users;
    },
    set(tab) {
      router.push({ name: tabToRouteMap[tab] });
    },
  });

  const onAddButtonClick = () => {
    activeTabRef.value?.onAddClick?.();
  };
</script>

<template>
  <div class="page">
    <users-page-header
      v-model:search="searchWord"
      v-model:selected-tab="selectedTab"
      @add-button-click="onAddButtonClick"
    ></users-page-header>
    <div class="main">
      <router-view v-slot="{ Component }">
        <component :is="Component" ref="activeTabRef"></component>
      </router-view>
    </div>
  </div>
</template>
