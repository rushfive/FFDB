using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.Extensions.Methods
{
	public static class ServiceProviderExtensions
	{
		public static T Create<T>(this IServiceProvider provider, params object[] parameters)
		{
			return ActivatorUtilities.CreateInstance<T>(provider, parameters);
		}
	}
}
