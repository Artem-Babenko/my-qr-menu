import type { ToastItem, ToastType } from '@/types/toasts';
import { defineStore } from 'pinia';
import { ref } from 'vue';

const MAX_TOASTS = 5;

export const useToastsStore = defineStore('toasts', () => {
  const toasts = ref<ToastItem[]>([]);
  const timers = new Map<number, number>();

  const generateId = () => Date.now() + Math.random();

  const remove = (id: number) => {
    toasts.value = toasts.value.filter((t) => t.id !== id);
    const timer = timers.get(id);
    if (timer) {
      clearTimeout(timer);
      timers.delete(id);
    }
  };

  const add = (type: ToastType, message: string, timeoutMs = 3000) => {
    if (toasts.value.length >= MAX_TOASTS) {
      remove(toasts.value[0]?.id ?? 0);
    }

    const id = generateId();
    toasts.value.push({ id, type, message });

    if (timeoutMs > 0) {
      const timer = window.setTimeout(() => remove(id), timeoutMs);
      timers.set(id, timer);
    }
  };

  return {
    toasts,
    add,
    remove,
    success: (msg: string) => add('success', msg),
    error: (msg: string) => add('error', msg),
  };
});
