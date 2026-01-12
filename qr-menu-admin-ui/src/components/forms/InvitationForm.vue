<script setup lang="ts">
  import type { NewInvitation } from '@/types/invitations';
  import { AppFlex, AppInput, AppLabel, AppSelect } from '../shared';
  import { computed } from 'vue';
  import { useNetworkStore } from '@/store/network';
  import { useRolesStore } from '@/store/roles';

  const networkStore = useNetworkStore();
  const rolesStore = useRolesStore();

  const invitation = defineModel<NewInvitation>('invitation', {
    required: true,
  });

  defineProps<{ readonlyUser?: boolean }>();

  const establishment = computed({
    get: () =>
      networkStore.network?.establishments.find(
        (est) => est.id === invitation.value.establishmentId,
      ) ?? null,
    set: (val) => {
      invitation.value.establishmentId = val?.id ?? null;
    },
  });

  const role = computed({
    get: () =>
      rolesStore.roles?.find((r) => r.id === invitation.value.roleId) ?? null,
    set: (val) => {
      invitation.value.roleId = val?.id ?? null;
    },
  });
</script>

<template>
  <div>
    <app-flex gap="20">
      <div class="input-block">
        <app-label for="name" label="Ім'я"></app-label>
        <app-input
          v-model="invitation.name"
          id="name"
          placeholder="Ведіть ім'я"
          :disabled="readonlyUser"
        ></app-input>
      </div>
      <div class="input-block">
        <app-label for="surname" label="Прізвище"></app-label>
        <app-input
          v-model="invitation.surname"
          id="surname"
          placeholder="Введіть прізвище"
          :disabled="readonlyUser"
        ></app-input>
      </div>
    </app-flex>
    <div class="input-block">
      <app-label for="phone" label="Телефон"></app-label>
      <app-input
        v-model="invitation.phone"
        id="phone"
        placeholder="Введіть телефон"
        disabled
      ></app-input>
    </div>
    <app-flex gap="20">
      <div class="input-block">
        <app-label label="Заклад"></app-label>
        <app-select
          v-model="establishment"
          :options="networkStore.network!.establishments"
          option-label="name"
          option-value="id"
          placeholder="Оберіть заклад"
        ></app-select>
      </div>
      <div class="input-block">
        <app-label label="Роль"></app-label>
        <app-select
          v-model="role"
          :options="rolesStore.roles"
          option-label="name"
          option-value="id"
          placeholder="Оберіть роль"
        ></app-select>
      </div>
    </app-flex>
  </div>
</template>

<style scoped>
  .input-block {
    margin-top: 20px;
    width: 100%;
  }
</style>
