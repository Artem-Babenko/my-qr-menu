<script lang="ts" setup>
  import { AppCard, AppFlex, AppIcon, AppText } from '../shared';
</script>

<template>
  <app-card class="no-invitations-card">
    <app-flex class="clock-icon" justify="center">
      <app-icon
        name="Clock"
        :size="20"
        color="var(--secondary-text)"
      ></app-icon>
    </app-flex>
    <app-text line="m" color="secondary">
      У вас поки немає запрошень. Очікуйте, поки адміністратор закладу надішле
      вам запрошення.
    </app-text>
  </app-card>
</template>

<style scoped>
  .no-invitations-card {
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: column;
    text-align: center;
    gap: 15px;
    width: 400px;
    height: 200px;
  }
  .clock-icon {
    width: 40px;
    height: 40px;
    background-color: var(--hover-on-secondary);
    border-radius: 10px;
  }
</style>
