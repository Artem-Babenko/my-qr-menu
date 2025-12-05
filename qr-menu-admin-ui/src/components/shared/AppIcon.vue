<script setup lang="ts">
import * as icons from 'lucide-vue-next';
import { computed } from 'vue';

export type IconName = keyof typeof icons;

interface AppIconProps {
  name: IconName;
  size?: number;
  color?: string;
  strokeWidth?: number;
}

const props = defineProps<AppIconProps>();

const icon = computed(() => icons[props.name] as any);
</script>

<template>
  <component
    :is="icon"
    :size="size"
    :color="color"
    :stroke-width="strokeWidth"
    class="app-icon"
  ></component>
</template>
