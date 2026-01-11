<script lang="ts" setup>
  import { AppButton, AppInput, AppModal } from '@/components/shared';
  import type { User } from '@/types/user';

  const user = defineModel<User>('user', { required: true });

  const showed = defineModel<boolean>('showed', { required: true });

  const close = () => {
    showed.value = false;
  };
</script>

<template>
  <app-modal v-model:showed="showed" title="Запрошення користувача">
    <div class="form">
      <app-input v-model="user.name"></app-input>
      <app-input v-model="user.surname"></app-input>
      <app-input v-model="user.email"></app-input>
      <div class="buttons">
        <app-button type="text" @click="close">Скасувати</app-button>
        <app-button>Зберегти</app-button>
      </div>
    </div>
  </app-modal>
</template>

<style scoped>
  .buttons {
    display: flex;
    justify-content: end;
    gap: 10px;
  }
</style>
