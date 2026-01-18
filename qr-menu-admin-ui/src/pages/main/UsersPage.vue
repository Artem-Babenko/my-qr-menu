<script lang="ts" setup>
  import { UsersPageHeader } from '@/components/headers';
  import { UserList, RoleList, InvitationList } from '@/components/lists';
  import { InvitationModal } from '@/components/modals';
  import { UserPageTab } from '@/consts/tabs';
  import { reactive, ref } from 'vue';

  const searchWord = ref('');
  const selectedTab = ref(UserPageTab.users);

  const modalShowed = reactive({
    invitation: false,
  });

  const addButtonEvents: Record<UserPageTab, () => void> = {
    [UserPageTab.users]: () => {},
    [UserPageTab.roles]: () => {},
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
      <role-list v-else-if="selectedTab === UserPageTab.roles"></role-list>
      <invitation-list
        v-else-if="selectedTab === UserPageTab.invites"
      ></invitation-list>
    </div>

    <invitation-modal
      v-model:showed="modalShowed.invitation"
    ></invitation-modal>
  </div>
</template>
