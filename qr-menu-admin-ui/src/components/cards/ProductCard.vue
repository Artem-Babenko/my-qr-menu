<script setup lang="ts">
  import type { Establishment } from '@/types/network';
  import type { ProductView } from '@/types/products';
  import { computed } from 'vue';
  import { AppCard, AppFlex, AppText } from '../shared';
  import { CardDropdown } from '../dropdowns';
  import type { ActionButton } from '../dropdowns';

  const props = defineProps<{
    product: ProductView;
    establishments?: Establishment[];
  }>();
  const emit = defineEmits<{
    edit: [product: ProductView];
    delete: [product: ProductView];
  }>();

  const activePrices = computed(() =>
    props.product.prices.filter((p) => p.isActive),
  );

  const activeCount = computed(() => activePrices.value.length);

  const priceLabel = computed(() => {
    const act = activePrices.value;
    if (act.length === 0) return 'Не відображається';

    const prices = act.map((p) => p.price ?? 0);
    const min = Math.min(...prices);
    const max = Math.max(...prices);

    if (min === 0 && max === 0) return 'Безкоштовно';
    if (min !== max) return `від ₴${min}`;
    return `₴${min}`;
  });

  const buttons = computed<ActionButton[]>(() => [
    {
      icon: 'Pencil',
      title: 'Редагувати',
      click: () => emit('edit', props.product),
    },
    {
      icon: 'Trash',
      title: 'Видалити',
      click: () => emit('delete', props.product),
    },
  ]);
</script>

<template>
  <app-card class="product-card">
    <app-flex justify="space-between" align="flex-start" :gap="10">
      <app-flex direction="column" align="flex-start" :gap="8" class="info">
        <app-text weight="600" class="title">{{ product.name }}</app-text>
        <app-text color="secondary" size="xs" line="m" class="desc">
          {{ product.description }}
        </app-text>
      </app-flex>

      <card-dropdown :buttons="buttons"></card-dropdown>
    </app-flex>

    <app-flex justify="space-between" align="center" class="bottom">
      <app-text weight="600">{{ priceLabel }}</app-text>
      <app-text color="secondary" size="xs">
        В {{ activeCount }} закладах
      </app-text>
    </app-flex>
  </app-card>
</template>

<style scoped>
  .product-card {
    height: 170px;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
  }

  .info {
    min-width: 0;
  }

  .title,
  .desc {
    max-width: 100%;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .desc {
    display: -webkit-box;
    -webkit-box-orient: vertical;
    -webkit-line-clamp: 3;
    overflow: hidden;
  }

  .bottom {
    border-top: 1px solid var(--border);
    padding-top: 10px;
  }
</style>

