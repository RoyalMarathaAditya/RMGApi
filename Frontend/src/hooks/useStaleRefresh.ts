/* Simple stale-while-revalidate hook.
   Returns cached data immediately if available, then refreshes in background.
   Falls back to fresh fetch if no cache entry exists. */

import { useCallback, useEffect, useRef, useState } from 'react';

interface CacheEntry<T> {
  data: T;
  timestamp: number;
}

const store = new Map<string, CacheEntry<unknown>>();
const TTL = 5 * 60 * 1000; // 5 minutes

export function useStaleRefresh<T>(
  key: string,
  fetcher: (signal: AbortSignal) => Promise<T>,
): { data: T | null; loading: boolean; error: string | null; refresh: () => void } {
  const [data, setData] = useState<T | null>(() => {
    const cached = store.get(key);
    if (cached && Date.now() - cached.timestamp < TTL) {
      return cached.data as T;
    }
    return null;
  });
  const [loading, setLoading] = useState(!data);
  const [error, setError] = useState<string | null>(null);
  const abortRef = useRef<AbortController | null>(null);
  const mountedRef = useRef(true);

  const fetch = useCallback(async () => {
    abortRef.current?.abort();
    const controller = new AbortController();
    abortRef.current = controller;

    setLoading(true);
    setError(null);
    try {
      const result = await fetcher(controller.signal);
      if (!mountedRef.current) return;
      store.set(key, { data: result, timestamp: Date.now() });
      setData(result);
      setLoading(false);
    } catch (err: any) {
      if (!mountedRef.current || controller.signal.aborted) return;
      setError(err?.response?.data?.message || err?.message || 'Failed to load');
      setLoading(false);
    }
  }, [key, fetcher]);

  useEffect(() => {
    mountedRef.current = true;
    if (!data) {
      fetch();
    }
    return () => {
      mountedRef.current = false;
    };
  }, [fetch, data]);

  const refresh = useCallback(() => {
    store.delete(key);
    fetch();
  }, [key, fetch]);

  return { data, loading, error, refresh };
}
