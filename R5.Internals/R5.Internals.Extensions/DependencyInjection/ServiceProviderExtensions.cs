﻿using Microsoft.Extensions.DependencyInjection;
using System;

namespace R5.Internals.Extensions.DependencyInjection
{
	public static class ServiceProviderExtensions
	{
		public static T Create<T>(this IServiceProvider provider, params object[] parameters)
		{
			return ActivatorUtilities.CreateInstance<T>(provider, parameters);
		}

		public static object Create(this IServiceProvider provider, Type instanceType, params object[] parameters)
		{
			return ActivatorUtilities.CreateInstance(provider, instanceType, parameters);
		}
	}
}
