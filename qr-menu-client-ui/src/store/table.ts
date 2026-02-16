import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { TableInfo } from '@/types/table';

export const useTableStore = defineStore('table', () => {
  const table = ref<TableInfo | null>(null);
  const loading = ref(false);

  function setTable(info: TableInfo) {
    table.value = info;
  }

  function clear() {
    table.value = null;
  }

  return { table, loading, setTable, clear };
});
