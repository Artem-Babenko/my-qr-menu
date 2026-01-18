<script lang="ts" setup>
  import type { Invitation } from '@/types/invitations';
  import { AppCard, AppFlex, AppIcon, AppText } from '../shared';
  import { computed } from 'vue';
  import { useRolesStore } from '@/store/roles';
  import { useNetworkStore } from '@/store/network';

  const props = defineProps<{ invitation: Invitation }>();

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

  const expiresIn = computed(() => {
    const now = new Date();
    const expiredAt = new Date(props.invitation.expiredAt);

    const diffMs = expiredAt.getTime() - now.getTime();
    if (diffMs <= 0) return 'Запрошення прострочене';

    const diffHours = Math.floor(diffMs / (1000 * 60 * 60));
    const days = Math.floor(diffHours / 24);
    const hours = diffHours % 24;

    if (days > 0) return `Діє ще ${days} дн.`;
    return `Діє ще ${hours} год.`;
  });
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

      <app-icon
        class="delete-icon"
        name="Trash"
        :size="16"
        color="var(--secondary-text)"
      ></app-icon>
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
  .delete-icon {
    margin-left: auto;
    transition: all 0.2s ease;
    cursor: pointer;
  }
  .delete-icon:hover {
    stroke: var(--error-text) !important;
  }
</style>
