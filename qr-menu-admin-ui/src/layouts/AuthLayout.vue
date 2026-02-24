<script setup lang="ts"></script>

<template>
  <div class="auth-layout">
    <router-view></router-view>
  </div>
</template>

<style scoped>
  .auth-layout {
    min-height: 100dvh;
    background-color: var(--background);
  }
</style>
