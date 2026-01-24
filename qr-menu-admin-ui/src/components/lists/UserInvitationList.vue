<script lang="ts" setup>
  import type { UserInvitation } from '@/types/invitations';
  import BaseCardList from './BaseCardList.vue';
  import { UserInvitaionCard } from '../cards';

  defineProps<{ invitations: UserInvitation[] }>();
</script>

<template>
  <base-card-list>
    <user-invitaion-card
      v-for="inv in invitations"
      :invitation="inv"
    ></user-invitaion-card>
  </base-card-list>
</template>
