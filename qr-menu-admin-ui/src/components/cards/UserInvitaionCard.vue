<script lang="ts" setup>
  import type { UserInvitation } from '@/types/invitations';
  import { AppButton, AppCard, AppFlex, AppIcon, AppText } from '../shared';
  import { computed } from 'vue';
  import { expiresAt } from '@/utils/dates';

  const props = defineProps<{ invitation: UserInvitation }>();

  const expired = computed(() => expiresAt(props.invitation.expiredAt));

  const active = computed(() => {
    const now = new Date();
    const expiredDate = new Date(props.invitation.expiredAt);
    return expiredDate.getTime() > now.getTime();
  });
</script>

<template>
  <app-card>
    <app-text line="m">{{ invitation.establishmentName }}</app-text>
    <app-text line="m">{{ invitation.establishmentAddress }}</app-text>
    <app-flex gap="5">
      <app-icon name="User" color="var(--secondary-text)"></app-icon>
      <app-text line="m" color="secondary">{{ invitation.roleName }}</app-text>
    </app-flex>
    <app-flex gap="5">
      <app-icon name="Clock" color="var(--secondary-text)"></app-icon>
      <app-text line="m" color="secondary">{{ expired }}</app-text>
    </app-flex>
    <app-flex gap="10" justify="center">
      <app-button type="outline">Відхилити</app-button>
      <app-button :disabled="!active">Прийняти</app-button>
    </app-flex>
  </app-card>
</template>

<style scoped></style>
