<script setup lang="ts">
import { getRandomInt } from '@/utils/numbers';
import AppText from './AppText.vue';

interface Props {
  label?: string;
  placeholder?: string;
  type?: 'text' | 'password';
  mask?: string | RegExp | object;
}

defineProps<Props>();

const model = defineModel<string | null>({
  required: true,
});

const id = `input-${getRandomInt(111111, 999999)}`;
</script>

<template>
  <div class="app-input" :class="{ 'has-value': !!model }">
    <input
      :id="id"
      :placeholder="placeholder"
      v-model="model"
      class="input"
      :type="type"
      v-maska="mask"
    />
    <app-text
      v-if="label"
      el="label"
      :for="id"
      class="label"
      size="xs"
      color="secondary"
    >
      {{ label }}
    </app-text>
  </div>
</template>

<style scoped lang="scss">
.app-input {
  position: relative;
  margin-top: 10px;
  width: 100%;
}
.label {
  position: absolute;
  top: -6px;
  left: 6px;
  background-color: #ffffff;
  padding: 0 5px;
  border-radius: 20px;
  transition: all 0.2s ease-in-out;
}
.input {
  padding: 10px 12px;
  border: 1px solid #bbb;
  border-radius: 6px;
  outline: 1px solid transparent;
  font-size: 14px;
  transition: all 0.2s ease-in-out;
  width: calc(100% - 24px);
}
.input::placeholder {
  font: var(--font-s);
  color: #bbb;
}
.app-input:hover {
  .label {
    color: #000;
  }
  .input {
    border-color: #000;
  }
}
.input:focus {
  border-color: #000;

  + .label {
    color: #000;
  }
}
.app-input.has-value .label {
  color: #000;
}
</style>
