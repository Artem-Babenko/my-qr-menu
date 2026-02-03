<script lang="ts" setup>
  import { networkApi } from '@/api/networkApi';
  import { useNetworkStore } from '@/store/network';
  import { useToastsStore } from '@/store/toasts';
  import { computed, reactive, ref, watch } from 'vue';
  import {
    AppButton,
    AppFlex,
    AppInput,
    AppLabel,
    AppModal,
    AppText,
  } from '../shared';
  import { getErrorMessage } from '@/consts/errorMessages';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';

  const showed = defineModel<boolean>('showed', { required: true });
  const props = defineProps<{ currentCount: number }>();
  const emit = defineEmits<{ saved: [] }>();

  const networkStore = useNetworkStore();
  const toasts = useToastsStore();
  const { hasAny } = usePermissions();

  const model = reactive({
    networkName: '',
    name: '',
    address: '',
  });

  const saving = ref(false);

  const isSecond = computed(() => props.currentCount === 1);

  const canCreate = computed(() => hasAny(PermissionType.establishmentsCreate));

  const saveDisabled = computed(() => {
    if (saving.value) return true;
    if (isSecond.value && !model.networkName.trim()) return true;
    if (!model.name.trim()) return true;
    if (!model.address.trim()) return true;
    return false;
  });

  watch(showed, (v) => {
    if (!v) return;
    model.networkName = networkStore.network?.name ?? '';
    model.name = '';
    model.address = '';
  });

  const close = () => (showed.value = false);

  const save = async () => {
    if (saveDisabled.value) return;
    if (!canCreate.value && !!networkStore.network?.id) return;
    saving.value = true;
    const payload = {
      name: model.name.trim(),
      address: model.address.trim(),
      networkName: isSecond.value ? model.networkName.trim() : null,
    };

    const resp = await networkApi.createEstablishment(payload);
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
  <app-modal v-model:showed="showed" title="Додати заклад" :width="620">
    <div class="form">
      <app-text v-if="isSecond" color="secondary" line="m">
        Ви створюєте другий заклад. Будь ласка, задайте назву мережі та дані
        закладу.
      </app-text>

      <div v-if="isSecond" class="input-block">
        <app-label label="Назва мережі" for="createNetworkName"></app-label>
        <app-input
          v-model="model.networkName"
          id="createNetworkName"
          placeholder="Введіть назву мережі"
        ></app-input>
      </div>

      <div class="input-block">
        <app-label label="Назва закладу" for="createEstName"></app-label>
        <app-input
          v-model="model.name"
          id="createEstName"
          placeholder="Введіть назву закладу"
        ></app-input>
      </div>

      <div class="input-block">
        <app-label label="Адреса" for="createEstAddress"></app-label>
        <app-input
          v-model="model.address"
          id="createEstAddress"
          placeholder="Введіть адресу"
        ></app-input>
      </div>

      <app-flex class="buttons" justify="flex-end" gap="10">
        <app-button type="outline" @click="close">Скасувати</app-button>
        <app-button
          :disabled="saveDisabled || (!canCreate && !!networkStore.network?.id)"
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
  .buttons {
    margin-top: 10px;
  }
</style>
