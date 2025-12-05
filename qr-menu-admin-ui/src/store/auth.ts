import { tokenStorage } from '@/utils/tokenStorage';
import { defineStore } from 'pinia';
import { computed, ref } from 'vue';

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(tokenStorage.get() ?? null);
  const authenticated = computed(() => !!token.value);

  const setToken = (newToken: string | null) => {
    token.value = newToken;

    if (newToken) {
      tokenStorage.set(newToken);
    } else {
      tokenStorage.remove();
    }
  };

  const init = () => {
    token.value = tokenStorage.get();
  };

  return {
    token,
    authenticated,
    setToken,
    init,
  };
});
