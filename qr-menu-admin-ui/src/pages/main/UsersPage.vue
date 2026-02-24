<script lang="ts" setup>
  import { computed, watch } from 'vue';
  import { AppTabs, AppText } from '@/components/shared';
  import { PageHeader } from '@/components/headers';
  import { UserPageTab } from '@/consts/tabs';
  import { useRoute, useRouter } from 'vue-router';
  import { ROUTES } from '@/router';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';
  import type { AppTab } from '@/types/components';

  const route = useRoute();
  const router = useRouter();
  const { hasAny } = usePermissions();

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

  const canView = computed(
    () => canUsers.value || canRoles.value || canInvites.value,
  );

  watch(
    canView,
    (v) => {
      if (v) return;
      router.replace({ name: ROUTES.dashboard });
    },
    { immediate: true },
  );

  watch(
    tabs,
    (newTabs) => {
      if (!newTabs.length) return;
      const allowed = new Set(newTabs.map((t) => t.id as UserPageTab));
      const current = routeToTabMap[route.name as string];
      if (current && !allowed.has(current) && firstAllowedTab.value) {
        router.replace({ name: tabToRouteMap[firstAllowedTab.value] });
      }
    },
    { immediate: true },
  );
</script>

<template>
  <div v-if="canView" class="page">
    <page-header section-name="Користувачі"></page-header>

    <app-tabs v-model:selected="selectedTab" :tabs="tabs"></app-tabs>

    <router-view></router-view>
  </div>
  <div v-else>
    <app-text color="secondary">
      Недостатньо прав для перегляду користувачів
    </app-text>
  </div>
</template>

<style scoped>
  .page {
    display: flex;
    flex-direction: column;
    gap: 20px;
  }

  @media (max-width: 768px) {
    .page {
      gap: 12px;
    }
  }
</style>
