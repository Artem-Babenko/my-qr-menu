<script lang="ts" setup>
  import { usersApi } from '@/api/usersApi';
  import {
    AppButton,
    AppCheckbox,
    AppFlex,
    AppInput,
    AppLabel,
    AppModal,
    AppSelect,
    AppText,
  } from '@/components/shared';
  import { useNetworkStore } from '@/store/network';
  import { useRolesStore } from '@/store/roles';
  import type { RoleView } from '@/types/roles';
  import type { User, UserEstablishmentAccess } from '@/types/user';
  import { computed, ref, watch } from 'vue';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';
  import { getErrorMessage } from '@/consts/errorMessages';
  import { useToastsStore } from '@/store/toasts';

  interface EstablishmentRow {
    establishmentId: number;
    name: string;
    enabled: boolean;
    role: RoleView | null;
  }

  const showed = defineModel<boolean>('showed', { required: true });
  const props = defineProps<{ user: User | null }>();
  const emit = defineEmits<{ saved: [] }>();

  const networkStore = useNetworkStore();
  const rolesStore = useRolesStore();
  const toasts = useToastsStore();
  const { hasAny } = usePermissions();

  const name = ref<string | null>(null);
  const surname = ref<string | null>(null);
  const phone = ref<string | null>(null);

  const rows = ref<EstablishmentRow[]>([]);
  const loading = ref(false);
  const saving = ref(false);

  const selectedRows = computed(() => rows.value.filter((r) => r.enabled));
  const saveDisabled = computed(() => {
    if (loading.value || saving.value) return true;
    if (selectedRows.value.length === 0) return true;
    return selectedRows.value.some((r) => !r.role);
  });

  const close = () => {
    showed.value = false;
  };

  const buildRows = (accesses: UserEstablishmentAccess[]) => {
    const ests = networkStore.network?.establishments ?? [];
    const accessByEstId = new Map<number, number>(
      accesses.map((a) => [a.establishmentId, a.roleId]),
    );

    rows.value = ests.map((est) => {
      const roleId = accessByEstId.get(est.id);
      const role = roleId
        ? (rolesStore.roles?.find((r) => r.id === roleId) ?? null)
        : null;
      return {
        establishmentId: est.id,
        name: est.name,
        enabled: roleId !== undefined,
        role,
      };
    });
  };

  watch(
    showed,
    (v) => {
      if (!v) return;
      name.value = props.user?.name ?? null;
      surname.value = props.user?.surname ?? null;
      phone.value = props.user?.phone ?? null;
      rows.value = [];
      loading.value = false;
      buildRows(props.user?.accesses ?? []);
    },
    { immediate: false },
  );

  const save = async () => {
    if (!props.user || saveDisabled.value) return;
    if (!hasAny(PermissionType.usersEdit)) return;

    saving.value = true;
    const payload = {
      accesses: selectedRows.value.map((r) => ({
        establishmentId: r.establishmentId,
        roleId: r.role!.id,
      })),
    };

    const resp = await usersApi.updateEstablishments(props.user.id, payload);
    saving.value = false;

    if (!resp.success) {
      toasts.error(getErrorMessage(resp.errorCode));
      return;
    }
    showed.value = false;
    emit('saved');
  };
</script>

<template>
  <app-modal
    v-model:showed="showed"
    title="Доступ користувача до закладів"
    :width="700"
  >
    <div class="form">
      <app-text color="secondary" line="m">
        Дані користувача редагувати не можна. Налаштуйте доступи до закладів та
        роль.
      </app-text>

      <div class="user-block">
        <div class="input-block">
          <app-label label="Ім'я" for="userName"></app-label>
          <app-input v-model="name" id="userName" disabled></app-input>
        </div>
        <div class="input-block">
          <app-label label="Прізвище" for="userSurname"></app-label>
          <app-input v-model="surname" id="userSurname" disabled></app-input>
        </div>
        <div class="input-block">
          <app-label label="Номер телефону" for="userPhone"></app-label>
          <app-input v-model="phone" id="userPhone" disabled></app-input>
        </div>
      </div>

      <div class="access-block">
        <app-label label="Заклади"></app-label>

        <div class="rows">
          <div v-for="row in rows" :key="row.establishmentId" class="row">
            <app-flex align="center" gap="12" class="row-content">
              <app-checkbox v-model="row.enabled"></app-checkbox>

              <app-text class="est-name" :class="{ disabled: !row.enabled }">
                {{ row.name }}
              </app-text>

              <div class="role">
                <app-select
                  v-if="row.enabled"
                  v-model="row.role"
                  :options="rolesStore.roles"
                  option-label="name"
                  placeholder="Оберіть роль"
                ></app-select>
                <app-text v-else color="secondary">—</app-text>
              </div>
            </app-flex>
          </div>
        </div>
      </div>

      <app-flex class="buttons" justify="flex-end" gap="10">
        <app-button type="outline" @click="close">Скасувати</app-button>
        <app-button
          :disabled="saveDisabled || !hasAny(PermissionType.usersEdit)"
          @click="save"
        >
          Зберегти
        </app-button>
      </app-flex>
    </div>
  </app-modal>
</template>

<style scoped>
  .form {
    display: flex;
    flex-direction: column;
    gap: 20px;
  }

  .user-block {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 12px;
  }

  .input-block {
    width: 100%;
  }

  .input-block:last-child {
    grid-column: 1 / -1;
  }

  .access-block {
    display: flex;
    flex-direction: column;
    gap: 10px;
  }

  .rows {
    display: flex;
    flex-direction: column;
    gap: 8px;
    max-height: 420px;
    overflow-y: auto;
    padding-right: 5px;
  }

  .row {
    border: 1px solid var(--border);
    border-radius: 8px;
    padding: 10px 12px;
    background-color: var(--hover-on-secondary);
  }

  .row-content {
    width: 100%;
  }

  .est-name {
    min-width: 0;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .est-name.disabled {
    color: var(--secondary-text);
  }

  .role {
    margin-left: auto;
    width: 260px;
  }

  .buttons {
    margin-top: 10px;
  }
</style>
