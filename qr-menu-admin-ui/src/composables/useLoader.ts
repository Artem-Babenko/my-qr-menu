import { ref, unref, watch, type MaybeRef } from 'vue';

type MaybeGetter<T> = MaybeRef<T> | (() => T);

interface LoaderOptions<T> {
  keys: MaybeGetter<unknown>[];
  fn: () => Promise<T>;
  enabled?: MaybeGetter<boolean>;
}

export function useLoader<T>(options: LoaderOptions<T>) {
  const data = ref<T | null>(null);
  const loading = ref(false);
  const error = ref<unknown>(null);

  async function execute() {
    if (options.enabled && !resolve(options.enabled)) return;

    loading.value = true;
    error.value = null;

    try {
      data.value = await options.fn();
    } finally {
      loading.value = false;
    }
  }

  watch(() => options.keys.map(resolve), execute, { immediate: true });

  return { data, loading, error, refetch: execute };
}

function resolve<T>(v: MaybeRef<T> | (() => T)): T {
  if (typeof v === 'function') {
    return (v as () => T)();
  }

  return unref(v);
}
