<script lang="ts" setup>
  import { networkApi } from '@/api/networkApi';
  import { PageHeader } from '@/components/headers';
  import {
    AppCard,
    AppIcon,
    AppText,
    type IconName,
  } from '@/components/shared';
  import { getErrorMessage } from '@/consts/errorMessages';
  import { useLoader } from '@/composables';
  import { ROUTES } from '@/router';
  import { useNetworkStore } from '@/store/network';
  import { useToastsStore } from '@/store/toasts';
  import type { DashboardStats } from '@/types/network';
  import { computed, toRef } from 'vue';
  import { useRouter } from 'vue-router';

  interface KpiCardView {
    icon: IconName;
    title: string;
    value: string;
  }

  interface QuickActionView {
    icon: IconName;
    title: string;
    subtitle: string;
    click: () => void;
  }

  const router = useRouter();
  const networkStore = useNetworkStore();
  const toasts = useToastsStore();
  const networkId = toRef(() => networkStore.network?.id);

  const emptyStats: DashboardStats = {
    activeOrdersCount: 0,
    activeUsersCount: 0,
    completedTodayOrdersCount: 0,
    completedTodayTotalSum: 0,
  };

  const { data: stats } = useLoader<DashboardStats>({
    keys: ['dashboard-stats', networkId],
    fn: async () => {
      if (!networkId.value) return emptyStats;
      const resp = await networkApi.getDashboardStats(networkId.value);
      if (!resp.success || !resp.data) {
        toasts.error(getErrorMessage(resp.errorCode));
        return emptyStats;
      }
      return resp.data;
    },
    enabled: () => !!networkId.value,
  });

  const model = computed(() => stats.value ?? emptyStats);

  const kpiCards = computed<KpiCardView[]>(() => [
    {
      icon: 'ShoppingBag',
      title: 'Активних замовлень',
      value: String(model.value.activeOrdersCount),
    },
    {
      icon: 'CheckCircle',
      title: 'Завершено замовлень',
      value: String(model.value.completedTodayOrdersCount),
    },
    {
      icon: 'CircleDollarSign',
      title: 'Прибуток',
      value: model.value.completedTodayTotalSum + ' грн',
    },
    {
      icon: 'Users',
      title: 'Активні користувачі',
      value: String(model.value.activeUsersCount),
    },
  ]);

  const quickActions: QuickActionView[] = [
    {
      icon: 'ShoppingBag',
      title: 'Переглянути замовлення',
      subtitle: 'Активні',
      click: () => {
        router.push({ name: ROUTES.orders });
      },
    },
    {
      icon: 'Plus',
      title: 'Створити замовлення',
      subtitle: 'До замовлень',
      click: () => {
        router.push({ name: ROUTES.orders, query: { openCreateModal: '1' } });
      },
    },
    {
      icon: 'UtensilsCrossed',
      title: 'Додати страву',
      subtitle: 'До меню',
      click: () => {
        router.push({
          name: ROUTES.menuProducts,
          query: { openCreateModal: '1' },
        });
      },
    },
  ];
</script>

<template>
  <div class="page">
    <page-header section-name="Головна"></page-header>

    <app-text size="m" weight="600" class="section-title">
      Статистика за сьогодні
    </app-text>
    <section class="kpi-grid">
      <app-card v-for="card in kpiCards" :key="card.title" class="kpi-card">
        <div class="icon-box">
          <app-icon :name="card.icon" :size="20"></app-icon>
        </div>
        <div class="kpi-texts">
          <app-text size="xs" color="secondary">{{ card.title }}</app-text>
          <app-text size="xl" weight="600">{{ card.value }}</app-text>
        </div>
      </app-card>
    </section>

    <app-text size="m" weight="600" class="section-title">Швидкі дії</app-text>
    <section class="actions-grid">
      <app-card
        v-for="action in quickActions"
        :key="action.title"
        class="action-card"
        @click="action.click"
      >
        <div class="icon-box">
          <app-icon :name="action.icon" :size="20"></app-icon>
        </div>
        <div class="action-texts">
          <app-text weight="600">{{ action.title }}</app-text>
          <app-text color="secondary">{{ action.subtitle }}</app-text>
        </div>
      </app-card>
    </section>
  </div>
</template>

<style scoped>
  .page {
    display: flex;
    flex-direction: column;
    gap: 20px;
  }

  .kpi-grid {
    display: grid;
    grid-template-columns: repeat(4, minmax(0, 1fr));
    gap: 16px;
  }

  .section-title {
    margin-top: 6px;
  }

  .actions-grid {
    display: grid;
    grid-template-columns: repeat(3, minmax(0, 1fr));
    gap: 16px;
  }

  .kpi-card,
  .action-card {
    display: flex;
    align-items: center;
    gap: 12px;
  }

  .action-card {
    cursor: pointer;
    user-select: none;
  }

  .icon-box {
    width: 36px;
    height: 36px;
    border-radius: 10px;
    background-color: var(--secondary-container);
    color: var(--on-secondary-container);
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
  }

  .kpi-texts,
  .action-texts {
    min-width: 0;
    display: flex;
    flex-direction: column;
    gap: 4px;
  }

  @media (max-width: 1100px) {
    .kpi-grid {
      grid-template-columns: repeat(2, minmax(0, 1fr));
    }
  }

  @media (max-width: 768px) {
    .page {
      gap: 12px;
    }
    .kpi-grid,
    .actions-grid {
      grid-template-columns: 1fr;
      gap: 12px;
    }
  }
</style>
