using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData
{
	public static class ServicesExtensions
	{
		public static IServiceCollection AddCoreDataSources(this IServiceCollection services)
		{
			return services
				.AddRosterServices()
				.AddWeekMatchupServices();
		}

		private static IServiceCollection AddRosterServices(this IServiceCollection services)
		{
			services
				.AddScoped<
					Dynamic.Rosters.Sources.V1.IRosterSource,
					Dynamic.Rosters.Sources.V1.RosterSource>()
				.AddScoped<
					Dynamic.Rosters.Sources.V1.IRosterScraper,
					Dynamic.Rosters.Sources.V1.RosterScraper>()
				.AddScoped<
					Dynamic.Rosters.Sources.V1.Mappers.IToVersionedModelMapper,
					Dynamic.Rosters.Sources.V1.Mappers.ToVersionedModelMapper>()
				.AddScoped<
					Dynamic.Rosters.Sources.V1.Mappers.IToCoreDataMapper,
					Dynamic.Rosters.Sources.V1.Mappers.ToCoreDataMapper>()
				.AddSingleton<
					Dynamic.Rosters.IRosterCache,
					Dynamic.Rosters.RosterCache>();

			return services;
		}

		private static IServiceCollection AddWeekMatchupServices(this IServiceCollection services)
		{
			services
				.AddScoped<
					Static.WeekMatchups.Sources.V1.IWeekMatchupSource,
					Static.WeekMatchups.Sources.V1.WeekMatchupSource>()
				.AddScoped<
					Static.WeekMatchups.Sources.V1.Mappers.IToVersionedModelMapper,
					Static.WeekMatchups.Sources.V1.Mappers.ToVersionedModelMapper>()
				.AddScoped<
					Static.WeekMatchups.Sources.V1.Mappers.IToCoreDataMapper,
					Static.WeekMatchups.Sources.V1.Mappers.ToCoreDataMapper>()
				.AddScoped<
					Static.WeekMatchups.IWeekMatchupsCache,
					Static.WeekMatchups.WeekMatchupsCache>();

			return services;
		}
	}
}
