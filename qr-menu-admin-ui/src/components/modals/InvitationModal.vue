<script lang="ts" setup>
  import { computed, reactive, ref, watch } from 'vue';
  import {
    AppButton,
    AppCard,
    AppFlex,
    AppIcon,
    AppInput,
    AppLabel,
    AppModal,
    AppText,
  } from '../shared';
  import { PHONE_MASK } from '@/consts/masks';
  import type { NewInvitation } from '@/types/invitations';
  import { InvitationForm } from '../forms';
  import { usersApi } from '@/api/usersApi';
  import { invitationApi } from '@/api/invitationApi';

  const showed = defineModel<boolean>('showed', { required: true });

  const phone = ref('');
  const invitation = ref<NewInvitation>(createNewInvitation());
  const form = reactive({ showed: false, readonlyUser: false });

  watch(showed, () => {
    phone.value = '';
    invitation.value = createNewInvitation();
    form.showed = false;
    form.readonlyUser = false;
  });

  const sendDisabled = computed(() => {
    const inv = invitation.value;
    return (
      !inv.name ||
      !inv.phone ||
      !inv.surname ||
      !inv.establishmentId ||
      !inv.roleId
    );
  });

  const close = () => {
    showed.value = false;
  };

  function createNewInvitation(): NewInvitation {
    return {
      phone: null,
      name: null,
      surname: null,
      targetUserId: null,
      roleId: null,
      establishmentId: null,
    };
  }

  const findUser = async () => {
    if (!phone.value || form.showed) return;
    const { data: user } = await usersApi.search(phone.value);
    if (user) {
      invitation.value.targetUserId = user.id;
      invitation.value.name = user.name;
      invitation.value.surname = user.surname;
      invitation.value.phone = user.phone;
    } else {
      invitation.value.phone = phone.value;
    }
    form.readonlyUser = !!user;
    form.showed = true;
  };

  const sendInvitation = async () => {
    if (sendDisabled.value) return;
    const inv = invitation.value;
    const resp = inv.targetUserId
      ? await invitationApi.createForExistingUser({
          targetUserId: inv.targetUserId,
          roleId: inv.roleId!,
          establishmentId: inv.establishmentId!,
        })
      : await invitationApi.createForNewUser({
          phone: inv.phone!,
          name: inv.name!,
          surname: inv.surname!,
          roleId: inv.roleId!,
          establishmentId: inv.establishmentId!,
        });
    if (!resp.data?.invitationId) return;
    showed.value = false;
  };
</script>

<template>
  <app-modal v-model:showed="showed" title="Запросити користувача" :width="400">
    <app-text color="secondary" line="m">
      Знайдіть користувача за номером телефону або створіть нове запрошення
    </app-text>
    <div class="search-block">
      <app-label label="Номер телефону" for="phoneNumber"></app-label>
      <app-flex gap="10">
        <app-input
          v-model="phone"
          placeholder="+38..."
          :mask="PHONE_MASK"
          id="phoneNumber"
          :disabled="form.showed"
        >
        </app-input>
        <app-button @click="findUser" :disabled="form.showed">
          <app-icon name="Search" :size="16" :stroke-width="2.5"></app-icon>
          Знайти
        </app-button>
      </app-flex>
    </div>
    <div v-if="form.showed" class="form">
      <app-card v-if="!form.readonlyUser" class="accent-card" type="yellow">
        <app-text color="accent">
          <app-icon name="User"></app-icon>
        </app-text>
        <app-text color="accent" line="m">
          Користувача не знайдено. Заповніть форму для створення запрошення.
        </app-text>
      </app-card>
      <invitation-form
        v-model:invitation="invitation"
        :readonly-user="form.readonlyUser"
      ></invitation-form>
    </div>

    <app-flex class="form-buttons" justify="flex-end" gap="10">
      <app-button type="outline" @click="close">Скасувати</app-button>
      <app-button :disabled="sendDisabled" @click="sendInvitation">
        Надіслати запрошення
      </app-button>
    </app-flex>
  </app-modal>
</template>

<style scoped>
  .search-block {
    margin: 15px 0 10px;
  }
  .form-buttons {
    margin-top: 20px;
  }
  .accent-card {
    display: flex;
    align-items: start;
    gap: 10px;
    padding: 10px 15px;
    margin-top: 20px;
  }
  .accent-card .app-icon {
    margin-top: 2px;
  }
  .input-block {
    margin-top: 20px;
    width: 100%;
  }
</style>
