<script setup lang="ts">
  import { networkApi } from '@/api/networkApi';
  import { AppButton, AppFlex, AppText } from '@/components/shared';
  import { EstablishmentList } from '@/components/lists';
  import {
    CreateEstablishmentModal,
    EstablishmentModal,
    NetworkModal,
  } from '@/components/modals';
  import { getErrorMessage } from '@/consts/errorMessages';
  import { useNetworkStore } from '@/store/network';
  import { useToastsStore } from '@/store/toasts';
  import type { Establishment } from '@/types/network';
  import { computed, ref } from 'vue';

  const networkStore = useNetworkStore();
  const toasts = useToastsStore();

  const createModalShowed = ref(false);
  const networkModalShowed = ref(false);
  const establishmentModalShowed = ref(false);
  const editingEstablishment = ref<Establishment | null>(null);

  const hasNetwork = computed(() => !!networkStore.network);
  const establishments = computed(
    () => networkStore.network?.establishments ?? [],
  );

  const isSingle = computed(() => establishments.value.length === 1);
  const isMultiple = computed(() => establishments.value.length >= 2);

  const openCreate = () => (createModalShowed.value = true);
  const openNetworkSettings = () => (networkModalShowed.value = true);

  const reloadNetwork = async () => {
    const id = networkStore.network?.id;
    if (!id) return;
    const resp = await networkApi.getNetwork(id);
    if (resp.success) {
      networkStore.network = resp.data;
    }
  };

  const onEdit = (est: Establishment) => {
    editingEstablishment.value = est;
    establishmentModalShowed.value = true;
  };

  const onDelete = async (est: Establishment) => {
    const confirmed = window.confirm(`Видалити заклад "${est.name}"?`);
    if (!confirmed) return;

    const resp = await networkApi.deleteEstablishment(est.id);
    if (!resp.success) {
      toasts.error(getErrorMessage(resp.errorCode));
      return;
    }
    await reloadNetwork();
  };

  const onSaved = async () => {
    await reloadNetwork();
  };
</script>

<template>
  <div v-if="hasNetwork" class="page">
    <app-flex justify="space-between" align="center" class="page-head">
      <div>
        <app-text weight="600" size="l">Заклади харчування</app-text>
        <app-text color="secondary" v-if="isMultiple">
          Управління закладами та мережею
        </app-text>
      </div>
      <app-button v-if="!isMultiple" @click="openCreate">
        Додати заклад
      </app-button>
    </app-flex>

    <!-- Single establishment: show only card list area (card component added later) -->
    <div v-if="isSingle" class="content">
      <establishment-list
        :establishments="establishments"
        @edit="onEdit"
        @delete="onDelete"
      ></establishment-list>
    </div>

    <!-- Multiple establishments: wrapper like screenshot -->
    <div v-else class="network-wrapper">
      <app-flex justify="space-between" align="center" class="network-head">
        <div>
          <app-text weight="600">
            Мережа "{{ networkStore.network?.name }}"
          </app-text>
          <app-text color="secondary">
            {{ establishments.length }} закладів у мережі
          </app-text>
        </div>
        <app-flex gap="10">
          <app-button type="outline" @click="openNetworkSettings">
            Налаштування мережі
          </app-button>
          <app-button @click="openCreate">Додати заклад</app-button>
        </app-flex>
      </app-flex>
      <div class="content">
        <establishment-list
          :establishments="establishments"
          @edit="onEdit"
          @delete="onDelete"
        ></establishment-list>
      </div>
    </div>

    <create-establishment-modal
      v-model:showed="createModalShowed"
      :current-count="establishments.length"
      @saved="onSaved"
    ></create-establishment-modal>

    <network-modal
      v-model:showed="networkModalShowed"
      @saved="onSaved"
    ></network-modal>

    <establishment-modal
      v-model:showed="establishmentModalShowed"
      :establishment="editingEstablishment"
      @saved="onSaved"
    ></establishment-modal>
  </div>
</template>

<style scoped>
  .page {
    display: flex;
    flex-direction: column;
    gap: 20px;
  }

  .page-head {
    gap: 10px;
  }

  .network-wrapper {
    border: 1px solid var(--border);
    border-radius: 10px;
    background-color: var(--background);
    padding: 20px;
    display: flex;
    flex-direction: column;
    gap: 20px;
  }

  .content {
    min-height: 200px;
  }
</style>
