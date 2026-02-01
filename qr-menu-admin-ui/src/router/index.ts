import AuthLayout from '@/layouts/AuthLayout.vue';
import MainLayout from '@/layouts/MainLayout.vue';
import UserLoaderGate from '@/layouts/UserLoaderGate.vue';
import LoginPage from '@/pages/auth/LoginPage.vue';
import RegByInvitationPage from '@/pages/auth/RegByInvitationPage.vue';
import RegistrationPage from '@/pages/auth/RegistrationPage.vue';
import DashboardPage from '@/pages/main/DashboardPage.vue';
import EstablishmentsPage from '@/pages/main/EstablishmentsPage.vue';
import MenuPage from '@/pages/main/MenuPage.vue';
import UsersPage from '@/pages/main/UsersPage.vue';
import MenuTabCategories from '@/pages/main/menu/MenuTabCategories.vue';
import MenuTabProducts from '@/pages/main/menu/MenuTabProducts.vue';
import UsersTabInvitations from '@/pages/main/users/UsersTabInvitations.vue';
import UsersTabRoles from '@/pages/main/users/UsersTabRoles.vue';
import UsersTabUsers from '@/pages/main/users/UsersTabUsers.vue';
import CreateEstablishmentPage from '@/pages/onboarding/CreateEstablishmentPage.vue';
import OnboardingPage from '@/pages/onboarding/OnboardingPage.vue';
import { useAuthStore } from '@/store/auth';
import {
  createRouter,
  createWebHistory,
  type RouteRecordRaw,
} from 'vue-router';

export const ROUTES = {
  login: 'login',
  registration: 'registration',
  regByInvitation: 'reg-by-invitation',
  onboarding: 'onboarding',
  createEstablishment: 'create-establishment',
  dashboard: 'dashboard',
  menu: 'menu',
  menuProducts: 'menu-products',
  menuCategories: 'menu-categories',
  establishments: 'establishments',
  users: 'users',
  usersRoles: 'users-roles',
  usersInvitations: 'users-invitations',
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
      {
        name: ROUTES.regByInvitation,
        path: `${ROUTES.regByInvitation}/:invitationId`,
        component: RegByInvitationPage,
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
        name: ROUTES.createEstablishment,
        path: ROUTES.createEstablishment,
        component: CreateEstablishmentPage,
      },
      {
        path: '',
        component: MainLayout,
        children: [
          {
            path: ROUTES.dashboard,
            name: ROUTES.dashboard,
            component: DashboardPage,
          },
          {
            path: ROUTES.menu,
            component: MenuPage,
            children: [
              {
                path: '',
                redirect: { name: ROUTES.menuProducts },
              },
              {
                path: 'products',
                name: ROUTES.menuProducts,
                component: MenuTabProducts,
              },
              {
                path: 'categories',
                name: ROUTES.menuCategories,
                component: MenuTabCategories,
              },
            ],
          },
          {
            path: ROUTES.establishments,
            name: ROUTES.establishments,
            component: EstablishmentsPage,
          },
          {
            path: ROUTES.users,
            component: UsersPage,
            children: [
              {
                path: '',
                name: ROUTES.users,
                component: UsersTabUsers,
              },
              {
                path: 'roles',
                name: ROUTES.usersRoles,
                component: UsersTabRoles,
              },
              {
                path: 'invitations',
                name: ROUTES.usersInvitations,
                component: UsersTabInvitations,
              },
            ],
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
