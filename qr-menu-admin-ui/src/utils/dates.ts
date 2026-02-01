export function expiresAt(dateTimeIso: string) {
  const now = new Date();
  const expiredAt = new Date(dateTimeIso);

  const diffMs = expiredAt.getTime() - now.getTime();
  if (diffMs <= 0) return 'Запрошення прострочене';

  const diffHours = Math.floor(diffMs / (1000 * 60 * 60));
  const days = Math.floor(diffHours / 24);
  const hours = diffHours % 24;

  if (days > 0) return `Діє ще ${days} дн.`;
  return `Діє ще ${hours} год.`;
}

export function formatDate(isoString: string): string {
  if (!isoString) return '';

  const date = new Date(isoString);
  const pad = (n: number) => n.toString().padStart(2, '0');

  const year = date.getFullYear();
  const month = pad(date.getMonth() + 1);
  const day = pad(date.getDate());
  const hours = pad(date.getHours());
  const minutes = pad(date.getMinutes());
  const seconds = pad(date.getSeconds());

  return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
}
