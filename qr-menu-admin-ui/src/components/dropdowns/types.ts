import type { IconName } from '../shared';

export interface ActionButton {
  icon: IconName;
  title: string;
  click: () => void;
  disabled?: boolean | (() => boolean);
}
