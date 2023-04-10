using AsyncKeyedLock;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace R5.Internals.Caching.Caches
{
	public interface IAsyncLazyCache : IDisposable
	{
		Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> taskFactory);
		T GetOrCreate<T>(string key, Func<T> factory);
		void Remove(string key);
	}

	public class AsyncLazyCache : IAsyncLazyCache
	{
		private readonly IMemoryCache _cache;
		private static readonly AsyncKeyedLocker<string> _locks = new AsyncKeyedLocker<string>(o =>
		{
			o.PoolSize = 20;
			o.PoolInitialFill = 1;
		});

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

			using (await _locks.LockAsync(key).ConfigureAwait(false))
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

		public void Remove(string key)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				throw new ArgumentNullException(nameof(key), "Valid cache key must be provided.");
			}

			using (_locks.Lock(key))
			{
				if (_cache.TryGetValue(key, out _))
				{
					_cache.Remove(key);
				}
			}
		}

		public void Dispose()
		{
			_cache?.Dispose();
		}
	}

	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddAsyncLazyCache(this IServiceCollection services)
		{
			if (services == null)
			{
				throw new ArgumentNullException(nameof(services), "Service collection must be provided.");
			}

			//services.TryAdd(ServiceDescriptor.Singleton<IMemoryCache, MemoryCache>());
			services.TryAdd(
				ServiceDescriptor.Singleton<IMemoryCache>(
					sp => new MemoryCache(new MemoryCacheOptions())));
			services.TryAdd(ServiceDescriptor.Singleton<IAsyncLazyCache, AsyncLazyCache>());

			return services;
		}
	}
}
