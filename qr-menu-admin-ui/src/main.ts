import { createPinia } from 'pinia';
import { createApp } from 'vue';

import { vMaska } from 'maska/vue';
import App from './App.vue';
import router from './router';
import { useAuthStore } from './store/auth';

const app = createApp(App);

app.use(createPinia());
app.use(router);
app.directive('maska', vMaska);

useAuthStore().init();

app.mount('#app');
