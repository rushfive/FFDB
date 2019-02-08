using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace R5.Lib.Tests.Cache.AsyncLazyCache
{
	public class AsyncLazyCacheTests : IDisposable
	{
		private Lib.Cache.AsyncLazyCache.AsyncLazyCache Cache;

		// setup
		public AsyncLazyCacheTests()
		{
			var memoryCache = new MemoryCache(
				new MemoryCacheOptions());

			Cache = new Lib.Cache.AsyncLazyCache.AsyncLazyCache(memoryCache);
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

		// todo: this is going to fail if only one thread is available when the
		// tasks are kicked off
		[Fact]
		public async Task AlreadyCached_ParallelTasks_ReturnsCachedValue()
		{
			var factoryInvoked = 0;
			var key = "key";

			var taskSetValue = "initial";

			Func<Task<string>> taskFactory1 = async () =>
			{
				await Task.Delay(500);

				factoryInvoked++;
				taskSetValue = "taskFactory1";
				
				return "value1";
			};

			Func<Task<string>> taskFactory2 = () =>
			{
				factoryInvoked++;
				taskSetValue = "taskFactory2";
				
				return Task.FromResult("value2");
			};

			Task<string> addEntryTask1 = Task.Run(() =>
			{
				Assert.Equal("initial", taskSetValue);
				
				return Cache.GetOrCreateAsync(key, taskFactory1);
			});

			Task<string> addEntryTask2 = Task.Run(async () =>
			{
				Assert.Equal("initial", taskSetValue);

				// give first task a head start to add 
				await Task.Delay(100);

				string value = await Cache.GetOrCreateAsync(key, taskFactory2);

				Assert.Equal("taskFactory1", taskSetValue);

				return value;
			});

			string[] result = await Task.WhenAll(addEntryTask1, addEntryTask2);

			Assert.Equal(1, factoryInvoked);
			Assert.Equal("value1", result[0]);
			Assert.Equal("value1", result[1]);
		}
	}
}
