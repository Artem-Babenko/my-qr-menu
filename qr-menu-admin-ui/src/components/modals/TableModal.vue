<script setup lang="ts">
  import { tablesApi } from '@/api/tablesApi';
  import { getErrorMessage } from '@/consts/errorMessages';
  import { useToastsStore } from '@/store/toasts';
  import type { TableView } from '@/types/tables';
  import { computed, ref, watch } from 'vue';
  import { AppButton, AppFlex, AppInput, AppLabel, AppModal } from '../shared';

  const showed = defineModel<boolean>('showed', { required: true });
  const props = defineProps<{
    establishmentId: number | null;
    table?: TableView | null;
  }>();
  const emit = defineEmits<{ saved: [] }>();

  const toasts = useToastsStore();

  const number = ref('');

  const isEditMode = computed(() => !!props.table);
  const modalTitle = computed(() =>
    isEditMode.value ? 'Редагувати стіл' : 'Додати стіл',
  );
  const saveDisabled = computed(() => !number.value.trim());

  watch(showed, (newValue) => {
    if (!newValue) return;
    number.value = props.table?.number ?? '';
  });

  const close = () => {
    showed.value = false;
  };

  const save = async () => {
    if (saveDisabled.value) return;

    const trimmed = number.value.trim();

    const resp =
      isEditMode.value && props.table
        ? await tablesApi.update(props.table.id, { number: trimmed })
        : props.establishmentId
          ? await tablesApi.create({
              establishmentId: props.establishmentId,
              number: trimmed,
            })
          : null;

    if (!resp) return;
    if (!resp.success) {
      toasts.error(getErrorMessage(resp.errorCode));
      return;
    }

    showed.value = false;
    emit('saved');
  };
</script>

<template>
  <app-modal v-model:showed="showed" :title="modalTitle" :width="420">
    <div class="form">
      <div class="input-block">
        <app-label label="Номер столу" for="tableNumber"></app-label>
        <app-input
          v-model="number"
          id="tableNumber"
          placeholder="Введіть номер столу"
        ></app-input>
      </div>
    </div>

    <app-flex class="form-buttons" justify="flex-end" gap="10">
      <app-button type="outline" @click="close">Скасувати</app-button>
      <app-button :disabled="saveDisabled" @click="save">
        {{ isEditMode ? 'Зберегти зміни' : 'Створити стіл' }}
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

  .form-buttons {
    margin-top: 20px;
  }
</style>
