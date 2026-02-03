<script lang="ts" setup>
  import { useLoader } from '@/composables';
  import BaseCardList from './BaseCardList.vue';
  import { useNetworkStore } from '@/store/network';
  import { toRef } from 'vue';
  import { invitationApi } from '@/api/invitationApi';
  import { InvitationCard } from '../cards';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';
  import { AppText } from '@/components/shared';

  const networkStore = useNetworkStore();
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

  defineExpose({ refetch });
</script>

<template>
  <app-text v-if="!canView()" color="secondary">
    Недостатньо прав для перегляду запрошень
  </app-text>
  <base-card-list>
    <invitation-card
      v-for="inv in invitations"
      :key="inv.id"
      :invitation="inv"
      @delete="deleteInvitation(inv.id)"
    ></invitation-card>
  </base-card-list>
</template>
