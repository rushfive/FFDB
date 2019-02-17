using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using R5.Lib.Cache.AsyncLazyCache;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddAsyncLazyCache(this IServiceCollection services)
		{
			if (services == null)
			{
				throw new ArgumentNullException(nameof(services), "Service collection must be provided.");
			}

			services.TryAdd(ServiceDescriptor.Singleton<IMemoryCache, MemoryCache>());
			services.TryAdd(ServiceDescriptor.Singleton<IAsyncLazyCache, AsyncLazyCache>());

			return services;
		}
	}
}
