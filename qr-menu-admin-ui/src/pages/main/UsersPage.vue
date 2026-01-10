<script lang="ts" setup>
  import { UsersPageHeader } from '@/components/headers';
  import { UserList, RoleList } from '@/components/lists';
  import { UserPageTab } from '@/consts/tabs';
  import { ref } from 'vue';

  const searchWord = ref('');
  const selectedTab = ref(UserPageTab.users);
</script>

<template>
  <div class="page">
    <users-page-header
      v-model:search="searchWord"
      v-model:selected-tab="selectedTab"
    ></users-page-header>
    <div class="main">
      <user-list v-if="selectedTab === UserPageTab.users"></user-list>
      <role-list v-else-if="selectedTab === UserPageTab.roles"></role-list>
      <template v-else-if="selectedTab === UserPageTab.invites"></template>
    </div>
  </div>
</template>
