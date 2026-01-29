<script setup lang="ts">
  import { onMounted, ref, useTemplateRef } from 'vue';
  import { NetworkInvitationList } from '@/components/lists';
  import { InvitationModal } from '@/components/modals';
  import { useRoute, useRouter } from 'vue-router';

  const listRef = useTemplateRef<{ refetch: () => unknown }>('listRef');
  const modalShowed = ref(false);
  const route = useRoute();
  const router = useRouter();

  const onAddClick = () => {
    modalShowed.value = true;
  };

  const onSaved = () => {
    listRef.value?.refetch();
  };

  onMounted(() => {
    if (route.query.openInviteModal === '1') {
      modalShowed.value = true;
      router.replace({ query: {} });
    }
  });

  defineExpose({ onAddClick });
</script>

<template>
  <network-invitation-list ref="listRef"></network-invitation-list>
  <invitation-modal
    v-model:showed="modalShowed"
    @save="onSaved"
  ></invitation-modal>
</template>
