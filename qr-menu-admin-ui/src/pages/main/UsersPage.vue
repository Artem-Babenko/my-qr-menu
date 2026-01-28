<script lang="ts" setup>
  import { ref } from 'vue';
  import { UsersPageHeader } from '@/components/headers';
  import {
    UserList,
    RoleList,
    NetworkInvitationList,
  } from '@/components/lists';
  import { UserPageTab } from '@/consts/tabs';
  import { reactive } from 'vue';

  const searchWord = ref('');
  const selectedTab = ref(UserPageTab.users);

  const modalShowed = reactive({
    invitation: false,
    role: false,
  });

  const addButtonEvents: Record<UserPageTab, () => void> = {
    [UserPageTab.users]: () => {},
    [UserPageTab.roles]: () => (modalShowed.role = true),
    [UserPageTab.invites]: () => (modalShowed.invitation = true),
  };
</script>

<template>
  <div class="page">
    <users-page-header
      v-model:search="searchWord"
      v-model:selected-tab="selectedTab"
      @add-button-click="addButtonEvents[selectedTab]()"
    ></users-page-header>
    <div class="main">
      <user-list v-if="selectedTab === UserPageTab.users"></user-list>
      <role-list
        v-else-if="selectedTab === UserPageTab.roles"
        v-model:showed="modalShowed.role"
      ></role-list>
      <network-invitation-list
        v-else-if="selectedTab === UserPageTab.invites"
        v-model:modal-showed="modalShowed.invitation"
      ></network-invitation-list>
    </div>
  </div>
</template>
