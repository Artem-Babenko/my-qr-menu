<script lang="ts" setup></script>

<template>
  <div class="base-card-list">
    <slot></slot>
  </div>
</template>

<style scoped>
  .base-card-list {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(min(100%, 260px), 1fr));
    gap: 20px;
  }

  @media (max-width: 640px) {
    .base-card-list {
      gap: 12px;
    }
  }
</style>
