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
using R5.Internals.Extensions.DependencyInjection;
using R5.Lib.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Pipelines.Stats
{
	public class AddForWeekPipeline : Pipeline<AddForWeekPipeline.Context>
	{
		private ILogger<AddForWeekPipeline> _logger { get; }

		public AddForWeekPipeline(
			ILogger<AddForWeekPipeline> logger,
			AsyncPipelineStage<Context> head,
			int nestedDepth)
			: base(logger, head, "Add Stats for Week", nestedDepth)
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

		public static AddForWeekPipeline Create(IServiceProvider sp, int nestedDepth)
		{
			var checkAlreadyUpdated = sp.Create<Stages.CheckAlreadyUpdated>(nestedDepth);

			var getPlayerWeekStats = sp.Create<Stages.GetPlayerWeekStats>(nestedDepth);
			var fetchSavePlayers = sp.Create<FetchPlayersStage<Context>>(nestedDepth);
			var addPlayerWeekStats = sp.Create<Stages.AddPlayerWeekStats>(nestedDepth);

			var getTeamWeekStats = sp.Create<Stages.GetTeamWeekStats>(nestedDepth);
			var addTeamWeekStats = sp.Create<Stages.AddTeamWeekStats>(nestedDepth);

			var getWeekMatchups = sp.Create<Stages.GetWeekMatchups>(nestedDepth);
			var addWeekMatchups = sp.Create<Stages.AddWeekMatchups>(nestedDepth);

			var addUpdateLog = sp.Create<Stages.AddUpdateLog>(nestedDepth);

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

			return sp.Create<AddForWeekPipeline>(chain, nestedDepth);
		}

		public static class Stages
		{
			public class CheckAlreadyUpdated : Stage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }

				public CheckAlreadyUpdated(
					ILogger<CheckAlreadyUpdated> logger,
					IDatabaseProvider dbProvider,
					int nestedDepth)
					: base(logger, "Check Stats Already Updated", nestedDepth)
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
					ILogger<GetPlayerWeekStats> logger,
					IDatabaseProvider dbProvider,
					IPlayerWeekStatsSource source,
					int nestedDepth)
					: base(logger, "Get Player Week Stats", nestedDepth)
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
					ILogger<AddPlayerWeekStats> logger,
					IDatabaseProvider dbProvider,
					int nestedDepth)
					: base(logger, "Add Player Week Stats", nestedDepth)
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
					ILogger<GetTeamWeekStats> logger,
					ITeamWeekStatsSource source,
					IWeekMatchupsCache weekMatchupsCache,
					WebRequestThrottle throttle,
					int nestedDepth)
					: base(logger, "Get Team Week Stats", nestedDepth)
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
					ILogger<AddTeamWeekStats> logger,
					IDatabaseProvider dbProvider,
					int nestedDepth)
					: base(logger, "Add Team Week Stats", nestedDepth)
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
					ILogger<GetWeekMatchups> logger,
					IWeekMatchupsCache cache,
					int nestedDepth)
					: base(logger, "Get Week Matchups", nestedDepth)
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
					ILogger<AddWeekMatchups> logger,
					IDatabaseProvider dbProvider,
					int nestedDepth)
					: base(logger, "Add Week Matchups", nestedDepth)
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
					ILogger<AddUpdateLog> logger,
					IDatabaseProvider dbProvider,
					int nestedDepth)
					: base(logger, "Add Update Log", nestedDepth)
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
