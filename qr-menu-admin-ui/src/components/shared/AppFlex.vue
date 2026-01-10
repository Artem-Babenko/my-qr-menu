<script lang="ts" setup>
  import { computed } from 'vue';

  type AlignOptions =
    | 'flex-start'
    | 'center'
    | 'flex-end'
    | 'stretch'
    | 'baseline';
  type JustifyOptions =
    | 'flex-start'
    | 'center'
    | 'flex-end'
    | 'space-between'
    | 'space-around'
    | 'space-evenly';
  type DirectionOptions = 'row' | 'row-reverse' | 'column' | 'column-reverse';

  const props = defineProps<{
    align?: AlignOptions;
    justify?: JustifyOptions;
    direction?: DirectionOptions;
    gap?: number;
  }>();

  const style = computed(() => ({
    display: 'flex',
    alignItems: props.align ?? 'center',
    justifyContent: props.justify ?? 'flex-start',
    flexDirection: props.direction ?? 'row',
    gap: (props.gap ?? 0).toString() + 'px',
  }));
</script>

<template>
  <div class="app-flex" :style="style">
    <slot></slot>
  </div>
</template>
