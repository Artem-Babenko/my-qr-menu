<script lang="ts" setup>
  import { nextTick, onBeforeUnmount, ref, watch } from 'vue';

  type Position = 'top left' | 'top right' | 'bottom left' | 'bottom right';

  const props = withDefaults(
    defineProps<{
      anchor?: Position;
      self?: Position;
      offset?: [number, number];
    }>(),
    {
      anchor: 'bottom left',
      self: 'top left',
      offset: () => [0, 0],
    },
  );

  const model = defineModel<boolean>('open', { default: false });

  const targetRef = ref<HTMLElement | null>(null);
  const menuRef = ref<HTMLElement | null>(null);
  const style = ref<Record<string, string>>({});

  function close() {
    model.value = false;
  }

  function toggle() {
    model.value = !model.value;
  }

  function updatePosition() {
    if (!targetRef.value || !menuRef.value) return;

    const target = targetRef.value.getBoundingClientRect();
    const menu = menuRef.value.getBoundingClientRect();

    let top = 0;
    let left = 0;

    if (props.anchor!.includes('bottom')) top = target.bottom;
    if (props.anchor!.includes('top')) top = target.top;
    if (props.anchor!.includes('left')) left = target.left;
    if (props.anchor!.includes('right')) left = target.right;

    if (props.self!.includes('bottom')) top -= menu.height;
    if (props.self!.includes('right')) left -= menu.width;

    top += props.offset![1];
    left += props.offset![0];

    style.value = {
      top: `${top}px`,
      left: `${left}px`,
    };
  }

  function onClickOutside(e: MouseEvent) {
    if (
      !menuRef.value?.contains(e.target as Node) &&
      !targetRef.value?.contains(e.target as Node)
    ) {
      close();
    }
  }

  watch(model, async (open) => {
    if (open) {
      await nextTick();
      updatePosition();
      document.addEventListener('click', onClickOutside);
    } else {
      document.removeEventListener('click', onClickOutside);
    }
  });

  onBeforeUnmount(() => {
    document.removeEventListener('click', onClickOutside);
  });
</script>

<template>
  <div ref="targetRef" @click.stop="toggle">
    <slot name="target"></slot>
  </div>

  <teleport to="body">
    <div v-if="model" ref="menuRef" class="app-dropdown" :style="style">
      <slot></slot>
    </div>
  </teleport>
</template>

<style scoped>
  .app-dropdown {
    position: absolute;
    z-index: 1000;
    background: #ffffff;
    border-radius: 5px;
    border: 1px solid var(--border);
    padding: 5px;
    box-shadow: 1px 3px 9px rgba(0, 0, 0, 0.05);
  }
</style>
