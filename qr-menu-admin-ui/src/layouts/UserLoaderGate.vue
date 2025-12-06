<script lang="ts" setup>
import { userApi } from '@/api/userApi';
import { ROUTES } from '@/router';
import { useAuthStore } from '@/store/auth';
import { useUserStore } from '@/store/user';
import { HttpStatusCode } from 'axios';
import { onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const userStore = useUserStore();
const router = useRouter();
const loaded = ref(false);

const loadUser = async () => {
  const resp = await userApi.current();
  if (resp.status !== HttpStatusCode.Ok) {
    throw new Error(resp.statusText);
  }
  return resp.data;
};

onMounted(async () => {
  if (!authStore.authenticated) {
    await router.replace({ name: ROUTES.login });
    return;
  }

  userStore.user ??= await loadUser();

  if (
    !userStore.user.networkId &&
    router.currentRoute.value.name !== ROUTES.onboarding
  ) {
    await router.replace({ name: ROUTES.onboarding });
  }

  loaded.value = true;
});
</script>

<template>
  <router-view v-if="loaded"></router-view>
</template>
