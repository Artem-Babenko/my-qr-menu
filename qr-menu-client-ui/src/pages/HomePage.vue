<script setup lang="ts">
  import { useRouter, useRoute } from 'vue-router';
  import { ROUTES } from '@/router';
  import { AppIcon, AppText } from '@/components/shared';
  import { useTableStore } from '@/store/table';

  const router = useRouter();
  const route = useRoute();
  const tableStore = useTableStore();

  const tableId = Number(route.params.tableId);

  function go(name: string) {
    router.push({ name, params: { tableId } });
  }
</script>

<template>
  <div class="home-page">
    <div v-if="tableStore.table" class="home-header">
      <app-text size="xxl" weight="600" line="m">
        {{ tableStore.table.establishmentName }}
      </app-text>
      <app-text size="s" color="secondary" line="m">
        {{ tableStore.table.establishmentAddress }}
      </app-text>
      <app-text size="s" color="secondary" line="m">
        Стіл №{{ tableStore.table.tableNumber }}
      </app-text>
    </div>

    <div v-else class="home-header">
      <app-text size="l" color="secondary">Завантаження...</app-text>
    </div>

    <div class="home-actions">
      <button class="action-card" @click="go(ROUTES.menu)">
        <app-icon name="UtensilsCrossed" :size="28" />
        <app-text size="l" weight="600">Меню</app-text>
        <app-text size="xs" color="secondary" line="m">
          Переглянути страви
        </app-text>
      </button>

      <button class="action-card" @click="go(ROUTES.basket)">
        <app-icon name="ShoppingCart" :size="28" />
        <app-text size="l" weight="600">Кошик</app-text>
        <app-text size="xs" color="secondary" line="m">
          Оформити замовлення
        </app-text>
      </button>

      <button class="action-card" @click="go(ROUTES.orders)">
        <app-icon name="ClipboardList" :size="28" />
        <app-text size="l" weight="600">Замовлення</app-text>
        <app-text size="xs" color="secondary" line="m">
          Переглянути статус
        </app-text>
      </button>
    </div>
  </div>
</template>

<style scoped>
  .home-page {
    display: flex;
    flex-direction: column;
    gap: var(--gap-lg);
  }

  .home-header {
    display: flex;
    flex-direction: column;
    gap: clamp(4px, 1vmin, 8px);
    padding-top: clamp(8px, 2vmin, 16px);
    padding-bottom: clamp(12px, 3vmin, 20px);
  }

  .home-actions {
    display: flex;
    flex-direction: column;
    gap: var(--gap);
  }

  .action-card {
    display: flex;
    flex-direction: column;
    align-items: flex-start;
    gap: clamp(4px, 1vmin, 8px);
    padding: var(--card-padding);
    background: var(--app-card-bg);
    border: none;
    border-radius: var(--radius-card);
    box-shadow: var(--app-card-shadow);
    cursor: pointer;
    transition:
      background-color 0.2s ease,
      box-shadow 0.2s ease;
    text-align: left;
    outline: none;
    position: relative;
    overflow: hidden;
  }

  .action-card::after {
    content: '';
    position: absolute;
    inset: 0;
    background-color: currentColor;
    opacity: 0;
    transition: opacity 0.2s ease;
    pointer-events: none;
  }
  .action-card:hover::after {
    opacity: 0.08;
  }
  .action-card:active::after {
    opacity: 0.12;
  }

</style>
