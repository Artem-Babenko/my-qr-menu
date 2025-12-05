<script setup lang="ts">
import type { Establishment } from '@/types/models';

const establishment = defineModel<Establishment>({ required: true });

defineProps<{ establishments: Establishment[] }>();
</script>

<template></template>
