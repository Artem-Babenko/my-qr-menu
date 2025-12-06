import { vMaska } from 'maska/vue';
import { createPinia } from 'pinia';
import { createApp } from 'vue';
import { initApi } from './api/api';
import App from './App.vue';
import router, { ROUTES } from './router';
import { useAuthStore } from './store/auth';
import { useUserStore } from './store/user';

const app = createApp(App);

app.use(createPinia());
app.use(router);
app.directive('maska', vMaska);

const authStore = useAuthStore();
authStore.init();

const logout = () => {
  authStore.setToken(null);
  useUserStore().user = null;
  router.replace({ name: ROUTES.login });
};

initApi(logout);

app.mount('#app');
