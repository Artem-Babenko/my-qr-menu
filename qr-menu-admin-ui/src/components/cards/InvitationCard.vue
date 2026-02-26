<script lang="ts" setup>
  import { computed } from 'vue';
  import type { Invitation } from '@/types/invitations';
  import { AppCard, AppFlex, AppIcon, AppText } from '../shared';
  import { CardDropdown } from '../dropdowns';
  import type { ActionButton } from '../dropdowns';
  import { useRolesStore } from '@/store/roles';
  import { useNetworkStore } from '@/store/network';
  import { expiresAt, formatDate } from '@/utils/dates';
  import { ROUTES } from '@/router';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';

  const props = defineProps<{ invitation: Invitation }>();
  const emit = defineEmits<{ delete: [] }>();

  const roleStore = useRolesStore();
  const networkStore = useNetworkStore();
  const { hasAny } = usePermissions();

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
    return formatDate(props.invitation.createdAt, false);
  });

  const expiresIn = computed(() => expiresAt(props.invitation.expiredAt));

  const canDelete = computed(() => hasAny(PermissionType.invitationsDelete));

  const canCopy = computed(
    () =>
      hasAny(PermissionType.invitationsView) ||
      hasAny(PermissionType.invitationsCreate),
  );

  const onDelete = () => emit('delete');

  const copyInvitationLink = async () => {
    const link = new URL(
      `/auth/${ROUTES.regByInvitation}/${props.invitation.id}`,
      window.location.origin,
    ).toString();
    await navigator.clipboard.writeText(link);
  };

  const buttons = computed<ActionButton[]>(() => {
    const list: ActionButton[] = [];

    if (!props.invitation.targetUserId) {
      list.push({
        icon: 'Copy',
        title: 'Скопіювати посилання',
        click: copyInvitationLink,
        disabled: () => !canCopy.value,
      });
    }

    list.push({
      icon: 'Trash',
      title: 'Видалити',
      click: onDelete,
      disabled: () => !canDelete.value,
    });

    return list;
  });
</script>

<template>
  <app-card class="invitation-card">
    <app-flex justify="space-between" align="flex-start">
      <app-flex :gap="10">
        <app-flex class="circle-icon" justify="center" align="center">
          <app-icon name="MailPlus" :size="20"></app-icon>
        </app-flex>
        <app-flex direction="column" gap="6" align="flex-start" class="head-info">
          <app-text weight="600">
            {{ invitation.name }} {{ invitation.surname }}
          </app-text>
          <app-text color="secondary" size="xs">
            <app-flex :gap="4">
              <app-icon name="Phone" :size="12" />
              {{ invitation.phone }}
            </app-flex>
          </app-text>
        </app-flex>
      </app-flex>
      <card-dropdown :buttons="buttons"></card-dropdown>
    </app-flex>

    <app-flex direction="column" align="flex-start" :gap="8">
      <app-text size="xs" color="secondary">
        <app-flex :gap="4">
          <app-icon name="UserRoundCog" :size="12" />
          {{ roleName }}
        </app-flex>
      </app-text>

      <app-text size="xs" color="secondary">
        <app-flex :gap="4">
          <app-icon name="Store" :size="12" />
          {{ establishmentName }}
        </app-flex>
      </app-text>
      <app-text size="xs" color="secondary">
        <app-flex :gap="4">
          <app-icon name="Clock3" :size="12" />
          {{ expiresIn }}
        </app-flex>
      </app-text>
    </app-flex>

    <app-flex class="footer" justify="space-between" align="center" :gap="10">
      <app-text size="xs" color="secondary">
        Створено: {{ createdAtFormatted }}
      </app-text>
    </app-flex>
  </app-card>
</template>

<style scoped>
  .invitation-card {
    min-height: 170px;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    gap: 10px;
  }
  .head-info {
    min-width: 0;
  }
  .head-info .app-text {
    max-width: 100%;
    overflow: hidden;
    text-overflow: ellipsis;
  }
  .circle-icon {
    width: 40px;
    height: 40px;
    background-color: var(--secondary-container);
    color: var(--on-secondary-container);
    border-radius: 50%;
    flex-shrink: 0;
  }
  .footer {
    border-top: 1px solid var(--outline-variant);
    padding-top: 12px;
  }
</style>
