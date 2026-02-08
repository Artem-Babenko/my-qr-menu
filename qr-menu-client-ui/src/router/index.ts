import { createRouter, createWebHistory } from 'vue-router';

export const ROUTES = {
  home: 'home',
  menu: 'menu',
  basket: 'basket',
  orders: 'orders',
} as const;

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/table/:tableId',
      component: () => import('@/layouts/ClientLayout.vue'),
      children: [
        {
          path: '',
          name: ROUTES.home,
          component: () => import('@/pages/HomePage.vue'),
        },
        {
          path: 'menu',
          name: ROUTES.menu,
          component: () => import('@/pages/MenuPage.vue'),
        },
        {
          path: 'basket',
          name: ROUTES.basket,
          component: () => import('@/pages/BasketPage.vue'),
        },
        {
          path: 'orders',
          name: ROUTES.orders,
          component: () => import('@/pages/OrdersPage.vue'),
        },
      ],
    },
  ],
});

export default router;
