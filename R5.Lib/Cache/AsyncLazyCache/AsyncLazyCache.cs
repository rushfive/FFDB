using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace R5.Lib.Cache.AsyncLazyCache
{
	public interface IAsyncLazyCache : IDisposable
	{
		Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> taskFactory);
		T GetOrCreate<T>(string key, Func<T> factory);
	}

	public class AsyncLazyCache : IAsyncLazyCache
	{
		private readonly IMemoryCache _cache;
		private static readonly KeyedLocks _locks = new KeyedLocks();

		public AsyncLazyCache(IMemoryCache cache)
		{
			_cache = cache;
		}

		public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factoryTask)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				throw new ArgumentNullException(nameof(key), "Valid cache key must be provided.");
			}
			if (factoryTask == null)
			{
				throw new ArgumentNullException(nameof(factoryTask), $"Factory task for key '{key}' must be provided.");
			}

			if (_cache.TryGetValue<T>(key, out T value))
			{
				return value;
			}

			using (await _locks.LockAsync(key))
			{
				if (!_cache.TryGetValue(key, out _))
				{
					T resolved = await factoryTask().ConfigureAwait(false);
					_cache.Set(key, resolved);
				}
			}

			return _cache.Get<T>(key);
		}

		public T GetOrCreate<T>(string key, Func<T> factory)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				throw new ArgumentNullException(nameof(key), "Valid cache key must be provided.");
			}
			if (factory == null)
			{
				throw new ArgumentNullException(nameof(factory), $"Factory for key '{key}' must be provided.");
			}

			if (_cache.TryGetValue<T>(key, out T value))
			{
				return value;
			}

			using (_locks.Lock(key))
			{
				if (!_cache.TryGetValue(key, out _))
				{
					T resolved = factory();
					_cache.Set(key, resolved);
				}
			}

			return _cache.Get<T>(key);
		}

		public void Dispose()
		{
			_cache?.Dispose();
		}

		// internal abstraction to manage the creating/releasing of keyed semaphores
		private sealed class KeyedLocks
		{
			private static readonly Dictionary<string, CountedLock> _locks
				= new Dictionary<string, CountedLock>(StringComparer.OrdinalIgnoreCase);

			public IDisposable Lock(string key)
			{
				AcquireLock(key).Wait();
				return new LockReleaser(key);
			}

			public async Task<IDisposable> LockAsync(string key)
			{
				await AcquireLock(key).WaitAsync().ConfigureAwait(false);
				return new LockReleaser(key);
			}

			// Returns the semaphore associated to the key. Either creates
			// a new one and adds to the map, or returns the already existing
			// one (additionally incrementing its referenced count)
			private SemaphoreSlim AcquireLock(string key)
			{
				CountedLock exclusiveLock;
				lock (_locks)
				{
					if (_locks.TryGetValue(key, out exclusiveLock))
					{
						exclusiveLock.Increment();
					}
					else
					{
						exclusiveLock = new CountedLock();
						_locks[key] = exclusiveLock;
					}
				}

				return exclusiveLock.Lock;
			}

			// Wrapper around a semaphore - keeps track of the count the key was referenced
			private sealed class CountedLock
			{
				public SemaphoreSlim Lock { get; } = new SemaphoreSlim(1, 1);
				private int _count { get; set; } = 1;

				public void Increment()
				{
					_count++;
				}

				public void Decrement()
				{
					_count--;
				}

				public bool NotReferenced()
				{
					return _count == 0;
				}
			}

			// IDisposable type returned after acquiring a lock. It will handle removing the
			// semaphore from the map if releasing results in no more references to the key.
			private sealed class LockReleaser : IDisposable
			{
				private string _key { get; }

				public LockReleaser(string key)
				{
					_key = key;
				}

				public void Dispose()
				{
					CountedLock exclusiveLock;
					lock (_locks)
					{
						exclusiveLock = _locks[_key];

						exclusiveLock.Decrement();
						if (!exclusiveLock.NotReferenced())
						{
							_locks.Remove(_key);
						}
					}

					exclusiveLock.Lock.Release();
				}
			}
		}
	}
}
