<script setup lang="ts">
  import { computed } from 'vue';

  type TextSize = 'xxs' | 'xs' | 's' | 'm' | 'l' | 'xl' | 'xxl';
  type TextWeight = '400' | '500' | '600';
  type TextColor = 'primary' | 'secondary' | 'accent' | 'error' | 'success';
  type LineHeight = 's' | 'm';

  interface AppTextProps {
    size?: TextSize;
    weight?: TextWeight;
    color?: TextColor;
    el?: string;
    line?: LineHeight;
  }

  const props = withDefaults(defineProps<AppTextProps>(), {
    size: 's',
    weight: '400',
    el: 'p',
    line: 's',
  });

  const classes = computed(() => [
    `size-${props.size}`,
    `weight-${props.weight}`,
    `color-${props.color}`,
    `line-${props.line}`,
  ]);
</script>

<template>
  <component :is="el" class="app-text" :class="classes">
    <slot></slot>
  </component>
</template>

<style scoped>
  .app-text {
    margin: 0;
  }

  .size-xxs { font: var(--font-xxs); }
  .size-xs { font: var(--font-xs); }
  .size-s { font: var(--font-s); }
  .size-m { font: var(--font-m); }
  .size-l { font: var(--font-l); }
  .size-xl { font: var(--font-xl); }
  .size-xxl { font: var(--font-xxl); }

  .weight-400 { font-weight: 400; }
  .weight-500 { font-weight: 500; }
  .weight-600 { font-weight: 600; }

  .line-s { line-height: 1; }
  .line-m { line-height: 1.5; }

  .color-primary { color: var(--on-surface); }
  .color-secondary { color: var(--on-surface-variant); }
  .color-accent { color: var(--on-tertiary-container); }
  .color-error { color: var(--error); }
  .color-success { color: var(--success); }
</style>
