using Microsoft.Extensions.DependencyInjection;
using R5.FFDB.Components.CoreData.Static.Players;

namespace R5.FFDB.Components.CoreData
{
	public static class ServicesExtensions
	{
		public static IServiceCollection AddCoreDataSources(this IServiceCollection services)
		{
			return services
				.AddRosterServices()
				.AddWeekMatchupServices()
				.AddTeamStatsServices()
				.AddPlayerWeekStatsServices()
				.AddPlayerAddServices();
		}

		private static IServiceCollection AddRosterServices(this IServiceCollection services)
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

		private static IServiceCollection AddWeekMatchupServices(this IServiceCollection services)
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

		private static IServiceCollection AddTeamStatsServices(this IServiceCollection services)
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

		private static IServiceCollection AddPlayerWeekStatsServices(this IServiceCollection services)
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

		private static IServiceCollection AddPlayerAddServices(this IServiceCollection services)
		{
			return services
				.AddScoped<IPlayerIdMappings, PlayerIdMappings>()
				.AddPlayerAddVersionedServices();
		}


		private static IServiceCollection AddPlayerAddVersionedServices(this IServiceCollection services)
		{
			services
				.AddScoped<
					Static.Players.Add.Sources.V1.IPlayerScraper,
					Static.Players.Add.Sources.V1.PlayerScraper>()
				.AddScoped<
					Static.Players.Add.Sources.V1.IPlayerAddSource,
					Static.Players.Add.Sources.V1.PlayerAddSource>()
				.AddScoped<
					Static.Players.Add.Sources.V1.Mappers.IToVersionedMapper,
					Static.Players.Add.Sources.V1.Mappers.ToVersionedMapper>()
				.AddScoped<
					Static.Players.Add.Sources.V1.Mappers.IToCoreMapper,
					Static.Players.Add.Sources.V1.Mappers.ToCoreMapper>();

			return services;
		}
	}
}
