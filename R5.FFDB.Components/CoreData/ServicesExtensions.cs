using Microsoft.Extensions.DependencyInjection;
using R5.FFDB.Components.CoreData.Static.Players.Sources.V1;
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
				.AddRoster()
				.AddWeekMatchup()
				.AddTeamStats()
				.AddPlayerWeekStats()
				.AddPlayers();
		}

		private static IServiceCollection AddRoster(this IServiceCollection services)
		{
			return services
				.AddRosterVersionedServices();
		}

		private static IServiceCollection AddRosterVersionedServices(this IServiceCollection services)
		{
			services
				.AddScoped<
					Dynamic.Rosters.Sources.V1.IRosterSource,
					Dynamic.Rosters.Sources.V1.RosterSource>()
				.AddScoped<
					Dynamic.Rosters.Sources.V1.IRosterScraper,
					Dynamic.Rosters.Sources.V1.RosterScraper>()
				.AddScoped<
					Dynamic.Rosters.Sources.V1.Mappers.IToVersionedMapper,
					Dynamic.Rosters.Sources.V1.Mappers.ToVersionedModelMapper>()
				.AddScoped<
					Dynamic.Rosters.Sources.V1.Mappers.IToCoreMapper,
					Dynamic.Rosters.Sources.V1.Mappers.ToCoreMapper>()
				.AddSingleton<
					Dynamic.Rosters.IRosterCache,
					Dynamic.Rosters.RosterCache>();

			return services;
		}

		private static IServiceCollection AddWeekMatchup(this IServiceCollection services)
		{
			return services
				.AddWeekMatchupVersionedServices();
		}

		private static IServiceCollection AddWeekMatchupVersionedServices(this IServiceCollection services)
		{
			services
				.AddScoped<
					Static.WeekMatchups.Sources.V1.IWeekMatchupSource,
					Static.WeekMatchups.Sources.V1.WeekMatchupSource>()
				.AddScoped<
					Static.WeekMatchups.Sources.V1.Mappers.IToVersionedMapper,
					Static.WeekMatchups.Sources.V1.Mappers.ToVersionedMapper>()
				.AddScoped<
					Static.WeekMatchups.Sources.V1.Mappers.IToCoreMapper,
					Static.WeekMatchups.Sources.V1.Mappers.ToCoreMapper>()
				.AddScoped<
					Static.WeekMatchups.IWeekMatchupsCache,
					Static.WeekMatchups.WeekMatchupsCache>();

			return services;
		}

		private static IServiceCollection AddTeamStats(this IServiceCollection services)
		{
			return services
				.AddTeamStatsVersionedServices();
		}

		private static IServiceCollection AddTeamStatsVersionedServices(this IServiceCollection services)
		{
			services
				.AddScoped<
					Static.TeamStats.Sources.V1.ITeamWeekStatsSource,
					Static.TeamStats.Sources.V1.TeamWeekStatsSource>()
				.AddScoped<
					Static.TeamStats.Sources.V1.Mappers.IToVersionedMapper,
					Static.TeamStats.Sources.V1.Mappers.ToVersionedMapper>()
				.AddScoped<
					Static.TeamStats.Sources.V1.Mappers.IToCoreMapper,
					Static.TeamStats.Sources.V1.Mappers.ToCoreMapper>()
				.AddScoped<
					Static.TeamStats.ITeamWeekStatsCache,
					Static.TeamStats.TeamWeekStatsCache>();

			return services;
		}

		private static IServiceCollection AddPlayerWeekStats(this IServiceCollection services)
		{
			return services
				.AddPlayerWeekStatsVersionedServices();
		}

		private static IServiceCollection AddPlayerWeekStatsVersionedServices(this IServiceCollection services)
		{
			services
				.AddScoped<
					Static.PlayerStats.Sources.V1.IPlayerWeekStatsSource,
					Static.PlayerStats.Sources.V1.PlayerWeekStatsSource>()
				.AddScoped<
					Static.PlayerStats.Sources.V1.Mappers.IToVersionedMapper,
					Static.PlayerStats.Sources.V1.Mappers.ToVersionedMapper>()
				.AddScoped<
					Static.PlayerStats.Sources.V1.Mappers.IToCoreMapper,
					Static.PlayerStats.Sources.V1.Mappers.ToCoreMapper>();

			return services;
		}

		private static IServiceCollection AddPlayers(this IServiceCollection services)
		{
			return services
				.AddPlayersVersionedServices()
				.AddScoped<
					Static.Players.IPlayerIdMappings,
					Static.Players.PlayerIdMappings>();
		}

		private static IServiceCollection AddPlayersVersionedServices(this IServiceCollection services)
		{
			return services
				.AddScoped<IPlayerScraper, PlayerScraper>()
				.AddPlayerAddVersionedServices()
				.AddPlayerUpdateVersionedServices();
		}

		private static IServiceCollection AddPlayerAddVersionedServices(this IServiceCollection services)
		{
			services
				.AddScoped<
					Static.Players.Sources.V1.Add.IPlayerAddSource,
					Static.Players.Sources.V1.Add.PlayerAddSource>()
				.AddScoped<
					Static.Players.Sources.V1.Add.Mappers.IToVersionedMapper,
					Static.Players.Sources.V1.Add.Mappers.ToVersionedMapper>()
				.AddScoped<
					Static.Players.Sources.V1.Add.Mappers.IToCoreMapper,
					Static.Players.Sources.V1.Add.Mappers.ToCoreMapper>();

			return services;
		}

		private static IServiceCollection AddPlayerUpdateVersionedServices(this IServiceCollection services)
		{
			services
				.AddScoped<
					Static.Players.Sources.V1.Update.IPlayerUpdateSource,
					Static.Players.Sources.V1.Update.PlayerUpdateSource>()
				.AddScoped<
					Static.Players.Sources.V1.Update.Mappers.IToVersionedMapper,
					Static.Players.Sources.V1.Update.Mappers.ToVersionedMapper>()
				.AddScoped<
					Static.Players.Sources.V1.Update.Mappers.IToCoreMapper,
					Static.Players.Sources.V1.Update.Mappers.ToCoreMapper>();

			return services;
		}
	}
}
