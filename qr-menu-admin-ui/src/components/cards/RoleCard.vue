<script lang="ts" setup>
  import type { RoleView } from '@/types/roles';
  import { AppCard, AppFlex, AppIcon, AppText } from '../shared';
  import { CardDropdown } from '../dropdowns';

  defineProps<{ role: RoleView }>();
</script>

<template>
  <app-card class="role-card">
    <app-flex justify="space-between" align="flex-start">
      <app-flex :gap="10">
        <app-flex class="circle-icon" justify="center">
          <app-icon name="UserRoundCog" :size="20"></app-icon>
        </app-flex>
        <app-flex direction="column" align="flex-start" :gap="6">
          <app-text weight="600">{{ role.name }}</app-text>
          <app-text color="secondary" size="xs">
            <app-flex :gap="4">
              <app-icon name="Users" :size="12"></app-icon>
              {{ role.numberOfUsers }} користувачів
            </app-flex>
          </app-text>
        </app-flex>
      </app-flex>
      <card-dropdown></card-dropdown>
    </app-flex>
    <app-text class="description" color="secondary">
      {{ role.description }}
    </app-text>
    <app-flex class="permissions" justify="space-between">
      <app-text color="secondary">Дозволи</app-text>
      <app-flex justify="center" class="circle-num">
        <app-text size="xxs" weight="500">
          {{ role.permissions.length }}
        </app-text>
      </app-flex>
    </app-flex>
  </app-card>
</template>

<style scoped>
  .role-card {
    height: 160px;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
  }
  .description {
    line-height: 1.5;
  }
  .permissions {
    border-top: 1px solid var(--border);
    padding-top: 10px;
  }
  .circle-icon,
  .circle-num {
    background-color: var(--hover-on-secondary);
    width: 22px;
    height: 22px;
    border-radius: 50%;
    flex-shrink: 0;
  }
  .circle-icon {
    width: 40px;
    height: 40px;
  }
</style>
