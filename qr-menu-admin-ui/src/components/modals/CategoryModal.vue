<script setup lang="ts">
  import { categoriesApi } from '@/api/categoriesApi';
  import { getErrorMessage } from '@/consts/errorMessages';
  import { useNetworkStore } from '@/store/network';
  import { useToastsStore } from '@/store/toasts';
  import type { CategoryView } from '@/types/categories';
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

  const showed = defineModel<boolean>('showed', { required: true });
  const props = defineProps<{ category?: CategoryView | null }>();
  const emit = defineEmits<{ saved: [] }>();

  const networkStore = useNetworkStore();
  const toasts = useToastsStore();

  const name = ref('');
  const description = ref<string | null>(null);
  const sortOrder = ref<string>('0');
  const isActive = ref(true);

  const isEditMode = computed(() => !!props.category);
  const modalTitle = computed(() =>
    isEditMode.value ? 'Редагувати категорію' : 'Додати категорію',
  );

  const saveDisabled = computed(() => {
    if (!name.value.trim()) return true;
    const so = Number(sortOrder.value);
    if (!Number.isFinite(so)) return true;
    return false;
  });

  watch(showed, (open) => {
    if (!open) return;
    name.value = props.category?.name ?? '';
    description.value = props.category?.description ?? null;
    sortOrder.value = String(props.category?.sortOrder ?? 0);
    isActive.value = props.category?.isActive ?? true;
  });

  const close = () => {
    showed.value = false;
  };

  const save = async () => {
    if (saveDisabled.value) return;

    const networkId = networkStore.network?.id;
    if (!networkId && !props.category) return;

    const payloadBase = {
      name: name.value.trim(),
      description: description.value?.trim() || null,
      sortOrder: Number(sortOrder.value),
      isActive: isActive.value,
    };

    const resp = props.category
      ? await categoriesApi.update(props.category.id, payloadBase)
      : await categoriesApi.create({
          ...payloadBase,
          networkId: networkId!,
        });

    if (!resp.success) {
      toasts.error(getErrorMessage(resp.errorCode));
      return;
    }

    showed.value = false;
    emit('saved');
  };
</script>

<template>
  <app-modal v-model:showed="showed" :title="modalTitle" :width="520">
    <div class="form">
      <div class="input-block">
        <app-label label="Назва" for="categoryName" />
        <app-input v-model="name" id="categoryName" placeholder="Введіть назву" />
      </div>

      <div class="input-block">
        <app-label label="Опис" for="categoryDescription" />
        <app-input
          v-model="description"
          id="categoryDescription"
          placeholder="Введіть опис (необов'язково)"
        />
      </div>

      <div class="input-block">
        <app-label label="Порядковий номер" for="categorySortOrder" />
        <app-input
          v-model="sortOrder"
          id="categorySortOrder"
          placeholder="0"
        />
      </div>

      <app-flex align="center" gap="10" class="active-row">
        <app-checkbox v-model="isActive" />
        <app-text>Активна</app-text>
      </app-flex>
    </div>

    <app-flex class="form-buttons" justify="flex-end" gap="10">
      <app-button type="outline" @click="close">Скасувати</app-button>
      <app-button :disabled="saveDisabled" @click="save">
        {{ isEditMode ? 'Зберегти зміни' : 'Створити категорію' }}
      </app-button>
    </app-flex>
  </app-modal>
</template>

<style scoped>
  .form {
    display: flex;
    flex-direction: column;
    gap: 16px;
  }
  .input-block {
    width: 100%;
  }
  .active-row {
    margin-top: 6px;
  }
  .form-buttons {
    margin-top: 20px;
  }
</style>

