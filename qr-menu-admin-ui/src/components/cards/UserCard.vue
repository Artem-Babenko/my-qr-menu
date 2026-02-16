<script lang="ts" setup>
  import { AppCard, AppText } from '@/components/shared';
  import type { User } from '@/types/user';
  import { UserInitials } from '../other';
  import { CardDropdown } from '../dropdowns';
  import { computed } from 'vue';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';

  const { user } = defineProps<{ user: User }>();
  const emit = defineEmits<{ edit: [user: User] }>();

  const { hasAny } = usePermissions();

  const establishmentsCount = computed(() => user.accesses?.length ?? 0);

  const canEdit = computed(() => hasAny(PermissionType.usersEdit));

  const onEdit = () => emit('edit', user);
</script>

<template>
  <app-card class="user-card">
    <div class="head">
      <user-initials v-bind="user"></user-initials>
      <card-dropdown
        :buttons="[
          {
            icon: 'Pencil',
            title: 'Редагувати',
            click: onEdit,
            disabled: () => !canEdit,
          },
        ]"
      ></card-dropdown>
    </div>
    <div class="info">
      <app-text>{{ user.name }} {{ user.surname }}</app-text>
      <app-text>{{ user.email }}</app-text>
      <app-text>{{ user.phone }}</app-text>
      <app-text>{{ establishmentsCount }} ресторани</app-text>
    </div>
    <div class="footer">
      <app-text>Остання активність: 5 год тому</app-text>
    </div>
  </app-card>
</template>

<style scoped>
  .head {
    display: flex;
    justify-content: space-between;
    align-items: start;
  }
  .info {
    display: flex;
    flex-direction: column;
    gap: 3px;
    margin: 10px 0;
  }
  .footer {
    border-top: 1px solid var(--outline-variant);
    padding-top: 5px;
  }
</style>
