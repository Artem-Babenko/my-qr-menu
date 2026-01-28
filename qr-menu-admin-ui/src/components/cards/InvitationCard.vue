<script lang="ts" setup>
  import type { Invitation } from '@/types/invitations';
  import { AppCard, AppFlex, AppIcon, AppText } from '../shared';
  import { computed } from 'vue';
  import { useRolesStore } from '@/store/roles';
  import { useNetworkStore } from '@/store/network';
  import { expiresAt } from '@/utils/dates';
  import { ROUTES } from '@/router';

  const props = defineProps<{ invitation: Invitation }>();
  const emit = defineEmits<{ delete: [] }>();

  const roleStore = useRolesStore();
  const networkStore = useNetworkStore();

  const roleName = computed(
    () =>
      roleStore.roles?.find((r) => r.id === props.invitation.roleId)?.name ??
      '—',
  );

  const establishmentName = computed(
    () =>
      networkStore.network?.establishments.find(
        (est) => est.id === props.invitation.establishmentId,
      )?.name ?? '—',
  );

  const createdAtFormatted = computed(() => {
    return new Date(props.invitation.createdAt).toLocaleDateString();
  });

  const expiresIn = computed(() => expiresAt(props.invitation.expiredAt));

  const copyInvitationLink = async () => {
    const link = new URL(
      `/auth/${ROUTES.regByInvitation}/${props.invitation.id}`,
      window.location.origin,
    ).toString();
    await navigator.clipboard.writeText(link);
  };
</script>

<template>
  <app-card>
    <app-flex gap="16" align="flex-start">
      <app-flex class="user-icon" justify="center">
        <app-icon
          name="User"
          :size="20"
          color="var(--secondary-text)"
        ></app-icon>
      </app-flex>

      <app-flex direction="column" gap="6" align="flex-start">
        <app-text weight="500">
          {{ invitation.name }} {{ invitation.surname }}
        </app-text>

        <app-text size="xs" color="secondary">
          <app-icon name="Phone" :size="12" />
          {{ invitation.phone }}
        </app-text>

        <app-text size="xs" color="secondary">
          <app-icon name="User" :size="12" />
          {{ roleName }}
        </app-text>

        <app-text size="xs" color="secondary">
          <app-icon name="Store" :size="12" />
          {{ establishmentName }}
        </app-text>

        <app-text size="xs" color="secondary">
          {{ expiresIn }}
        </app-text>

        <app-text size="xs" color="secondary">
          Створено: {{ createdAtFormatted }}
        </app-text>
      </app-flex>

      <app-flex class="icons" gap="10">
        <app-icon name="Trash" @click="emit('delete')"></app-icon>
        <app-icon
          v-if="!invitation.targetUserId"
          name="Copy"
          @click="copyInvitationLink"
        ></app-icon>
      </app-flex>
    </app-flex>
  </app-card>
</template>

<style scoped>
  .user-icon {
    width: 40px;
    height: 40px;
    background-color: var(--hover-on-secondary);
    border-radius: 10px;
  }
  .icons {
    margin-left: auto;
  }
  .icons .app-icon {
    color: var(--secondary-text);
    transition: all 0.2s ease;
    cursor: pointer;

    &:hover {
      color: var(--primary-text);
    }
  }
</style>
