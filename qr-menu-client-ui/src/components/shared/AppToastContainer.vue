<script setup lang="ts">
  import AppToast from './AppToast.vue';
  import type { ToastItem } from '@/types/toasts';

  defineProps<{ toasts: ToastItem[] }>();

  const emit = defineEmits<{ close: [id: number] }>();
</script>

<template>
  <teleport to="body">
    <div class="toast-container">
      <transition-group name="toast-fade" tag="div" class="toast-list">
        <app-toast
          v-for="toast in toasts"
          :key="toast.id"
          :type="toast.type"
          :message="toast.message"
          @close="emit('close', toast.id)"
        ></app-toast>
      </transition-group>
    </div>
  </teleport>
</template>

<style scoped>
  .toast-container {
    position: fixed;
    top: 20px;
    left: 50%;
    transform: translateX(-50%);
    z-index: 1000;
    pointer-events: none;
  }

  .toast-list {
    display: flex;
    flex-direction: column;
    gap: 10px;
  }

  .toast-container :deep(.app-card) {
    pointer-events: auto;
  }

  .toast-fade-enter-active,
  .toast-fade-leave-active {
    transition: all 0.2s ease;
  }

  .toast-fade-enter-from,
  .toast-fade-leave-to {
    opacity: 0;
    transform: translateY(-10px);
  }
</style>
