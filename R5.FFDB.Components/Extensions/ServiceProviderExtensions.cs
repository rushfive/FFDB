using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.Extensions
{
	public static class ServiceProviderExtensions
	{
		public static T ResolveInstance<T>(this IServiceProvider provider, params object[] parameters)
		{
			return ActivatorUtilities.CreateInstance<T>(provider, parameters);
		}
	}
}
