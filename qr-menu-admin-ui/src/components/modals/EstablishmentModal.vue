<script lang="ts" setup>
  import { networkApi } from '@/api/networkApi';
  import { useToastsStore } from '@/store/toasts';
  import type { Establishment } from '@/types/network';
  import { computed, ref, watch } from 'vue';
  import { AppButton, AppFlex, AppInput, AppLabel, AppModal } from '../shared';
  import { getErrorMessage } from '@/consts/errorMessages';

  const showed = defineModel<boolean>('showed', { required: true });
  const props = defineProps<{ establishment: Establishment | null }>();
  const emit = defineEmits<{ saved: [] }>();

  const toasts = useToastsStore();

  const name = ref<string | null>(null);
  const address = ref<string | null>(null);
  const saving = ref(false);

  const saveDisabled = computed(() => {
    if (saving.value) return true;
    if (!name.value?.trim()) return true;
    if (!address.value?.trim()) return true;
    return false;
  });

  watch(showed, (v) => {
    if (!v) return;
    name.value = props.establishment?.name ?? '';
    address.value = props.establishment?.address ?? '';
    saving.value = false;
  });

  const close = () => (showed.value = false);

  const save = async () => {
    if (!props.establishment || saveDisabled.value) return;
    saving.value = true;
    const resp = await networkApi.updateEstablishment(props.establishment.id, {
      name: name.value!.trim(),
      address: address.value!.trim(),
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
  <app-modal v-model:showed="showed" title="Налаштування закладу" :width="620">
    <div class="form">
      <div class="input-block">
        <app-label label="Назва" for="estName"></app-label>
        <app-input
          v-model="name"
          id="estName"
          placeholder="Введіть назву закладу"
        ></app-input>
      </div>

      <div class="input-block">
        <app-label label="Адреса" for="estAddress"></app-label>
        <app-input
          v-model="address"
          id="estAddress"
          placeholder="Введіть адресу"
        ></app-input>
      </div>

      <app-flex class="buttons" justify="flex-end" gap="10">
        <app-button type="outline" @click="close">Скасувати</app-button>
        <app-button :disabled="saveDisabled" @click="save">Зберегти</app-button>
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
