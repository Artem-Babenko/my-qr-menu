<script setup lang="ts">
  import { categoriesApi } from '@/api/categoriesApi';
  import { CategoryCard } from '@/components/cards';
  import { CategoryModal } from '@/components/modals';
  import {
    AppButton,
    AppFlex,
    AppSearchInput,
    AppText,
  } from '@/components/shared';
  import { getErrorMessage } from '@/consts/errorMessages';
  import { useNetworkStore } from '@/store/network';
  import { useToastsStore } from '@/store/toasts';
  import type { CategoryView } from '@/types/categories';
  import { computed, ref, watch } from 'vue';

  const toasts = useToastsStore();
  const networkStore = useNetworkStore();
  const search = ref('');

  const categories = ref<CategoryView[]>([]);
  const loading = ref(false);

  const modalShowed = ref(false);
  const editingCategory = ref<CategoryView | null>(null);

  const networkId = computed(() => networkStore.network?.id ?? null);

  const filtered = computed(() => {
    const q = search.value.trim().toLowerCase();
    const list = categories.value
      .slice()
      .sort((a, b) => a.sortOrder - b.sortOrder);
    if (!q) return list;
    return list.filter((c) => {
      const hay = `${c.name} ${c.description ?? ''}`.toLowerCase();
      return hay.includes(q);
    });
  });

  const loadCategories = async () => {
    const id = networkId.value;
    if (!id) return;

    loading.value = true;
    try {
      const resp = await categoriesApi.byNetwork(id);
      if (!resp.success) {
        toasts.error(getErrorMessage(resp.errorCode));
        categories.value = [];
        return;
      }
      categories.value = resp.data ?? [];
    } finally {
      loading.value = false;
    }
  };

  const openCreate = async () => {
    editingCategory.value = null;
    modalShowed.value = true;
  };

  const openEdit = (category: CategoryView) => {
    editingCategory.value = category;
    modalShowed.value = true;
  };

  const onDelete = async (category: CategoryView) => {
    const confirmed = window.confirm(`Видалити категорію "${category.name}"?`);
    if (!confirmed) return;
    const resp = await categoriesApi.delete(category.id);
    if (!resp.success) {
      toasts.error(getErrorMessage(resp.errorCode));
      return;
    }
    await loadCategories();
  };

  const onSaved = async () => {
    await loadCategories();
  };

  watch(
    networkId,
    (id) => {
      if (!id) return;
      loadCategories();
    },
    { immediate: true },
  );
</script>

<template>
  <div class="tab">
    <app-flex class="controls" align="center" gap="20">
      <div class="search">
        <app-search-input v-model="search" placeholder="Пошук категорій..." />
      </div>
      <app-button class="add-btn" :disabled="!networkId" @click="openCreate">
        Додати категорію
      </app-button>
    </app-flex>

    <app-text v-if="loading" color="secondary">Завантаження...</app-text>

    <div v-else class="list">
      <category-card
        v-for="cat in filtered"
        :key="cat.id"
        :category="cat"
        @edit="openEdit"
        @delete="onDelete"
      ></category-card>
      <app-text v-if="filtered.length === 0" color="secondary">
        Нічого не знайдено
      </app-text>
    </div>

    <category-modal
      v-model:showed="modalShowed"
      :category="editingCategory"
      @saved="onSaved"
    ></category-modal>
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
    min-width: 200px;
    white-space: nowrap;
  }

  .list {
    display: flex;
    flex-direction: column;
    gap: 10px;
  }
</style>
