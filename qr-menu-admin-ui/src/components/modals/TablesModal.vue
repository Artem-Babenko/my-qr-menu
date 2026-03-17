<script setup lang="ts">
  import QRCode from 'qrcode';
  import { useToastsStore } from '@/store/toasts';
  import type { Establishment } from '@/types/network';
  import type { TableView } from '@/types/tables';
  import { computed, ref } from 'vue';
  import {
    AppButton,
    AppCard,
    AppFlex,
    AppIcon,
    AppModal,
    AppText,
  } from '../shared';

  const showed = defineModel<boolean>('showed', { required: true });

  withDefaults(
    defineProps<{
      establishment?: Establishment | null;
      tables?: TableView[];
      loading?: boolean;
      readonly?: boolean;
    }>(),
    {
      establishment: null,
      tables: () => [],
      loading: false,
      readonly: false,
    },
  );

  const emit = defineEmits<{
    add: [];
    edit: [table: TableView];
    delete: [table: TableView];
  }>();

  const toasts = useToastsStore();

  const qrModalShowed = ref(false);
  const qrTable = ref<TableView | null>(null);
  const qrCodeUrl = ref('');
  const qrLoading = ref(false);
  const qrError = ref('');

  const qrTitle = computed(() =>
    qrTable.value ? `QR-код для столу №${qrTable.value.number}` : 'QR-код',
  );

  const clientBaseUrl = (
    import.meta.env.VITE_CLIENT_BASE_URL || 'http://localhost:5174'
  ).replace(/\/+$/, '');

  const getTableLink = (tableId: number) => `${clientBaseUrl}/table/${tableId}`;

  const copyTableLink = async (table: TableView) => {
    const link = getTableLink(table.id);
    try {
      await navigator.clipboard.writeText(link);
      toasts.success(`Посилання для столу №${table.number} скопійовано`);
    } catch {
      toasts.error('Не вдалося скопіювати посилання');
    }
  };

  const openQrModal = async (table: TableView) => {
    qrTable.value = table;
    qrCodeUrl.value = '';
    qrError.value = '';
    qrLoading.value = true;
    qrModalShowed.value = true;

    try {
      qrCodeUrl.value = await QRCode.toDataURL(getTableLink(table.id), {
        width: 300,
        margin: 2,
      });
    } catch {
      qrError.value = 'Не вдалося згенерувати QR-код';
    } finally {
      qrLoading.value = false;
    }
  };
</script>

<template>
  <app-modal v-model:showed="showed" title="Редагування столів" :width="650">
    <app-flex justify="space-between" align="center" class="head" :gap="10">
      <app-text v-if="establishment" color="secondary" line="m">
        {{ establishment.name }}
      </app-text>
      <app-button :disabled="readonly" @click="!readonly && emit('add')">
        <app-icon name="Plus" :size="16" :stroke-width="2.5"></app-icon>
        Додати стіл
      </app-button>
    </app-flex>

    <div class="list">
      <app-text v-if="loading" color="secondary">Завантаження...</app-text>

      <app-text v-else-if="tables.length === 0" color="secondary">
        Столів поки немає
      </app-text>

      <app-card
        v-else
        v-for="table in tables"
        :key="table.id"
        class="table-card"
      >
        <app-flex class="table-row" justify="space-between" align="center">
          <app-text class="table-title" weight="500">
            Стіл №{{ table.number }}
          </app-text>

          <app-flex class="icons" gap="10" align="center">
            <app-icon
              name="Pencil"
              :class="{ disabled: readonly }"
              @click="!readonly && emit('edit', table)"
            ></app-icon>
            <app-icon name="Copy" @click="copyTableLink(table)"></app-icon>
            <app-icon name="QrCode" @click="openQrModal(table)"></app-icon>
            <app-icon
              name="Trash"
              :class="{ disabled: readonly }"
              @click="!readonly && emit('delete', table)"
            ></app-icon>
          </app-flex>
        </app-flex>
      </app-card>
    </div>
  </app-modal>

  <app-modal v-model:showed="qrModalShowed" :title="qrTitle" :width="350">
    <div class="qr-content">
      <app-text v-if="qrLoading" color="secondary">
        Генеруємо QR-код...
      </app-text>
      <app-text v-else-if="qrError" color="secondary">{{ qrError }}</app-text>
      <img
        v-else-if="qrCodeUrl"
        class="qr-image"
        :src="qrCodeUrl"
        alt="QR-код для столу"
      />
    </div>
  </app-modal>
</template>

<style scoped>
  .head {
    margin-bottom: 15px;
  }

  .list {
    display: flex;
    flex-direction: column;
    gap: 10px;
    margin-top: 10px;
  }

  .table-card {
    padding: 12px 14px;
  }
  .table-row {
    min-width: 0;
  }
  .table-title {
    flex: 0 0 auto;
    white-space: nowrap;
  }

  .icons .app-icon {
    color: var(--on-surface-variant);
    transition: all 0.2s ease;
    cursor: pointer;

    &:hover {
      color: var(--on-surface);
    }
  }
  .icons .app-icon.disabled {
    opacity: 0.5;
    cursor: default;
    pointer-events: none;
  }

  .qr-content {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 280px;
  }

  .qr-image {
    width: 300px;
    height: 300px;
    object-fit: contain;
    border-radius: 20px;
  }

  @media (max-width: 700px) {
    .head {
      flex-wrap: wrap;
    }
    .head :deep(.app-button) {
      width: 100%;
      justify-content: center;
    }
    .table-row {
      flex-direction: row;
      align-items: center !important;
      gap: 10px !important;
    }
    .icons {
      width: auto;
      margin-left: auto;
      justify-content: flex-end;
      flex-wrap: nowrap;
      gap: 12px !important;
    }
  }
</style>
