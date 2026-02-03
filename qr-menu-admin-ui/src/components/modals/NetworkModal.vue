<script lang="ts" setup>
  import { networkApi } from '@/api/networkApi';
  import { useNetworkStore } from '@/store/network';
  import { useToastsStore } from '@/store/toasts';
  import { computed, ref, watch } from 'vue';
  import { AppButton, AppFlex, AppInput, AppLabel, AppModal } from '../shared';
  import { getErrorMessage } from '@/consts/errorMessages';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';

  const showed = defineModel<boolean>('showed', { required: true });
  const emit = defineEmits<{ saved: [] }>();

  const networkStore = useNetworkStore();
  const toasts = useToastsStore();

  const { hasAny } = usePermissions();
  const canEdit = computed(() => hasAny(PermissionType.networkEdit));

  const name = ref<string | null>(null);
  const saving = ref(false);

  const saveDisabled = computed(() => {
    if (saving.value) return true;
    if (!name.value?.trim()) return true;
    return false;
  });

  watch(showed, (v) => {
    if (!v) return;
    name.value = networkStore.network?.name ?? '';
    saving.value = false;
  });

  const close = () => (showed.value = false);

  const save = async () => {
    if (!networkStore.network?.id || saveDisabled.value) return;
    if (!canEdit.value) return;
    saving.value = true;
    const resp = await networkApi.updateNetworkName(networkStore.network.id, {
      name: name.value!.trim(),
    });
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
  <app-modal v-model:showed="showed" title="Налаштування мережі" :width="520">
    <div class="form">
      <div class="input-block">
        <app-label label="Назва мережі" for="networkName"></app-label>
        <app-input
          v-model="name"
          id="networkName"
          placeholder="Введіть назву мережі"
        ></app-input>
      </div>

      <app-flex class="buttons" justify="flex-end" gap="10">
        <app-button type="outline" @click="close">Скасувати</app-button>
        <app-button :disabled="saveDisabled || !canEdit" @click="save">
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
  .buttons {
    margin-top: 10px;
  }
</style>
