<script setup lang="ts">
  import { productsApi } from '@/api/productsApi';
  import type { CategoryView } from '@/types/categories';
  import type { Establishment } from '@/types/network';
  import type { ProductPriceItem, ProductView } from '@/types/products';
  import { computed, ref, watch } from 'vue';
  import {
    AppButton,
    AppCheckbox,
    AppFileDropzone,
    AppFlex,
    AppIcon,
    AppInput,
    AppLabel,
    AppModal,
    AppSelect,
    AppText,
  } from '../shared';
  import { useNetworkStore } from '@/store/network';
  import { useToastsStore } from '@/store/toasts';
  import { getErrorMessage } from '@/consts/errorMessages';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';

  interface PriceRow {
    establishmentId: number;
    name: string;
    enabled: boolean;
    priceText: string;
  }

  const showed = defineModel<boolean>('showed', { required: true });
  const props = defineProps<{
    product?: ProductView | null;
    categories: CategoryView[];
  }>();
  const emit = defineEmits<{ saved: [] }>();

  const networkStore = useNetworkStore();
  const toasts = useToastsStore();
  const { hasAny } = usePermissions();

  const canEdit = computed(() => hasAny(PermissionType.productsEdit));

  const name = ref('');
  const description = ref('');
  const photoUrls = ref<string[]>([]);
  const uploadingPhotos = ref(false);
  const selectedCategory = ref<CategoryView | null>(null);

  const rows = ref<PriceRow[]>([]);

  const isEditMode = computed(() => !!props.product);
  const modalTitle = computed(() =>
    isEditMode.value ? 'Редагувати страву' : 'Додати страву',
  );

  const networkId = computed(() => networkStore.network?.id ?? null);
  const establishments = computed<Establishment[]>(
    () => networkStore.network?.establishments ?? [],
  );

  const saveDisabled = computed(() => {
    if (!name.value.trim()) return true;
    if (!description.value.trim()) return true;
    if (!selectedCategory.value) return true;
    if (!networkId.value && !props.product) return true;
    if (photoUrls.value.some((p) => p.trim().length > 1000)) return true;
    if (uploadingPhotos.value) return true;

    // validate prices
    for (const r of rows.value) {
      const normalized = r.priceText.replace(',', '.').trim();
      const price = Number(normalized);
      if (!Number.isFinite(price) || price < 0) return true;
    }
    return false;
  });

  function buildDefaultRows(): PriceRow[] {
    return establishments.value.map((e) => ({
      establishmentId: e.id,
      name: e.name,
      enabled: true,
      priceText: '0',
    }));
  }

  function rowsFromProduct(product: ProductView): PriceRow[] {
    const byEst = new Map(product.prices.map((p) => [p.establishmentId, p]));
    return establishments.value.map((e) => {
      const pp = byEst.get(e.id);
      return {
        establishmentId: e.id,
        name: e.name,
        enabled: pp?.isActive ?? true,
        priceText: String(pp?.price ?? 0),
      };
    });
  }

  watch(
    [showed, establishments],
    ([open]) => {
      if (!open) return;

      name.value = props.product?.name ?? '';
      description.value = props.product?.description ?? '';
      photoUrls.value = [...(props.product?.photoUrls ?? [])];

      const catId = props.product?.categoryId ?? null;
      selectedCategory.value =
        props.categories.find((c) => c.id === catId) ??
        props.categories[0] ??
        null;

      rows.value = props.product
        ? rowsFromProduct(props.product)
        : buildDefaultRows();
    },
    { immediate: false },
  );

  const close = () => {
    showed.value = false;
  };

  function toPriceItems(): ProductPriceItem[] {
    return rows.value.map((r) => ({
      establishmentId: r.establishmentId,
      isActive: r.enabled,
      price: Number(r.priceText.replace(',', '.').trim()) || 0,
    }));
  }

  function removePhoto(index: number) {
    photoUrls.value.splice(index, 1);
  }

  function normalizedPhotoUrls() {
    return photoUrls.value
      .map((p) => p.trim())
      .filter((p) => p.length > 0);
  }

  async function uploadPhoto(file: File) {
    const resp = await productsApi.uploadPhoto(file);
    if (!resp.success || !resp.data?.url) {
      toasts.error(getErrorMessage(resp.errorCode));
      return;
    }
    photoUrls.value.push(resp.data.url);
  }

  async function onFilesSelected(files: File[]) {
    if (!files.length) return;
    uploadingPhotos.value = true;
    try {
      for (const file of files) {
        await uploadPhoto(file);
      }
    } finally {
      uploadingPhotos.value = false;
    }
  }

  const save = async () => {
    if (saveDisabled.value) return;
    if (!selectedCategory.value) return;
    if (!canEdit.value) return;

    const payloadCommon = {
      name: name.value.trim(),
      description: description.value.trim(),
      categoryId: selectedCategory.value.id,
      photoUrls: normalizedPhotoUrls(),
      prices: toPriceItems(),
    };

    const resp = props.product
      ? await productsApi.update(props.product.id, payloadCommon)
      : await productsApi.create({
          ...payloadCommon,
          networkId: networkId.value!,
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
  <app-modal v-model:showed="showed" :title="modalTitle" :width="650">
    <div class="form">
      <div class="input-block">
        <app-label label="Назва" for="productName" />
        <app-input
          v-model="name"
          id="productName"
          placeholder="Введіть назву"
        ></app-input>
      </div>

      <div class="input-block">
        <app-label label="Опис" for="productDescription" />
        <app-input
          v-model="description"
          id="productDescription"
          placeholder="Введіть опис"
        ></app-input>
      </div>

      <div class="input-block">
        <app-label label="Категорія" />
        <app-select
          v-model="selectedCategory"
          :options="categories"
          option-label="name"
          placeholder="Оберіть категорію"
        ></app-select>
      </div>

      <div class="photos">
        <app-text weight="600">Фото страви</app-text>

        <div v-if="photoUrls.length" class="photo-rows">
          <div v-for="(url, idx) in photoUrls" :key="url + idx" class="photo-row">
            <img :src="url" :alt="`Фото ${idx + 1}`" class="photo-preview" />
            <app-text class="photo-url" size="xs" line="m">{{ url }}</app-text>
            <button type="button" class="photo-remove" @click="removePhoto(idx)">
              <app-icon name="Trash2" :size="16" />
            </button>
          </div>
        </div>

        <app-file-dropzone
          :disabled="uploadingPhotos"
          @select="onFilesSelected"
        ></app-file-dropzone>
        <app-text v-if="uploadingPhotos" size="xs" color="secondary">
          Завантаження фото...
        </app-text>
      </div>

      <div class="prices">
        <app-text weight="600">Відображення та ціна у закладах</app-text>
        <div class="rows">
          <div v-for="row in rows" :key="row.establishmentId" class="row">
            <app-flex align="center" gap="12" class="row-content">
              <app-checkbox v-model="row.enabled" />

              <app-text class="est-name" :class="{ disabled: !row.enabled }">
                {{ row.name }}
              </app-text>

              <div class="price">
                <app-input
                  v-if="row.enabled"
                  v-model="row.priceText"
                  placeholder="0"
                ></app-input>
                <app-text v-else color="secondary">—</app-text>
              </div>
            </app-flex>
          </div>
        </div>
      </div>
    </div>

    <app-flex class="form-buttons" justify="flex-end" gap="10">
      <app-button type="outline" @click="close">Скасувати</app-button>
      <app-button :disabled="saveDisabled || !canEdit" @click="save">
        {{ isEditMode ? 'Зберегти зміни' : 'Створити страву' }}
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

  .prices {
    display: flex;
    flex-direction: column;
    gap: 10px;
    border-top: 1px solid var(--outline-variant);
    padding-top: 15px;
    margin-top: 5px;
  }

  .photos {
    display: flex;
    flex-direction: column;
    gap: 10px;
  }

  .photo-rows {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }

  .photo-row {
    display: grid;
    grid-template-columns: auto 1fr auto;
    gap: 8px;
    align-items: center;
    padding: 8px;
    border: 1px solid var(--outline-variant);
    border-radius: 10px;
    background: var(--surface-container-lowest);
  }

  .photo-url {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .photo-remove {
    width: 36px;
    height: 36px;
    border-radius: 8px;
    border: 1px solid var(--outline-variant);
    background: var(--surface-container-lowest);
    color: var(--on-surface);
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
  }

  .photo-preview {
    width: 52px;
    height: 52px;
    border-radius: 10px;
    object-fit: cover;
    border: 1px solid var(--outline-variant);
    background: var(--surface-container-lowest);
  }

  .rows {
    display: flex;
    flex-direction: column;
    gap: 10px;
  }

  .row-content {
    width: 100%;
  }

  .est-name.disabled {
    opacity: 0.6;
  }

  .price {
    width: 160px;
  }

  .form-buttons {
    margin-top: 20px;
  }

  @media (max-width: 700px) {
    .row-content {
      flex-wrap: wrap;
      align-items: flex-start;
      gap: 8px;
    }
    .price {
      width: 100%;
    }
  }

  @media (max-width: 576px) {
    .form-buttons {
      flex-direction: column;
      margin-top: 14px;
    }
    .form-buttons :deep(.app-button) {
      width: 100%;
      justify-content: center;
    }
  }
</style>
