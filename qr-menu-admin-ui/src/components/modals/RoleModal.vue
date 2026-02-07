<script lang="ts" setup>
  import { computed, ref, watch } from 'vue';
  import {
    AppButton,
    AppCheckbox,
    AppFlex,
    AppInput,
    AppLabel,
    AppModal,
    AppText,
  } from '../shared';
  import type { RoleRequest, RoleView } from '@/types/roles';
  import {
    PERMISSION_GROUPS,
    PERMISSION_LABELS,
    PermissionType,
  } from '@/consts/roles';
  import { rolesApi } from '@/api/rolesApi';
  import { useRolesStore } from '@/store/roles';
  import { useNetworkStore } from '@/store/network';
  import { usePermissions } from '@/composables';
  import { getErrorMessage } from '@/consts/errorMessages';
  import { useToastsStore } from '@/store/toasts';

  const showed = defineModel<boolean>('showed', { required: true });
  const props = defineProps<{ role?: RoleView | null }>();
  const emit = defineEmits<{ save: [] }>();

  const rolesStore = useRolesStore();
  const networkStore = useNetworkStore();
  const toasts = useToastsStore();
  const { hasAny } = usePermissions();

  const name = ref('');
  const description = ref('');
  const selectedPermissions = ref<PermissionType[]>([]);

  const isEditMode = computed(() => !!props.role);
  const saveDisabled = computed(() => !name.value.trim());
  const modalTitle = computed(() =>
    isEditMode.value ? 'Редагувати роль' : 'Створити роль',
  );
  const canEdit = computed(() =>
    isEditMode.value
      ? hasAny(PermissionType.rolesUpdate)
      : hasAny(PermissionType.rolesCreate),
  );

  const isPermissionSelected = (permission: PermissionType) => {
    return selectedPermissions.value.includes(permission);
  };

  watch(showed, (newValue) => {
    if (!newValue) return;
    if (props.role) {
      name.value = props.role.name;
      description.value = props.role.description || '';
      selectedPermissions.value = [...props.role.permissions];
    } else {
      resetForm();
    }
  });

  const resetForm = () => {
    name.value = '';
    description.value = '';
    selectedPermissions.value = [];
  };

  const togglePermission = (permission: PermissionType) => {
    const index = selectedPermissions.value.indexOf(permission);
    if (index > -1) {
      selectedPermissions.value.splice(index, 1);
    } else {
      selectedPermissions.value.push(permission);
    }
  };

  const setPermission = (permission: PermissionType, enabled: boolean) => {
    const index = selectedPermissions.value.indexOf(permission);
    const has = index > -1;
    if (enabled && !has) selectedPermissions.value.push(permission);
    if (!enabled && has) selectedPermissions.value.splice(index, 1);
  };

  const close = () => {
    showed.value = false;
  };

  const getPermissionLabel = (permission: PermissionType): string => {
    return PERMISSION_LABELS[permission] || 'Невідомий дозвіл';
  };

  const save = async () => {
    if (saveDisabled.value || !networkStore.network?.id) return;
    if (!canEdit.value) return;

    const payload: RoleRequest = {
      name: name.value.trim(),
      description: description.value.trim() || null,
      networkId: networkStore.network.id,
      permissions: [...selectedPermissions.value],
    };

    let response;
    if (isEditMode.value && props.role) {
      response = await rolesApi.update(props.role.id, payload);
    } else {
      response = await rolesApi.create(payload);
    }

    if (!response.success || !response.data) {
      toasts.error(getErrorMessage(response.errorCode));
      return;
    }

    if (isEditMode.value) {
      rolesStore.updateRole(response.data);
    } else {
      rolesStore.addRole(response.data);
    }

    showed.value = false;
    emit('save');
  };
</script>

<template>
  <app-modal v-model:showed="showed" :title="modalTitle" :width="600">
    <div class="form">
      <div class="input-block">
        <app-label label="Назва ролі" for="roleName"></app-label>
        <app-input
          v-model="name"
          id="roleName"
          placeholder="Введіть назву ролі"
        ></app-input>
      </div>

      <div class="input-block">
        <app-label label="Опис ролі" for="roleDescription"></app-label>
        <app-input
          v-model="description"
          id="roleDescription"
          placeholder="Введіть опис ролі (необов'язково)"
        ></app-input>
      </div>

      <div class="permissions-block">
        <app-label label="Дозволи"></app-label>
        <div class="permissions-groups">
          <div
            v-for="group in PERMISSION_GROUPS"
            :key="group.name"
            class="permission-group"
          >
            <app-text weight="600" size="s" class="group-title">
              {{ group.name }}
            </app-text>
            <div class="permissions-list">
              <div
                v-for="permission in group.permissions"
                :key="permission"
                class="permission-item"
                @click="canEdit && togglePermission(permission)"
              >
                <app-checkbox
                  :disabled="!canEdit"
                  :model-value="isPermissionSelected(permission)"
                  @update:model-value="
                    (v: boolean) => setPermission(permission, v)
                  "
                  @click.stop
                ></app-checkbox>
                <app-text size="s">
                  {{ getPermissionLabel(permission) }}
                </app-text>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <app-flex class="form-buttons" justify="flex-end" gap="10">
      <app-button type="outline" @click="close">Скасувати</app-button>
      <app-button :disabled="saveDisabled || !canEdit" @click="save">
        {{ isEditMode ? 'Зберегти зміни' : 'Створити роль' }}
      </app-button>
    </app-flex>
  </app-modal>
</template>

<style scoped>
  .form {
    display: flex;
    flex-direction: column;
    gap: 20px;
  }

  .input-block {
    width: 100%;
  }

  .permissions-block {
    width: 100%;
  }

  .permissions-groups {
    display: flex;
    flex-direction: column;
    gap: 20px;
    margin-top: 10px;
    max-height: 400px;
    overflow-y: auto;
    padding-right: 5px;
  }

  .permission-group {
    display: flex;
    flex-direction: column;
    gap: 10px;
    padding: 15px;
    border: 1px solid var(--border);
    border-radius: 8px;
    background-color: var(--hover-on-secondary);
  }

  .group-title {
    margin-bottom: 5px;
  }

  .permissions-list {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }

  .permission-item {
    display: flex;
    align-items: center;
    gap: 10px;
    cursor: pointer;
    padding: 5px;
    border-radius: 4px;
    transition: background-color 0.2s ease;
  }

  .permission-item:hover {
    background-color: var(--secondary);
  }

  .form-buttons {
    margin-top: 20px;
  }
</style>
