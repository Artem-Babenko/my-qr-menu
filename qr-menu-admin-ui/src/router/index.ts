import AuthLayout from '@/layouts/AuthLayout.vue';
import MainLayout from '@/layouts/MainLayout.vue';
import UserLoaderGate from '@/layouts/UserLoaderGate.vue';
import LoginPage from '@/pages/auth/LoginPage.vue';
import RegistrationPage from '@/pages/auth/RegistrationPage.vue';
import MainPage from '@/pages/MainPage.vue';
import OnboardingPage from '@/pages/OnboardingPage.vue';
import { useAuthStore } from '@/store/auth';
import {
  createRouter,
  createWebHistory,
  type RouteRecordRaw,
} from 'vue-router';

export const ROUTES = {
  login: 'login',
  registration: 'registration',
  onboarding: 'onboarding',
  dashboard: 'dashboard',
};

const routes: RouteRecordRaw[] = [
  {
    path: '/auth',
    component: AuthLayout,
    meta: { authLayout: true },
    children: [
      {
        name: ROUTES.login,
        path: ROUTES.login,
        component: LoginPage,
      },
      {
        name: ROUTES.registration,
        path: ROUTES.registration,
        component: RegistrationPage,
      },
    ],
  },
  {
    path: '/',
    component: UserLoaderGate,
    meta: { requiresAuth: true },
    redirect: { name: ROUTES.dashboard },
    children: [
      {
        name: ROUTES.onboarding,
        path: ROUTES.onboarding,
        component: OnboardingPage,
      },
      {
        path: '',
        component: MainLayout,
        meta: { requiresAuth: true },
        children: [
          {
            path: ROUTES.dashboard,
            name: ROUTES.dashboard,
            component: MainPage,
          },
        ],
      },
    ],
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: routes,
});

router.beforeEach((to) => {
  const authStore = useAuthStore();

  if (to.meta.requiresAuth && !authStore.token) {
    return { name: ROUTES.login };
  }

  if (to.meta.authLayout && authStore.token) {
    return { name: ROUTES.dashboard };
  }

  return true;
});

export default router;
