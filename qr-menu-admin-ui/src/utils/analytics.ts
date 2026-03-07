import type { AnalyticsRange } from '@/types/analytics';

export function getRangeStart(
  now: Date,
  range: AnalyticsRange,
  points: number,
): Date {
  const start = startOfDay(now);
  if (range === 'today') return start;
  return addDays(start, -(points - 1));
}

export function formatDuration(minutes: number): string {
  const rounded = Math.round(minutes);
  if (rounded <= 0) return '0 хв';
  if (rounded < 60) return `${rounded} хв`;
  const hours = Math.floor(rounded / 60);
  const rest = rounded % 60;
  return rest ? `${hours} год ${rest} хв` : `${hours} год`;
}

export function getChartMinWidth(pointsCount: number): number {
  return Math.max(340, pointsCount * 34);
}

export function getMaxTicksLimit(pointsCount: number): number {
  if (pointsCount <= 7) return pointsCount;
  if (pointsCount <= 24) return 12;
  return 10;
}

export function readCssVar(
  styles: CSSStyleDeclaration,
  name: string,
  fallback: string,
): string {
  const value = styles.getPropertyValue(name).trim();
  return value || fallback;
}

function startOfDay(date: Date): Date {
  const result = new Date(date);
  result.setHours(0, 0, 0, 0);
  return result;
}

function addDays(date: Date, days: number): Date {
  const result = new Date(date);
  result.setDate(result.getDate() + days);
  return result;
}
