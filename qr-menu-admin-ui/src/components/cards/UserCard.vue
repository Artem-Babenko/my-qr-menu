<script lang="ts" setup>
  import { computed } from 'vue';
  import { AppCard, AppFlex, AppIcon, AppText } from '@/components/shared';
  import type { User } from '@/types/user';
  import { UserInitials } from '../other';
  import { CardDropdown } from '../dropdowns';
  import { usePermissions } from '@/composables';
  import { PermissionType } from '@/consts/roles';

  const props = defineProps<{ user: User }>();
  const emit = defineEmits<{ edit: [user: User] }>();

  const { hasAny } = usePermissions();

  const establishmentsCount = computed(() => props.user.accesses?.length ?? 0);
  const canEdit = computed(() => hasAny(PermissionType.usersEdit));

  const onEdit = () => emit('edit', props.user);

  const restaurantsLabel = computed(() => {
    const count = establishmentsCount.value;
    const mod10 = count % 10;
    const mod100 = count % 100;

    if (mod10 === 1 && mod100 !== 11) return `${count} ресторан`;
    if (mod10 >= 2 && mod10 <= 4 && (mod100 < 12 || mod100 > 14)) {
      return `${count} ресторани`;
    }
    return `${count} ресторанів`;
  });

  const lastActivityLabel = computed(() => {
    const value = props.user.lastActivityAt;
    if (!value) return 'Остання активність: —';

    const last = new Date(value);
    if (Number.isNaN(last.getTime())) return 'Остання активність: —';

    const diffMs = Date.now() - last.getTime();
    if (diffMs < 0) return `Остання активність: ${last.toLocaleString()}`;

    const diffMinutes = Math.floor(diffMs / 60000);
    if (diffMinutes < 1) return 'Остання активність: щойно';
    if (diffMinutes < 60) return `Остання активність: ${diffMinutes} хв тому`;

    const diffHours = Math.floor(diffMinutes / 60);
    if (diffHours < 24) return `Остання активність: ${diffHours} год тому`;

    const diffDays = Math.floor(diffHours / 24);
    if (diffDays < 7) return `Остання активність: ${diffDays} дн тому`;

    return `Остання активність: ${last.toLocaleDateString()}`;
  });
</script>

<template>
  <app-card class="user-card">
    <app-flex justify="space-between" align="flex-start">
      <app-flex :gap="10">
        <user-initials :name="props.user.name" :surname="props.user.surname" />
        <app-flex
          direction="column"
          align="flex-start"
          :gap="6"
          class="head-info"
        >
          <app-text weight="600">
            {{ props.user.name }} {{ props.user.surname }}
          </app-text>
          <app-text color="secondary" size="xs">
            <app-flex :gap="4">
              <app-icon name="Phone" :size="12"></app-icon>
              {{ props.user.phone }}
            </app-flex>
          </app-text>
        </app-flex>
      </app-flex>
      <card-dropdown
        :buttons="[
          {
            icon: 'Pencil',
            title: 'Редагувати',
            click: onEdit,
            disabled: () => !canEdit,
          },
        ]"
      ></card-dropdown>
    </app-flex>

    <app-flex direction="column" align="flex-start" :gap="8">
      <app-text color="secondary" size="xs">
        <app-flex :gap="4">
          <app-icon name="Mail" :size="12"></app-icon>
          {{ props.user.email }}
        </app-flex>
      </app-text>
      <app-text color="secondary" size="xs">
        <app-flex :gap="4">
          <app-icon name="Store" :size="12"></app-icon>
          {{ restaurantsLabel }}
        </app-flex>
      </app-text>
    </app-flex>

    <app-flex class="footer" justify="space-between">
      <app-text color="secondary" size="xs">
        {{ lastActivityLabel }}
      </app-text>
    </app-flex>
  </app-card>
</template>

<style scoped>
  .user-card {
    min-height: 170px;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    gap: 10px;
  }
  .head-info {
    min-width: 0;
  }
  .head-info .app-text {
    max-width: 100%;
    overflow: hidden;
    text-overflow: ellipsis;
  }
  .footer {
    border-top: 1px solid var(--outline-variant);
    padding-top: 15px;
  }
</style>
