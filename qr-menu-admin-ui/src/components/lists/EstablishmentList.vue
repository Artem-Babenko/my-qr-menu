<script setup lang="ts">
  import type { Establishment } from '@/types/network';
  import { EstablishmentCard } from '../cards';
  import BaseCardList from './BaseCardList.vue';

  defineProps<{ establishments: Establishment[] }>();

  const emit = defineEmits<{
    edit: [establishment: Establishment];
    tables: [establishment: Establishment];
    delete: [establishment: Establishment];
  }>();
</script>

<template>
  <base-card-list>
    <establishment-card
      v-for="est in establishments"
      :key="est.id"
      :establishment="est"
      @edit="emit('edit', est)"
      @tables="emit('tables', est)"
      @delete="emit('delete', est)"
    ></establishment-card>
  </base-card-list>
</template>
