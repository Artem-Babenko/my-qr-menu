<script setup lang="ts">
import type { IconName } from '@/components/shared';
import { AppText } from '@/components/shared';
import type { Establishment } from '@/types/models';
import { ref } from 'vue';
import PageButton from '../components/buttons/NavButton.vue';
import EstablishmentSelect from './EstablishmentSelect.vue';
import NavUserInfo from './NavUserInfo.vue';

type PageButton = { icon: IconName; name: string };

const pageButtons: PageButton[] = [
  { icon: 'LayoutDashboard', name: 'Головна' },
  { icon: 'UtensilsCrossed', name: 'Меню' },
  { icon: 'ShoppingBag', name: 'Замовлення' },
  { icon: 'BarChart3', name: 'Аналітика' },
  { icon: 'Store', name: 'Заклади' },
  { icon: 'Users', name: 'Користувачі' },
  { icon: 'Settings', name: 'Налаштування' },
];

const establishments: Establishment[] = [
  { id: 1, name: 'Смакота - Центер', address: 'вул. Цетральна 23' },
  { id: 2, name: 'Смакота - Паркова', address: 'вул. Паркова 56' },
  { id: 3, name: 'Смакота - Магістраль', address: 'вул. Заводська 14' },
];

const selectedPage = ref<PageButton>(pageButtons[0]!);
const selectedEstablishment = ref<Establishment>(establishments[0]!);
</script>

<template>
  <div class="main-layout">
    <aside class="nav">
      <app-text size="l" weight="600" class="site-name">QR Menu</app-text>
      <establishment-select
        v-model="selectedEstablishment"
        :establishments="establishments"
      ></establishment-select>
      <div class="nav-buttons">
        <PageButton
          v-for="btn in pageButtons"
          :key="btn.name"
          :label="btn.name"
          :icon="btn.icon"
          :selected="selectedPage.name === btn.name"
          @click="selectedPage = btn"
        ></PageButton>
      </div>
      <nav-user-info></nav-user-info>
    </aside>
    <main>
      <router-view></router-view>
    </main>
  </div>
</template>

<style scoped>
.main-layout {
  height: 100%;
  width: 100%;
  overflow: hidden;
  display: flex;
}
.site-name {
  font-size: 22px;
  padding: 20px 25px;
  border-bottom: 1px solid var(--border);
}
.nav {
  width: 250px;
  display: flex;
  flex-direction: column;
  border-right: 1px solid var(--border);
}
.nav-user-info {
  margin-top: auto;
}
.nav-buttons {
  display: flex;
  flex-direction: column;
  gap: 5px;
  padding: 20px 10px;
}
</style>
