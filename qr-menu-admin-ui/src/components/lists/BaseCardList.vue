<script lang="ts" setup></script>

<template>
  <div class="base-card-list">
    <slot></slot>
  </div>
</template>

<style scoped>
  .base-card-list {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
    gap: 20px;
  }
</style>
