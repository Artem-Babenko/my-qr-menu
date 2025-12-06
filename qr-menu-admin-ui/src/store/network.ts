import type { Network } from '@/types/network';
import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useNetworkStore = defineStore('network', () => {
  const network = ref<Network | null>(null);

  return { network };
});
