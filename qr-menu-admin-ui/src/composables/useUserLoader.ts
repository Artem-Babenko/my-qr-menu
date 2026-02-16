import { networkApi } from '@/api/networkApi';
import { rolesApi } from '@/api/rolesApi';
import { usersApi } from '@/api/usersApi';
import { ROUTES } from '@/router';
import { useAuthStore } from '@/store/auth';
import { useNetworkStore } from '@/store/network';
import { useRolesStore } from '@/store/roles';
import { useUserStore } from '@/store/user';
import { ref } from 'vue';
import { useRouter } from 'vue-router';

export function useUserLoader() {
  const authStore = useAuthStore();
  const userStore = useUserStore();
  const rolesStore = useRolesStore();
  const networkStore = useNetworkStore();
  const router = useRouter();
  const loaded = ref(false);

  async function loadUser() {
    const resp = await usersApi.current();
    return resp.data;
  }

  async function loadNetworkData(networkId: number | null | undefined) {
    if (!networkId) return;

    const [networkResp, rolesResp] = await Promise.all([
      networkApi.getNetwork(networkId),
      rolesApi.all(networkId),
    ]);

    networkStore.network = networkResp.data;
    rolesStore.roles = rolesResp.data;
  }

  async function loadUserData() {
    if (!authStore.authenticated) {
      await router.replace({ name: ROUTES.login });
      return;
    }

    userStore.user = await loadUser();
    const networkId = userStore.user!.networkId;

    if (!networkId && router.currentRoute.value.name !== ROUTES.onboarding) {
      await router.replace({ name: ROUTES.onboarding });
    }
    await loadNetworkData(networkId);

    loaded.value = true;
  }

  return { loadUserData, loaded };
}
