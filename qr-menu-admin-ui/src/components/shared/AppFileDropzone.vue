<script setup lang="ts">
  import { computed, ref } from 'vue';
  import AppIcon from './AppIcon.vue';
  import AppText from './AppText.vue';

  const props = withDefaults(
    defineProps<{
      disabled?: boolean;
      accept?: string;
      multiple?: boolean;
      title?: string;
      hint?: string;
    }>(),
    {
      disabled: false,
      accept: 'image/jpeg,image/png,image/webp',
      multiple: true,
      title: 'Перетягніть фото сюди',
      hint: 'або натисніть для вибору',
    },
  );

  const emit = defineEmits<{
    select: [files: File[]];
  }>();

  const fileInputRef = ref<HTMLInputElement | null>(null);
  const isDragging = ref(false);

  const dropzoneClass = computed(() => ({
    dragging: isDragging.value,
    disabled: props.disabled,
  }));

  function openPicker() {
    if (props.disabled) return;
    fileInputRef.value?.click();
  }

  function emitFiles(fileList: FileList | null) {
    if (props.disabled || !fileList || fileList.length === 0) return;
    emit('select', Array.from(fileList));
    if (fileInputRef.value) fileInputRef.value.value = '';
  }

  function onInputChange(event: Event) {
    const target = event.target as HTMLInputElement;
    emitFiles(target.files);
  }

  function onDragOver(event: DragEvent) {
    if (props.disabled) return;
    event.preventDefault();
    isDragging.value = true;
  }

  function onDragLeave(event: DragEvent) {
    event.preventDefault();
    isDragging.value = false;
  }

  function onDrop(event: DragEvent) {
    if (props.disabled) return;
    event.preventDefault();
    isDragging.value = false;
    emitFiles(event.dataTransfer?.files ?? null);
  }
</script>

<template>
  <div
    class="dropzone"
    :class="dropzoneClass"
    role="button"
    tabindex="0"
    @click="openPicker"
    @dragover="onDragOver"
    @dragleave="onDragLeave"
    @drop="onDrop"
  >
    <input
      ref="fileInputRef"
      class="hidden-input"
      type="file"
      :accept="accept"
      :multiple="multiple"
      :disabled="disabled"
      @change="onInputChange"
    />

    <app-icon name="Upload" :size="18" />
    <app-text size="s" weight="600">{{ title }}</app-text>
    <app-text size="xs" color="secondary">{{ hint }}</app-text>
  </div>
</template>

<style scoped>
  .dropzone {
    width: 100%;
    min-height: 86px;
    border: 1px dashed var(--outline-variant);
    border-radius: 12px;
    background: var(--surface-container-lowest);
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 4px;
    cursor: pointer;
    transition:
      border-color 0.2s ease,
      background-color 0.2s ease;
    text-align: center;
    padding: 12px;
  }

  .dropzone:hover,
  .dropzone.dragging {
    border-color: var(--primary);
    background: var(--surface-container-low);
  }

  .dropzone.disabled {
    opacity: 0.6;
    pointer-events: none;
  }

  .hidden-input {
    display: none;
  }
</style>
