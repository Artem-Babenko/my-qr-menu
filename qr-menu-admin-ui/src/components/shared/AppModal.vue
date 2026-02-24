<script lang="ts" setup>
  import { computed, onBeforeUnmount, watch } from 'vue';
  import { AppButton, AppIcon, AppText } from '.';

  const props = defineProps<{
    title: string;
    width?: number;
  }>();

  const showed = defineModel<boolean>('showed', { required: true });

  const modalStyle = computed(() => {
    if (!props.width) return {};
    return {
      width: `${props.width}px`,
    };
  });

  function close() {
    showed.value = false;
  }

  const unlockBody = () => {
    document.body.style.overflow = '';
  };

  watch(showed, (value) => {
    if (value) {
      document.body.style.overflow = 'hidden';
      return;
    }
    unlockBody();
  });

  onBeforeUnmount(() => {
    unlockBody();
  });
</script>

<template>
  <teleport to="body">
    <transition name="fade" appear>
      <div v-if="showed" class="background" @mousedown.self="close">
        <div class="modal" :style="modalStyle">
          <div class="header">
            <app-text size="l" weight="600">{{ title }}</app-text>
            <app-button class="close-btn" type="text" @click="close">
              <app-icon name="X" :size="16"></app-icon>
            </app-button>
          </div>
          <slot></slot>
        </div>
      </div>
    </transition>
  </teleport>
</template>

<style scoped>
  .background {
    background-color: rgba(0, 0, 0, 0.3);
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1000;
  }
  .modal {
    background-color: var(--surface-container-low);
    color: var(--on-surface);
    border-radius: 15px;
    border: 1px solid var(--outline-variant);
    padding: 20px 25px;
    width: min(100%, var(--modal-max-width, 640px));
    max-width: 95vw;
    max-height: calc(100dvh - 32px);
    overflow-y: auto;
    overscroll-behavior: contain;
  }
  .header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding-bottom: 15px;
  }
  .close-btn {
    height: 30px;
    width: 30px;
    padding: 0;
    justify-content: center;
  }

  @media (max-width: 576px) {
    .background {
      align-items: center;
      justify-content: center;
      padding: 0 5px;
    }
    .modal {
      width: 100%;
      max-width: calc(100vw - 10px);
      max-height: calc(100dvh - 20px);
      border-radius: 14px;
      padding: 16px;
    }
    .header {
      padding-bottom: 10px;
    }
  }
</style>
