<script lang="ts" setup>
  import { AppCard, AppFlex, AppIcon, AppText } from '../shared';
</script>

<template>
  <app-card class="no-invitations-card">
    <app-flex class="clock-icon" justify="center">
      <app-icon name="Clock" :size="20"></app-icon>
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
    width: min(100%, 400px);
    min-height: 200px;
    box-sizing: border-box;
    margin: 0 auto;
  }
  .clock-icon {
    width: 40px;
    height: 40px;
    background-color: var(--secondary-container);
    color: var(--on-secondary-container);
    border-radius: 10px;
  }

  @media (max-width: 640px) {
    .no-invitations-card {
      width: 100%;
      min-height: 170px;
      gap: 12px;
      text-align: left;
      align-items: flex-start;
    }
  }
</style>
