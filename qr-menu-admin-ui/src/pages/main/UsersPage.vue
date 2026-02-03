<script lang="ts" setup>
  import { computed, onMounted, ref, useTemplateRef } from 'vue';
  import { UsersPageHeader } from '@/components/headers';
  import { UserPageTab } from '@/consts/tabs';
  import { useRoute, useRouter } from 'vue-router';
  import { ROUTES } from '@/router';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';
  import type { AppTab } from '@/types/components';

  const route = useRoute();
  const router = useRouter();
  const { hasAny } = usePermissions();

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

  const canUsers = computed(() => hasAny(PermissionType.usersView));
  const canRoles = computed(() => hasAny(PermissionType.rolesView));
  const canInvites = computed(() => hasAny(PermissionType.invitationsView));

  const tabs = computed<AppTab[]>(() => {
    const res: AppTab[] = [];
    if (canUsers.value)
      res.push({ id: UserPageTab.users, title: 'Користувачі' });
    if (canRoles.value) res.push({ id: UserPageTab.roles, title: 'Ролі' });
    if (canInvites.value)
      res.push({ id: UserPageTab.invites, title: 'Запрошення' });
    return res;
  });

  const firstAllowedTab = computed<UserPageTab | null>(() => {
    return (tabs.value[0]?.id as UserPageTab | undefined) ?? null;
  });

  const selectedTab = computed<UserPageTab>({
    get() {
      const fromRoute =
        routeToTabMap[route.name as string] ?? UserPageTab.users;
      const allowed = new Set(tabs.value.map((t) => t.id as UserPageTab));
      if (allowed.size === 0) return UserPageTab.users;
      return allowed.has(fromRoute)
        ? fromRoute
        : (firstAllowedTab.value as UserPageTab);
    },
    set(tab) {
      const allowed = new Set(tabs.value.map((t) => t.id as UserPageTab));
      if (!allowed.has(tab)) return;
      router.push({ name: tabToRouteMap[tab] });
    },
  });

  const addDisabled = computed(() => {
    const canCreateInvite = hasAny(PermissionType.invitationsCreate);
    const canCreateRole = hasAny(PermissionType.rolesCreate);
    if (selectedTab.value === UserPageTab.users) return !canCreateInvite;
    if (selectedTab.value === UserPageTab.invites) return !canCreateInvite;
    if (selectedTab.value === UserPageTab.roles) return !canCreateRole;
    return true;
  });

  const hideAddButton = computed(() => tabs.value.length === 0);

  const onAddButtonClick = () => {
    if (addDisabled.value) return;
    activeTabRef.value?.onAddClick?.();
  };

  onMounted(() => {
    if (tabs.value.length === 0) {
      router.replace({ name: ROUTES.dashboard });
      return;
    }

    // If user opened a tab without permission - redirect to first allowed.
    const allowed = new Set(tabs.value.map((t) => t.id as UserPageTab));
    const current = routeToTabMap[route.name as string];
    if (current && !allowed.has(current) && firstAllowedTab.value) {
      router.replace({ name: tabToRouteMap[firstAllowedTab.value] });
    }
  });
</script>

<template>
  <div class="page">
    <users-page-header
      v-model:search="searchWord"
      v-model:selected-tab="selectedTab"
      :tabs="tabs"
      :add-disabled="addDisabled"
      :hide-add-button="hideAddButton"
      @add-button-click="onAddButtonClick"
    ></users-page-header>
    <div class="main">
      <router-view v-slot="{ Component }">
        <component :is="Component" ref="activeTabRef"></component>
      </router-view>
    </div>
  </div>
</template>
