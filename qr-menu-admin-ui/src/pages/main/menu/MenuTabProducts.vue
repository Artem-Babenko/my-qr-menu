<script setup lang="ts">
  import { categoriesApi } from '@/api/categoriesApi';
  import { productsApi } from '@/api/productsApi';
  import { ProductCard } from '@/components/cards';
  import { ProductModal } from '@/components/modals';
  import BaseCardList from '@/components/lists/BaseCardList.vue';
  import {
    AppButton,
    AppFlex,
    AppSearchInput,
    AppText,
  } from '@/components/shared';
  import { getErrorMessage } from '@/consts/errorMessages';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';
  import { useNetworkStore } from '@/store/network';
  import { useToastsStore } from '@/store/toasts';
  import type { CategoryView } from '@/types/categories';
  import type { ProductView } from '@/types/products';
  import { computed, ref, watch } from 'vue';

  const toasts = useToastsStore();
  const networkStore = useNetworkStore();
  const { hasAny } = usePermissions();

  const search = ref('');
  const categories = ref<CategoryView[]>([]);
  const products = ref<ProductView[]>([]);
  const loading = ref(false);
  const modalShowed = ref(false);
  const editingProduct = ref<ProductView | null>(null);

  const networkId = computed(() => networkStore.network?.id ?? null);
  const canView = computed(() => hasAny(PermissionType.productsView));
  const canCreate = computed(() => hasAny(PermissionType.productsCreate));
  const canEdit = computed(() => hasAny(PermissionType.productsEdit));
  const canDelete = computed(() => hasAny(PermissionType.productsDelete));

  const categoriesSorted = computed(() =>
    categories.value.slice().sort((a, b) => a.sortOrder - b.sortOrder),
  );

  const filteredProducts = computed(() => {
    const q = search.value.trim().toLowerCase();
    if (!q) return products.value;
    return products.value.filter((p) => {
      const hay = `${p.name} ${p.description}`.toLowerCase();
      return hay.includes(q);
    });
  });

  const productsByCategory = computed(() => {
    const map = new Map<number, ProductView[]>();
    for (const p of filteredProducts.value) {
      const list = map.get(p.categoryId) ?? [];
      list.push(p);
      map.set(p.categoryId, list);
    }
    for (const [k, v] of map.entries()) {
      map.set(
        k,
        v.slice().sort((a, b) => a.name.localeCompare(b.name)),
      );
    }
    return map;
  });

  const loadData = async () => {
    const id = networkId.value;
    if (!id) return;
    if (!canView.value) return;
    loading.value = true;
    try {
      const [cats, prods] = await Promise.all([
        categoriesApi.byNetwork(id),
        productsApi.byNetwork(id),
      ]);
      if (!cats.success) {
        toasts.error(getErrorMessage(cats.errorCode));
        categories.value = [];
      } else {
        categories.value = cats.data ?? [];
      }
      if (!prods.success) {
        toasts.error(getErrorMessage(prods.errorCode));
        products.value = [];
      } else {
        products.value = prods.data ?? [];
      }
    } finally {
      loading.value = false;
    }
  };

  watch(
    networkId,
    (id) => {
      if (!id) return;
      loadData();
    },
    { immediate: true },
  );

  const openCreate = () => {
    if (!canCreate.value) return;
    editingProduct.value = null;
    modalShowed.value = true;
  };

  const openEdit = (product: ProductView) => {
    if (!canEdit.value) return;
    editingProduct.value = product;
    modalShowed.value = true;
  };

  const onDelete = async (product: ProductView) => {
    if (!canDelete.value) return;
    const confirmed = window.confirm(`Видалити страву "${product.name}"?`);
    if (!confirmed) return;
    const resp = await productsApi.delete(product.id);
    if (!resp.success) {
      toasts.error(getErrorMessage(resp.errorCode));
      return;
    }
    await loadData();
  };

  const onSaved = async () => {
    await loadData();
  };
</script>

<template>
  <div class="tab">
    <app-text v-if="!canView" color="secondary">
      Недостатньо прав для перегляду меню
    </app-text>
    <template v-else>
      <app-flex class="controls" align="center" gap="20">
        <div class="search">
          <app-search-input v-model="search" placeholder="Пошук страв..." />
        </div>
        <app-button
          class="add-btn"
          :disabled="!networkId || !canCreate"
          @click="openCreate"
        >
          Додати страву
        </app-button>
      </app-flex>

      <app-text v-if="loading" color="secondary">Завантаження...</app-text>

      <div v-else class="groups">
        <div
          v-for="cat in categoriesSorted"
          :key="cat.id"
          class="group"
          v-show="(productsByCategory.get(cat.id)?.length ?? 0) > 0"
        >
          <app-flex
            direction="column"
            align="flex-start"
            :gap="6"
            class="group-head"
          >
            <app-text weight="600">{{ cat.name }}</app-text>
            <app-text
              v-if="cat.description"
              color="secondary"
              size="xs"
              line="m"
            >
              {{ cat.description }}
            </app-text>
          </app-flex>

          <base-card-list>
            <product-card
              v-for="p in productsByCategory.get(cat.id) ?? []"
              :key="p.id"
              :product="p"
              :establishments="networkStore.network?.establishments ?? []"
              :readonly="!canEdit"
              @edit="openEdit"
              @delete="onDelete"
            ></product-card>
          </base-card-list>
        </div>

        <app-text v-if="filteredProducts.length === 0" color="secondary">
          Нічого не знайдено
        </app-text>
      </div>

      <product-modal
        v-model:showed="modalShowed"
        :product="editingProduct"
        :categories="categoriesSorted"
        @saved="onSaved"
      ></product-modal>
    </template>
  </div>
</template>

<style scoped>
  .tab {
    display: flex;
    flex-direction: column;
    gap: 15px;
  }
  .controls {
    width: 100%;
  }
  .search {
    flex: 1;
  }
  :deep(.search .app-search-input) {
    width: 100%;
  }
  .add-btn {
    min-width: 180px;
    white-space: nowrap;
  }

  .groups {
    display: flex;
    flex-direction: column;
    gap: 22px;
  }

  .group-head {
    padding-bottom: 10px;
    border-bottom: 1px solid var(--outline-variant);
    margin-bottom: 12px;
  }
</style>
