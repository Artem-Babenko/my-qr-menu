<script setup lang="ts" generic="T">
import { computed, onBeforeUnmount, onMounted, ref, useTemplateRef } from 'vue';
import AppButton from './AppButton.vue';
import AppIcon from './AppIcon.vue';
import AppText from './AppText.vue';

interface AppSelectProps {
  options: T[];
  optionValue?: keyof T;
  optionLabel?: keyof T;
  placeholder?: string;
}

const props = defineProps<AppSelectProps>();
const model = defineModel<T | null>({ required: true });
const root = useTemplateRef('root');
const showed = ref(false);

const handleClickOutside = (event: MouseEvent) => {
  if (!root.value) return;
  if (!root.value.contains(event.target as Node)) {
    showed.value = false;
  }
};

onMounted(() => {
  document.addEventListener('click', handleClickOutside);
});

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside);
});

const getOptionLabel = (opt: T) => {
  if (props.optionLabel && typeof opt === 'object' && opt !== null) {
    return String((opt as any)[props.optionLabel]);
  }
  return String(opt);
};

// const getOptionValue = (opt: T) => {
//   if (props.optionValue && typeof opt === 'object' && opt !== null) {
//     return (opt as any)[props.optionValue];
//   }
//   return opt;
// };

const selectedLabel = computed(() => {
  if (!model.value) return props.placeholder ?? 'Оберіть...';
  if (props.optionLabel && typeof model.value === 'object') {
    return String((model.value as any)[props.optionLabel]);
  }
  return String(model.value);
});

const selectOption = (opt: T) => {
  model.value = opt;
  showed.value = false;
};
</script>

<template>
  <div class="app-select" ref="root" @click="showed = !showed">
    <slot name="before"></slot>
    <app-text class="value" :class="{ placeholder: !model }">
      {{ selectedLabel }}
    </app-text>
    <app-icon
      name="ChevronDown"
      :size="14"
      :class="{ rotate: showed }"
    ></app-icon>

    <div v-if="showed" class="options">
      <div v-for="(opt, i) in options" :key="i" @click.stop="selectOption(opt)">
        <slot name="option">
          <app-button type="text" class="option-btn">
            <app-text>{{ getOptionLabel(opt) }}</app-text>
          </app-button>
        </slot>
      </div>
    </div>
  </div>
</template>

<style scoped>
.app-select {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 0 12px;
  border: 1px solid var(--border);
  border-radius: 15px;
  cursor: pointer;
  position: relative;
  min-width: 120px;
  background: var(--background);
  height: 36px;
}
.value.placeholder {
  color: var(--text-muted, #aaa);
}

.app-icon {
  margin-left: auto;
  transition: transform 0.2s;
}
.app-icon.rotate {
  transform: scale(-1);
}

.options {
  position: absolute;
  top: calc(100% + 5px);
  left: 0;
  border-radius: 10px;
  background-color: var(--background);
  border: 1px solid var(--border);
  padding: 3px;
  box-shadow: 1px 3px 5px rgba(0, 0, 0, 0.1);
  width: 100%;
  display: flex;
  flex-direction: column;
  z-index: 10;
}
.option-btn {
  width: 100%;
  text-align: left;
  padding: 10px;
  border-radius: 7.5px;
}
.option-btn:hover {
  background-color: rgba(0, 0, 0, 0.05);
}
</style>
