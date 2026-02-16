<script lang="ts" setup>
  import type { UserInvitation } from '@/types/invitations';
  import { AppButton, AppCard, AppFlex, AppIcon, AppText } from '../shared';
  import { computed } from 'vue';
  import { expiresAt } from '@/utils/dates';
  import { InvitationStatus } from '@/consts/invitations';

  const emit = defineEmits<{
    accept: [];
    cancel: [];
    delete: [];
  }>();

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
    <app-text class="est-name" weight="500">
      {{ invitation.establishmentName }}
    </app-text>
    <app-flex class="property-block">
      <app-icon name="Signpost"></app-icon>
      <app-text>{{ invitation.establishmentAddress }}</app-text>
    </app-flex>
    <app-flex class="property-block">
      <app-icon name="User"></app-icon>
      <app-text>{{ invitation.roleName }}</app-text>
    </app-flex>
    <app-flex class="property-block">
      <app-icon name="Clock"></app-icon>
      <app-text>{{ expired }}</app-text>
    </app-flex>
    <app-flex class="buttons">
      <template v-if="invitation.status === InvitationStatus.pending">
        <app-button type="outline" @click="emit('cancel')">
          Відхилити
        </app-button>
        <app-button :disabled="!active" @click="emit('accept')">
          Прийняти
        </app-button>
      </template>
      <app-button v-else type="outline" @click="emit('delete')">
        Видалити
      </app-button>
    </app-flex>
  </app-card>
</template>

<style scoped>
  .property-block {
    gap: 5px !important;
    color: var(--on-surface-variant);
    margin-bottom: 5px;
  }
  .est-name {
    margin-bottom: 10px;
  }
  .buttons {
    gap: 10px !important;
    margin-top: 10px;

    .app-button {
      width: 100%;
      justify-content: center;
    }
  }
</style>
