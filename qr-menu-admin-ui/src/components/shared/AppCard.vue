<script lang="ts" setup></script>

<template>
  <div class="app-card">
    <slot></slot>
  </div>
</template>

<style scoped>
  .app-card {
    padding: 20px;
    border-radius: 10px;
    border: 1px solid var(--border);
  }
</style>
