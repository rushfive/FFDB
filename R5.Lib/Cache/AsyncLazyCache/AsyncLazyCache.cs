using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace R5.Lib.Cache.AsyncLazyCache
{
	public interface IAsyncLazyCache : IDisposable
	{
		Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> taskFactory);
	}

	public class AsyncLazyCache : IAsyncLazyCache
	{
		private readonly IMemoryCache _cache;
		private readonly SemaphoreSlim _exclusiveLock = new SemaphoreSlim(1, 1);

		public AsyncLazyCache(IMemoryCache cache)
		{
			_cache = cache;
		}


		public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> taskFactory)
		{
			if (_cache.TryGetValue<T>(key, out T value))
			{
				return value;
			}

			await _exclusiveLock.WaitAsync().ConfigureAwait(false);

			try
			{
				if (!_cache.TryGetValue(key, out _))
				{
					T resolved = await taskFactory().ConfigureAwait(false);
					_cache.Set(key, resolved);
				}
			}
			finally
			{
				_exclusiveLock.Release();
			}

			return _cache.Get<T>(key);
		}

		public void Dispose()
		{
			_cache?.Dispose();
		}
	}
}
