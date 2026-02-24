<script lang="ts" setup>
  import { computed } from 'vue';
  import { useLoader } from '@/composables';
  import BaseCardList from './BaseCardList.vue';
  import { useNetworkStore } from '@/store/network';
  import { toRef } from 'vue';
  import { invitationApi } from '@/api/invitationApi';
  import { InvitationCard } from '../cards';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';
  import { AppText } from '@/components/shared';
  import { useRolesStore } from '@/store/roles';

  const props = withDefaults(defineProps<{ search?: string }>(), {
    search: '',
  });

  const networkStore = useNetworkStore();
  const rolesStore = useRolesStore();
  const networkId = toRef(() => networkStore.network?.id);

  const { hasAny } = usePermissions();
  const canView = () => hasAny(PermissionType.invitationsView);
  const canDelete = () => hasAny(PermissionType.invitationsDelete);

  const { data: invitations, refetch } = useLoader({
    keys: ['invations', networkId],
    fn: async () => {
      if (!networkId.value) return;
      const resp = await invitationApi.getByNetwork(networkId.value);
      if (!resp.success) return [];
      return resp.data ?? [];
    },
    enabled: () => !!networkId.value && canView(),
  });

  const deleteInvitation = async (id: string) => {
    if (!invitations.value) return;
    if (!canDelete()) return;
    const resp = await invitationApi.delete(id);
    if (!resp.success) throw new Error('Error while deliting invation');
    invitations.value = invitations.value.filter((inv) => inv.id !== id);
  };

  const filteredInvitations = computed(() => {
    const list = invitations.value ?? [];
    const roles = rolesStore.roles ?? [];
    const q = props.search.trim().toLowerCase();
    if (!q) return list;
    return list.filter((invitation) => {
      const roleName =
        roles.find((role) => role.id === invitation.roleId)?.name ?? '';
      const establishmentName =
        networkStore.network?.establishments.find(
          (est) => est.id === invitation.establishmentId,
        )?.name ?? '';
      const hay =
        `${invitation.name ?? ''} ${invitation.surname ?? ''} ${invitation.phone ?? ''} ${roleName} ${establishmentName}`.toLowerCase();
      return hay.includes(q);
    });
  });

  defineExpose({ refetch });
</script>

<template>
  <app-text v-if="!canView()" color="secondary">
    Недостатньо прав для перегляду запрошень
  </app-text>
  <base-card-list>
    <invitation-card
      v-for="inv in filteredInvitations"
      :key="inv.id"
      :invitation="inv"
      @delete="deleteInvitation(inv.id)"
    ></invitation-card>
    <app-text
      v-if="canView() && filteredInvitations.length === 0"
      color="secondary"
    >
      Нічого не знайдено
    </app-text>
  </base-card-list>
</template>
