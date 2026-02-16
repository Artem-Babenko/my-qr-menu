<script lang="ts" setup>
  import { useUserLoader } from '@/composables/useUserLoader';
  import { onMounted } from 'vue';

  const loader = useUserLoader();

  onMounted(async () => {
    await loader.loadUserData();
  });
</script>

<template>
  <router-view v-if="loader.loaded.value"></router-view>
</template>
