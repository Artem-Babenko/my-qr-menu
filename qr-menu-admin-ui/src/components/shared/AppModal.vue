<script lang="ts" setup>
  import { AppButton, AppIcon, AppText } from '.';

  defineProps<{ title: string }>();

  const showed = defineModel<boolean>('showed', { required: true });

  function close() {
    showed.value = false;
  }
</script>

<template>
  <teleport to="body">
    <transition name="fade" appear>
      <div v-if="showed" class="background" @click.self="close">
        <div class="modal">
          <div class="header">
            <app-text size="m" weight="600">{{ title }}</app-text>
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
    background-color: #fff;
    border-radius: 10px;
    border: 1px solid var(--border);
    padding: 15px 25px;
    min-width: 400px;
    max-width: 90%;
    max-height: 90%;
    overflow-y: auto;
  }
  .header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding-bottom: 15px;
  }
  .close-btn {
    padding: 5px 5px 3px;
  }
</style>
