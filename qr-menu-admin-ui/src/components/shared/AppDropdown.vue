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
  const VIEWPORT_PADDING = 8;

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

    const maxTop = window.innerHeight - menu.height - VIEWPORT_PADDING;
    const maxLeft = window.innerWidth - menu.width - VIEWPORT_PADDING;

    top = Math.min(
      Math.max(top, VIEWPORT_PADDING),
      Math.max(maxTop, VIEWPORT_PADDING),
    );
    left = Math.min(
      Math.max(left, VIEWPORT_PADDING),
      Math.max(maxLeft, VIEWPORT_PADDING),
    );

    style.value = {
      position: 'fixed',
      top: `${top}px`,
      left: `${left}px`,
    };
  }

  function bindPositionListeners() {
    window.addEventListener('resize', updatePosition);
    window.addEventListener('scroll', updatePosition, true);
  }

  function unbindPositionListeners() {
    window.removeEventListener('resize', updatePosition);
    window.removeEventListener('scroll', updatePosition, true);
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
      bindPositionListeners();
    } else {
      document.removeEventListener('click', onClickOutside);
      unbindPositionListeners();
    }
  });

  onBeforeUnmount(() => {
    document.removeEventListener('click', onClickOutside);
    unbindPositionListeners();
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
    position: fixed;
    z-index: 1000;
    background-color: var(--surface-container-low);
    color: var(--on-surface);
    border-radius: 15px;
    padding: 6px;
    box-shadow: 1px 2px 15px rgba(0, 0, 0, 0.1);
  }
  .dark .app-dropdown {
    box-shadow: 1px 3px 15px rgba(0, 0, 0, 0.4);
  }
</style>
