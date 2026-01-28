<script lang="ts" setup>
  import { computed } from 'vue';
  import { DotsButton } from '../buttons';
  import { AppButton, AppDropdown, AppIcon } from '../shared';
  import type { ActionButton } from './types';

  const props = withDefaults(defineProps<{ buttons?: ActionButton[] }>(), {
    buttons: () => [],
  });

  const emit = defineEmits<{ edit: []; delete: [] }>();

  const defaultButtons: ActionButton[] = [
    { icon: 'Pencil', title: 'Редагувати', click: () => emit('edit') },
    { icon: 'Trash', title: 'Видалити', click: () => emit('delete') },
  ];

  const buttons = computed(() => {
    return props.buttons.length > 0 ? props.buttons : defaultButtons;
  });

  const isButtonDisabled = (button: ActionButton): boolean => {
    if (button.disabled === undefined) return false;
    if (typeof button.disabled === 'function') {
      return button.disabled();
    }
    return button.disabled;
  };
</script>

<template>
  <app-dropdown anchor="bottom right" self="top right">
    <template #target>
      <dots-button></dots-button>
    </template>
    <template #default>
      <div class="menu-content">
        <app-button
          type="text"
          v-for="btn in buttons"
          :key="btn.title"
          :disabled="isButtonDisabled(btn)"
          @click="!isButtonDisabled(btn) && btn.click()"
        >
          <app-icon :name="btn.icon" :size="14"></app-icon>
          {{ btn.title }}
        </app-button>
      </div>
    </template>
  </app-dropdown>
</template>

<style scoped>
  .menu-content {
    display: flex;
    flex-direction: column;
    gap: 3px;
  }
  .menu-content .app-button {
    text-align: left;
    padding: 5px 10px;
    gap: 10px;
  }
</style>
