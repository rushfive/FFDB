using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData;
using R5.FFDB.Components.CoreData.Static.PlayerStats.Sources.V1;
using R5.FFDB.Components.CoreData.Static.TeamStats.Models;
using R5.FFDB.Components.CoreData.Static.TeamStats.Sources.V1;
using R5.FFDB.Components.CoreData.Static.WeekMatchups;
using R5.FFDB.Components.Http;
using R5.FFDB.Components.Pipelines.CommonStages;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.Internals.Abstractions.Pipeline;
using R5.Internals.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Pipelines.Stats
{
	public class AddForWeekPipeline : Pipeline<AddForWeekPipeline.Context>
	{
		private IAppLogger _logger { get; }

		public AddForWeekPipeline(
			IAppLogger logger,
			AsyncPipelineStage<Context> head)
			: base(logger, head, "Add Stats for Week")
		{
			_logger = logger;
		}

		public class Context : IFetchPlayersContext
		{
			public WeekInfo Week { get; set; }
			public List<string> FetchNflIds { get; set; }
			public List<PlayerWeekStats> PlayerWeekStats { get; set; }
			public List<TeamWeekStats> TeamWeekStats { get; set; }
			public List<WeekMatchup> WeekMatchups { get; set; }
		}

		public static AddForWeekPipeline Create(IServiceProvider sp)
		{
			var checkAlreadyUpdated = sp.Create<Stages.CheckAlreadyUpdated>();

			var getPlayerWeekStats = sp.Create<Stages.GetPlayerWeekStats>();
			var fetchSavePlayers = sp.Create<FetchPlayersStage<Context>>();
			var addPlayerWeekStats = sp.Create<Stages.AddPlayerWeekStats>();

			var getTeamWeekStats = sp.Create<Stages.GetTeamWeekStats>();
			var addTeamWeekStats = sp.Create<Stages.AddTeamWeekStats>();

			var getWeekMatchups = sp.Create<Stages.GetWeekMatchups>();
			var addWeekMatchups = sp.Create<Stages.AddWeekMatchups>();

			var addUpdateLog = sp.Create<Stages.AddUpdateLog>();

			AsyncPipelineStage<Context> chain = checkAlreadyUpdated;
			chain
				.SetNext(getPlayerWeekStats)
				.SetNext(fetchSavePlayers)
				.SetNext(addPlayerWeekStats)
				.SetNext(getTeamWeekStats)
				.SetNext(addTeamWeekStats)
				.SetNext(getWeekMatchups)
				.SetNext(addWeekMatchups)
				.SetNext(addUpdateLog);

			return sp.Create<AddForWeekPipeline>(chain);
		}

		public static class Stages
		{
			public class CheckAlreadyUpdated : Stage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }

				public CheckAlreadyUpdated(
					IAppLogger logger,
					IDatabaseProvider dbProvider)
					: base(logger, "Check Stats Already Updated")
				{
					_dbProvider = dbProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					IDatabaseContext dbContext = _dbProvider.GetContext();

					bool alreadyUpdated = await dbContext.UpdateLog.HasUpdatedWeekAsync(context.Week);
					if (alreadyUpdated)
					{
						LogInformation($"Stats for week '{context.Week}' have already been added.");
						return ProcessResult.End;
					}

					return ProcessResult.Continue;
				}
			}

			public class GetPlayerWeekStats : Stage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }
				private IPlayerWeekStatsSource _source { get; }

				public GetPlayerWeekStats(
					IAppLogger logger,
					IDatabaseProvider dbProvider,
					IPlayerWeekStatsSource source)
					: base(logger, "Get Player Week Stats")
				{
					_dbProvider = dbProvider;
					_source = source;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					IDatabaseContext dbContext = _dbProvider.GetContext();

					HashSet<string> existingPlayers = (await dbContext.Player.GetAllAsync())
						.Select(p => p.NflId)
						.ToHashSet(StringComparer.OrdinalIgnoreCase);

					SourceResult<List<PlayerWeekStats>> result = await _source.GetAsync(context.Week);

					List<string> newIds = (result.Value.Select(s => s.NflId))
						.Where(id => !existingPlayers.Contains(id) && !TeamDataStore.IsTeam(id))
						.ToList();

					context.PlayerWeekStats = result.Value;
					context.FetchNflIds = newIds;

					return ProcessResult.Continue;
				}
			}

			public class AddPlayerWeekStats : Stage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }

				public AddPlayerWeekStats(
					IAppLogger logger,
					IDatabaseProvider dbProvider)
					: base(logger, "Add Player Week Stats")
				{
					_dbProvider = dbProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					IDatabaseContext dbContext = _dbProvider.GetContext();

					HashSet<string> alreadyUpdated = (await dbContext.PlayerStats.GetPlayerNflIdsAsync(context.Week))
						.ToHashSet(StringComparer.OrdinalIgnoreCase);

					if (alreadyUpdated.Any())
					{
						LogDebug($"{alreadyUpdated.Count} player week stats already added.");
					}

					List<PlayerWeekStats> remaining = context.PlayerWeekStats
						.Where(s => !alreadyUpdated.Contains(s.NflId))
						.ToList();

					LogDebug($"{remaining.Count} player week stats remaining to be added.");

					await dbContext.PlayerStats.AddAsync(remaining);

					return ProcessResult.Continue;
				}
			}

			public class GetTeamWeekStats : Stage<Context>
			{
				private ITeamWeekStatsSource _source { get; }
				private IWeekMatchupsCache _weekMatchupsCache { get; }
				private WebRequestThrottle _throttle { get; }

				public GetTeamWeekStats(
					IAppLogger logger,
					ITeamWeekStatsSource source,
					IWeekMatchupsCache weekMatchupsCache,
					WebRequestThrottle throttle)
					: base(logger, "Get Team Week Stats")
				{
					_source = source;
					_weekMatchupsCache = weekMatchupsCache;
					_throttle = throttle;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					var stats = new List<TeamWeekStats>();

					List<string> gameIds = await _weekMatchupsCache.GetGameIdsForWeekAsync(context.Week);
					foreach(var gameId in gameIds)
					{
						SourceResult<TeamWeekStatsSourceModel> result = await _source.GetAsync((gameId, context.Week));

						stats.Add(result.Value.HomeTeamStats);
						stats.Add(result.Value.AwayTeamStats);

						if (result.FetchedFromWeb)
						{
							await _throttle.DelayAsync();
						}
					}

					context.TeamWeekStats = stats;
					return ProcessResult.Continue;
				}
			}

			public class AddTeamWeekStats : Stage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }

				public AddTeamWeekStats(
					IAppLogger logger,
					IDatabaseProvider dbProvider)
					: base(logger, "Add Team Week Stats")
				{
					_dbProvider = dbProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					IDatabaseContext dbContext = _dbProvider.GetContext();

					HashSet<int> alreadyUpdated = (await dbContext.TeamStats.GetAsync(context.Week))
						.Select(s => s.TeamId)
						.ToHashSet();

					if (alreadyUpdated.Any())
					{
						LogDebug($"{alreadyUpdated.Count} team week stats already added.");
					}

					List<TeamWeekStats> remaining = context.TeamWeekStats
						.Where(s => !alreadyUpdated.Contains(s.TeamId))
						.ToList();

					LogDebug($"{remaining.Count} team week stats remaining to be added.");

					await dbContext.TeamStats.AddAsync(remaining);

					return ProcessResult.Continue;
				}
			}

			public class GetWeekMatchups : Stage<Context>
			{
				private IWeekMatchupsCache _cache { get; }

				public GetWeekMatchups(
					IAppLogger logger,
					IWeekMatchupsCache cache)
					: base(logger, "Get Week Matchups")
				{
					_cache = cache;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					List<WeekMatchup> matchups = await _cache.GetMatchupsForWeekAsync(context.Week);

					context.WeekMatchups = matchups;
					
					return ProcessResult.Continue;
				}
			}

			public class AddWeekMatchups : Stage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }

				public AddWeekMatchups(
					IAppLogger logger,
					IDatabaseProvider dbProvider)
					: base(logger, "Add Week Matchups")
				{
					_dbProvider = dbProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					IDatabaseContext dbContext = _dbProvider.GetContext();

					HashSet<string> alreadyUpdated = (await dbContext.WeekMatchups.GetAsync(context.Week))
						.Select(s => s.NflGameId)
						.ToHashSet(StringComparer.OrdinalIgnoreCase);

					if (alreadyUpdated.Any())
					{
						LogDebug($"{alreadyUpdated.Count} week matchups already added.");
					}

					List<WeekMatchup> remaining = context.WeekMatchups
						.Where(s => !alreadyUpdated.Contains(s.NflGameId))
						.ToList();

					LogDebug($"{remaining.Count} team week stats remaining to be added.");

					await dbContext.WeekMatchups.AddAsync(remaining);

					return ProcessResult.Continue;
				}
			}

			public class AddUpdateLog : Stage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }

				public AddUpdateLog(
					IAppLogger logger,
					IDatabaseProvider dbProvider)
					: base(logger, "Add Update Log")
				{
					_dbProvider = dbProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					IDatabaseContext dbContext = _dbProvider.GetContext();

					await dbContext.UpdateLog.AddAsync(context.Week);

					return ProcessResult.Continue;
				}
			}
		}
	}
}
