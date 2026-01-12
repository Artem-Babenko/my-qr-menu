<script lang="ts" setup>
  import { RoleCard } from '../cards';
  import BaseCardList from './BaseCardList.vue';
  import { useRolesStore } from '@/store/roles';

  const rolesStore = useRolesStore();
</script>

<template>
  <base-card-list>
    <role-card v-for="role in rolesStore.roles" :role="role"></role-card>
  </base-card-list>
</template>
