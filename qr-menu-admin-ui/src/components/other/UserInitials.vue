<script lang="ts" setup>
  import { AppText } from '@/components/shared';
  import { computed } from 'vue';

  const props = defineProps<{ name: string; surname: string }>();

  const initials = computed(() => {
    const nameSymbol = props.name[0];
    const surnameSymbol = props.surname[0];
    return `${nameSymbol}${surnameSymbol}`.toUpperCase();
  });
</script>

<template>
  <app-text class="user-img" weight="600">{{ initials }}</app-text>
</template>

<style scoped>
  .user-img {
    border-radius: 50%;
    width: 40px;
    height: 40px;
    background-color: var(--hover-on-secondary);
    display: flex;
    align-items: center;
    justify-content: center;
  }
</style>
