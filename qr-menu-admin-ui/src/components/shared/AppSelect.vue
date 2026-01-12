<script setup lang="ts" generic="T">
  import { computed, ref } from 'vue';
  import { AppFlex, AppDropdown, AppButton, AppIcon, AppText } from '.';

  interface AppSelectProps<T> {
    options: T[] | null;
    optionValue?: keyof T;
    optionLabel?: keyof T;
    placeholder?: string;
  }

  const props = defineProps<AppSelectProps<T>>();
  const model = defineModel<T | null | undefined>({ required: true });

  const open = ref(false);

  const getOptionLabel = (opt: T) => {
    if (props.optionLabel && typeof opt === 'object' && opt !== null) {
      return String((opt as any)[props.optionLabel]);
    }
    return String(opt);
  };

  const selectedLabel = computed(() => {
    if (!model.value) {
      return props.placeholder ?? 'Оберіть...';
    }
    if (props.optionLabel && typeof model.value === 'object') {
      return String((model.value as any)[props.optionLabel]);
    }
    return String(model.value);
  });

  function selectOption(opt: T) {
    model.value = opt;
    open.value = false;
  }
</script>

<template>
  <AppDropdown v-model:open="open" :offset="[0, 6]">
    <template #target>
      <app-flex class="app-select" gap="10">
        <slot name="before"></slot>

        <app-text class="value" :class="{ placeholder: !model }">
          {{ selectedLabel }}
        </app-text>

        <app-icon
          name="ChevronDown"
          :size="14"
          class="chevron"
          :class="{ rotate: open }"
        ></app-icon>
      </app-flex>
    </template>

    <template #default>
      <div class="options">
        <div
          v-for="(opt, i) in options"
          :key="i"
          @click.stop="selectOption(opt)"
        >
          <slot name="option" :option="opt">
            <app-button type="text" class="option-btn">
              <app-text>{{ getOptionLabel(opt) }}</app-text>
            </app-button>
          </slot>
        </div>
      </div>
    </template>
  </AppDropdown>
</template>

<style scoped>
  .app-select {
    border: 1px solid var(--border);
    border-radius: 5px;
    cursor: pointer;
    width: 100%;
    background: var(--background);
    height: 36px;
  }
  .value {
    padding-left: 12px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    width: 100%;
    min-width: 0;
  }
  .value.placeholder {
    color: var(--text-on-secondary);
  }
  .chevron {
    padding-right: 10px;
    margin-left: auto;
    transition: transform 0.2s;
    flex-shrink: 0;
  }
  .chevron.rotate {
    transform: scaleY(-1);
  }
  .options {
    display: flex;
    flex-direction: column;
    min-width: 100%;
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
