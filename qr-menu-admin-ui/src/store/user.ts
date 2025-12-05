import type { User } from '@/types/user';
import { defineStore } from 'pinia';
import { computed, ref } from 'vue';

export const useUserStore = defineStore('user', () => {
  const user = ref<User | null>(null);

  const requireUser = computed(() => {
    if (!user.value) throw new Error('User is not loaded');
    return user.value;
  });

  return { user, requireUser };
});
