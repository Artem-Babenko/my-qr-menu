<script setup lang="ts">
  import AppText from './AppText.vue';

  defineProps<{ message?: string }>();
</script>

<template>
  <div class="app-error-text">
    <app-text
      v-if="message"
      size="xxs"
      color="error"
      class="app-error-text-inner"
      :title="message"
    >
      {{ message }}
    </app-text>
  </div>
</template>

<style scoped>
  .app-error-text {
    margin-top: 5px;
    min-height: 10px;
    width: 100%;
    text-align: left;
    padding: 0 2px;
  }

  .app-error-text-inner {
    display: block;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
</style>
