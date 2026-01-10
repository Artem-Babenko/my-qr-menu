<script setup lang="ts">
import { computed } from 'vue';

type TextSize = 'xxs' | 'xs' | 's' | 'm' | 'l' | 'xl' | 'xxl';
type TextWeight = '400' | '500' | '600';
type TextColor = 'primary' | 'secondary' | 'error';

interface AppTextProps {
  size?: TextSize;
  weight?: TextWeight;
  color?: TextColor;
  el?: string;
}

const props = withDefaults(defineProps<AppTextProps>(), {
  size: 's',
  weight: '400',
  el: 'p',
});

const classes = computed(() => [
  `size-${props.size}`,
  `weight-${props.weight}`,
  `color-${props.color}`,
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

.size-xxs {
  font: var(--font-xxs);
}
.size-xs {
  font: var(--font-xs);
}
.size-s {
  font: var(--font-s);
}
.size-m {
  font: var(--font-m);
}
.size-l {
  font: var(--font-l);
}
.size-xl {
  font: var(--font-xl);
}
.size-xxl {
  font: var(--font-xxl);
}

.weight-400 {
  font-weight: 400;
}
.weight-500 {
  font-weight: 500;
}
.weight-600 {
  font-weight: 600;
}

.color-primary {
  color: var(--primary-text);
}
.color-secondary {
  color: var(--secondary-text);
}
.color-error {
  color: var(--error-text);
}
</style>
