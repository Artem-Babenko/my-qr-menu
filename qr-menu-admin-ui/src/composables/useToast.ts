import { storeToRefs } from 'pinia';
import { useToastsStore } from '@/store/toasts';

export function useToast() {
  const store = useToastsStore();
  const { toasts } = storeToRefs(store);

  return {
    toasts,
    success: store.success,
    error: store.error,
    remove: store.remove,
  };
}
