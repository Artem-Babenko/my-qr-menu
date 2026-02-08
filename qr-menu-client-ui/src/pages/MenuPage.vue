<script setup lang="ts">
  import { ref, computed, nextTick } from 'vue';
  import { useRoute } from 'vue-router';
  import { AppButton, AppIcon, AppText, AppCard } from '@/components/shared';
  import PageHeader from '@/components/headers/PageHeader.vue';
  import { useTableStore } from '@/store/table';
  import { useBasketStore } from '@/store/basket';
  import { clientApi } from '@/api/clientApi';
  import { useLoader } from '@/composables/useLoader';
  import type { MenuCategory } from '@/types/menu';

  const route = useRoute();
  const tableStore = useTableStore();
  const basketStore = useBasketStore();

  const establishmentId = computed(() => tableStore.table?.establishmentId ?? 0);

  const { data: categories, loading } = useLoader<MenuCategory[] | null>({
    keys: [establishmentId],
    fn: async () => {
      if (!establishmentId.value) return null;
      const resp = await clientApi.getMenu(establishmentId.value);
      return resp.success ? resp.data : null;
    },
    enabled: () => establishmentId.value > 0,
  });

  const activeCategory = ref<number | null>(null);

  function scrollToCategory(categoryId: number) {
    activeCategory.value = categoryId;
    const el = document.getElementById(`category-${categoryId}`);
    if (el) {
      el.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  function addToBasket(product: { id: number; name: string; price: number }) {
    basketStore.add(product);
  }
</script>

<template>
  <div class="menu-page">
    <page-header title="Меню" subtitle="Оберіть страви та додайте до кошика" />

    <div v-if="loading" class="loading">
      <app-text size="s" color="secondary">Завантаження меню...</app-text>
    </div>

    <template v-else-if="categories && categories.length > 0">
      <div class="category-tabs">
        <button
          v-for="cat in categories"
          :key="cat.id"
          class="category-chip"
          :class="{ active: activeCategory === cat.id }"
          @click="scrollToCategory(cat.id)"
        >
          {{ cat.name }}
        </button>
      </div>

      <div class="menu-sections">
        <div
          v-for="cat in categories"
          :key="cat.id"
          :id="`category-${cat.id}`"
          class="menu-section"
        >
          <div class="section-header">
            <app-text size="l" weight="600" line="m">{{ cat.name }}</app-text>
            <app-text
              v-if="cat.description"
              size="xs"
              color="secondary"
              line="m"
            >
              {{ cat.description }}
            </app-text>
          </div>

          <div class="products-list">
            <app-card
              v-for="product in cat.products"
              :key="product.id"
              class="product-card"
            >
              <div class="product-info">
                <app-text size="m" weight="600" line="m">
                  {{ product.name }}
                </app-text>
                <app-text
                  v-if="product.description"
                  size="xs"
                  color="secondary"
                  line="m"
                  class="product-desc"
                >
                  {{ product.description }}
                </app-text>
                <app-text size="m" weight="600">
                  {{ product.price }} ₴
                </app-text>
              </div>

              <div class="product-action">
                <app-button
                  v-if="basketStore.getQuantity(product.id) === 0"
                  type="filled"
                  @click="addToBasket(product)"
                >
                  <app-icon name="Plus" :size="16" />
                  Додати
                </app-button>
                <div v-else class="qty-controls">
                  <button
                    class="qty-btn"
                    @click="basketStore.decrement(product.id)"
                  >
                    <app-icon name="Minus" :size="16" />
                  </button>
                  <app-text size="m" weight="600">
                    {{ basketStore.getQuantity(product.id) }}
                  </app-text>
                  <button
                    class="qty-btn"
                    @click="basketStore.increment(product.id)"
                  >
                    <app-icon name="Plus" :size="16" />
                  </button>
                </div>
              </div>
            </app-card>
          </div>
        </div>
      </div>
    </template>

    <div v-else class="empty">
      <app-text size="s" color="secondary">Меню порожнє</app-text>
    </div>
  </div>
</template>

<style scoped>
  .menu-page {
    display: flex;
    flex-direction: column;
  }

  .loading,
  .empty {
    display: flex;
    justify-content: center;
    padding: var(--gap-lg) 0;
  }

  .category-tabs {
    display: flex;
    gap: clamp(6px, 1.5vw, 8px);
    overflow-x: auto;
    padding-bottom: var(--gap);
    scrollbar-width: none;
    -ms-overflow-style: none;
  }

  .category-tabs::-webkit-scrollbar {
    display: none;
  }

  .category-chip {
    flex-shrink: 0;
    padding: clamp(6px, 1.5vw, 8px) clamp(12px, 3vw, 16px);
    border-radius: 20px;
    border: 1px solid var(--border);
    background: #ffffff;
    font: var(--font-xs);
    font-weight: 500;
    cursor: pointer;
    transition: all 0.2s ease;
    white-space: nowrap;
    outline: none;
  }

  .category-chip.active,
  .category-chip:hover {
    background: var(--primary);
    color: var(--text-on-primary);
    border-color: var(--primary);
  }

  .menu-sections {
    display: flex;
    flex-direction: column;
    gap: var(--gap-lg);
  }

  .section-header {
    display: flex;
    flex-direction: column;
    gap: 2px;
    margin-bottom: var(--gap);
  }

  .products-list {
    display: flex;
    flex-direction: column;
    gap: var(--gap);
  }

  .product-card {
    display: flex;
    justify-content: space-between;
    align-items: flex-end;
    gap: var(--gap);
  }

  .product-info {
    display: flex;
    flex-direction: column;
    gap: clamp(2px, 0.5vmin, 4px);
    flex: 1;
    min-width: 0;
  }

  .product-desc {
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
  }

  .product-action {
    flex-shrink: 0;
  }

  .qty-controls {
    display: flex;
    align-items: center;
    gap: clamp(8px, 2vw, 12px);
  }

  .qty-btn {
    width: clamp(32px, 8vmin, 40px);
    height: clamp(32px, 8vmin, 40px);
    border-radius: var(--radius-sm);
    border: 1px solid var(--border);
    background: #ffffff;
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    transition: all 0.2s ease;
    outline: none;
  }

  .qty-btn:hover {
    border-color: var(--primary);
  }

  .qty-btn:active {
    background: var(--hover-on-secondary);
  }
</style>
