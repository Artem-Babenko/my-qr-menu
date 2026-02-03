<script setup lang="ts">
  import { onMounted, ref, useTemplateRef } from 'vue';
  import { NetworkInvitationList } from '@/components/lists';
  import { InvitationModal } from '@/components/modals';
  import { useRoute, useRouter } from 'vue-router';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';
  import { AppText } from '@/components/shared';

  const route = useRoute();
  const router = useRouter();
  const { hasAny } = usePermissions();

  const listRef = useTemplateRef<{ refetch: () => unknown }>('listRef');
  const modalShowed = ref(false);

  const canViewInvites = () => hasAny(PermissionType.invitationsView);
  const canCreateInvite = () => hasAny(PermissionType.invitationsView);

  const onAddClick = () => {
    if (!canCreateInvite()) return;
    modalShowed.value = true;
  };

  const onSaved = () => {
    listRef.value?.refetch();
  };

  onMounted(() => {
    if (!canCreateInvite()) return;
    if (route.query.openInviteModal === '1') {
      modalShowed.value = true;
      router.replace({ query: {} });
    }
  });

  defineExpose({ onAddClick });
</script>

<template>
  <app-text v-if="!canViewInvites()" color="secondary">
    Недостатньо прав для перегляду запрошень
  </app-text>
  <template v-else>
    <network-invitation-list ref="listRef"></network-invitation-list>
    <invitation-modal
      v-model:showed="modalShowed"
      @save="onSaved"
    ></invitation-modal>
  </template>
</template>
