using Microsoft.Extensions.Caching.Memory;
using R5.Internals.Caching.Caches;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace R5.Internals.Caching.Tests
{
	public class AsyncLazyCacheTests : IDisposable
	{
		private AsyncLazyCache Cache;

		// setup
		public AsyncLazyCacheTests()
		{
			var memoryCache = new MemoryCache(
				new MemoryCacheOptions());

			Cache = new AsyncLazyCache(memoryCache);
		}

		// teardown
		public void Dispose()
		{
			Cache?.Dispose();
		}

		// Getting Cached Values

		[Fact]
		public async Task AlreadyCached_ReturnsCachedValue()
		{
			var factoryInvoked = 0;
			var key = "key";

			Func<Task<string>> taskFactory1 = () =>
			{
				factoryInvoked++;
				return Task.FromResult("value1");
			};

			Func<Task<string>> taskFactory2 = () =>
			{
				factoryInvoked++;
				return Task.FromResult("value2");
			};

			var resolved1 = await Cache.GetOrCreateAsync(key, taskFactory1);
			var resolved2 = await Cache.GetOrCreateAsync(key, taskFactory1);

			Assert.Equal("value1", resolved1);
			Assert.Equal(1, factoryInvoked);

			Assert.Equal("value1", resolved2);
			Assert.Equal(1, factoryInvoked);
		}
	}
}
