<script setup lang="ts">
  import { computed, onMounted, ref, useTemplateRef } from 'vue';
  import { NetworkInvitationList } from '@/components/lists';
  import { InvitationModal } from '@/components/modals';
  import { useRoute, useRouter } from 'vue-router';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';
  import { AppFlex, AppSearchInput, AppText } from '@/components/shared';
  import { AddButton } from '@/components/buttons';

  const route = useRoute();
  const router = useRouter();
  const { hasAny } = usePermissions();

  const listRef = useTemplateRef<{ refetch: () => unknown }>('listRef');
  const modalShowed = ref(false);
  const search = ref('');

  const canViewInvites = computed(() => hasAny(PermissionType.invitationsView));
  const canCreateInvite = computed(() =>
    hasAny(PermissionType.invitationsCreate),
  );

  const onAddClick = () => {
    if (!canCreateInvite.value) return;
    modalShowed.value = true;
  };

  const onSaved = () => {
    listRef.value?.refetch();
  };

  onMounted(() => {
    if (!canCreateInvite.value) return;
    if (route.query.openInviteModal === '1') {
      modalShowed.value = true;
      router.replace({ query: {} });
    }
  });
</script>

<template>
  <app-text v-if="!canViewInvites" color="secondary">
    Недостатньо прав для перегляду запрошень
  </app-text>
  <div v-else class="tab">
    <app-flex class="controls" align="center" gap="20">
      <div class="search">
        <app-search-input
          v-model="search"
          placeholder="Пошук запрошень..."
        ></app-search-input>
      </div>
      <add-button :disabled="!canCreateInvite" @click="onAddClick">
        Створити запрошення
      </add-button>
    </app-flex>

    <network-invitation-list
      ref="listRef"
      :search="search"
    ></network-invitation-list>
    <invitation-modal
      v-model:showed="modalShowed"
      @save="onSaved"
    ></invitation-modal>
  </div>
</template>

<style scoped>
  .tab {
    display: flex;
    flex-direction: column;
    gap: 15px;
  }
  .controls {
    width: 100%;
    flex-wrap: wrap;
  }
  .search {
    flex: 1;
  }
  :deep(.search .app-search-input) {
    width: 100%;
  }

  @media (max-width: 768px) {
    .controls {
      gap: 12px;
    }
    .search {
      width: 100%;
      flex: 1 0 100%;
    }
    .controls :deep(.app-button) {
      width: 100%;
      justify-content: center;
    }
  }
</style>
