<script setup lang="ts">
  import { UserModal } from '@/components/modals';
  import { UserList } from '@/components/lists';
  import { useRouter } from 'vue-router';
  import { ROUTES } from '@/router';
  import type { User } from '@/types/user';
  import { computed, ref } from 'vue';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';
  import { AppFlex, AppSearchInput } from '@/components/shared';
  import { AddButton } from '@/components/buttons';

  const router = useRouter();
  const modalShowed = ref(false);
  const editingUser = ref<User | null>(null);
  const search = ref('');
  const { hasAny } = usePermissions();

  const canCreateInvite = computed(() =>
    hasAny(PermissionType.invitationsCreate),
  );

  const onAddClick = () => {
    if (!canCreateInvite.value) return;
    router.push({
      name: ROUTES.usersInvitations,
      query: { openInviteModal: '1' },
    });
  };

  const onEdit = (user: User) => {
    if (!hasAny(PermissionType.usersEdit)) return;
    editingUser.value = user;
    modalShowed.value = true;
  };
</script>

<template>
  <div class="tab">
    <app-flex class="controls" align="center" gap="20">
      <div class="search">
        <app-search-input
          v-model="search"
          placeholder="Пошук користувачів..."
        ></app-search-input>
      </div>
      <add-button :disabled="!canCreateInvite" @click="onAddClick">
        Запросити користувача
      </add-button>
    </app-flex>

    <user-list :search="search" @edit="onEdit"></user-list>
  </div>
  <user-modal v-model:showed="modalShowed" :user="editingUser"></user-modal>
</template>

<style scoped>
  .tab {
    display: flex;
    flex-direction: column;
    gap: 15px;
  }
  .controls {
    width: 100%;
  }
  .search {
    flex: 1;
  }
  :deep(.search .app-search-input) {
    width: 100%;
  }
</style>
