<script lang="ts" setup>
  import { computed } from 'vue';
  import { RoleCard } from '../cards';
  import BaseCardList from './BaseCardList.vue';
  import { useRolesStore } from '@/store/roles';
  import type { RoleView } from '@/types/roles';
  import { rolesApi } from '@/api/rolesApi';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';
  import { AppText } from '@/components/shared';

  const props = withDefaults(defineProps<{ search?: string }>(), {
    search: '',
  });

  const emit = defineEmits<{ edit: [role: RoleView] }>();

  const rolesStore = useRolesStore();
  const { hasAny } = usePermissions();

  const canDelete = () => hasAny(PermissionType.rolesDelete);

  const openEdit = (role: RoleView) => {
    emit('edit', role);
  };

  const deleteRole = async (role: RoleView) => {
    if (!canDelete()) return;
    const resp = await rolesApi.delete(role.id);
    if (!resp.success) throw resp.errorCode;
    rolesStore.deleteRole(role);
  };

  const filteredRoles = computed(() => {
    const list = rolesStore.roles ?? [];
    const q = props.search.trim().toLowerCase();
    if (!q) return list;
    return list.filter((role) => {
      const hay = `${role.name} ${role.description ?? ''}`.toLowerCase();
      return hay.includes(q);
    });
  });
</script>

<template>
  <base-card-list>
    <role-card
      v-for="role in filteredRoles"
      :key="role.id"
      :role="role"
      @edit="openEdit(role)"
      @delete="deleteRole(role)"
    ></role-card>
    <app-text v-if="filteredRoles.length === 0" color="secondary">
      Нічого не знайдено
    </app-text>
  </base-card-list>
</template>
