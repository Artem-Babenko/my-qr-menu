<script setup lang="ts">
  import { computed } from 'vue';
  import AppCard from './AppCard.vue';
  import AppFlex from './AppFlex.vue';
  import AppIcon from './AppIcon.vue';
  import AppText from './AppText.vue';
  import type { ToastType } from '@/types/toasts';

  const props = defineProps<{ type: ToastType; message: string }>();

  const emit = defineEmits<{ close: [] }>();

  const iconName = computed(() => {
    return props.type === 'success' ? 'CheckCircle2' : 'XCircle';
  });

  const iconColor = computed(() => {
    return props.type === 'success'
      ? 'var(--success)'
      : 'var(--error)';
  });
</script>

<template>
  <app-card class="app-toast">
    <app-flex align="center" gap="8">
      <app-icon :name="iconName" :color="iconColor" :size="18"></app-icon>
      <app-text size="s">{{ message }}</app-text>
    </app-flex>
    <button class="close-btn" type="button" @click="emit('close')">
      <app-icon name="X"></app-icon>
    </button>
  </app-card>
</template>

<style scoped>
  .app-toast {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 10px 14px;
    box-shadow: 0 6px 18px rgba(0, 0, 0, 0.15);
    min-width: 260px;
    max-width: 90vw;
  }

  .close-btn {
    border: none;
    background: transparent;
    cursor: pointer;
    padding: 0;
    margin-left: 12px;
    display: flex;
    align-items: center;
    color: var(--on-surface-variant);
  }
</style>
