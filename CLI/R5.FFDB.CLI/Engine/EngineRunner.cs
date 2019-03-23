using R5.FFDB.CLI.Commands;
using R5.FFDB.Core.Models;
using R5.FFDB.Engine;
using R5.Internals.Abstractions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using CM = R5.Internals.Abstractions.SystemConsole.ConsoleManager;

namespace R5.FFDB.CLI.Engine
{
	public class EngineRunner
	{
		private FfdbEngine _engine { get; }

		public EngineRunner(FfdbEngine engine)
		{
			_engine = engine;
		}

		public async Task RunAsync(RunInfoBase runInfo)
		{
			if (runInfo is InitialSetup.RunInfo initialSetup)
			{
				await RunInitialSetupAsync(initialSetup);
				return;
			}

			bool ffdbInitialized = await _engine.HasBeenInitializedAsync();
			if (!ffdbInitialized)
			{
				throw new InvalidOperationException("Initial setup must first be run before "
					+ $"you can use command '{runInfo.CommandKey}'.");
			}

			switch (runInfo)
			{
				case UpdateRosters.RunInfo _:
					await RunRostersUpdateAsync();
					break;
				case AddStats.RunInfo addStats:
					await RunAddStatsAsync(addStats);
					break;
				case UpdateRosteredPlayers.RunInfo updatePlayers:
					await RunUpdatePlayersAsync(updatePlayers);
					break;
				case ViewState.RunInfo _:
					await RunViewStateAsync();
					break;
				default:
					throw new ArgumentOutOfRangeException($"'{runInfo.GetType().Name}' is an invalid '{nameof(RunInfoBase)}' type.");
			}
		}

		private Task RunInitialSetupAsync(InitialSetup.RunInfo initialSetup)
		{
			return _engine.RunInitialSetupAsync(initialSetup.SkipAddingStats);
		}

		private Task RunRostersUpdateAsync()
		{
			return _engine.Team.UpdateRosterMappingsAsync();
		}

		private async Task RunAddStatsAsync(AddStats.RunInfo runInfo)
		{
			if (!runInfo.Week.HasValue)
			{
				await _engine.Stats.AddMissingAsync();
				return;
			}

			var earliestAvailable = new WeekInfo(2010, 1);
			var latestAvailable = await _engine.GetLatestWeekAsync();

			var specifiedWeek = runInfo.Week.Value;

			if (specifiedWeek < earliestAvailable)
			{
				throw new InvalidOperationException($"Cannot add stats for week '{specifiedWeek}'. "
					+ $"The earliest available week is '{earliestAvailable}'.");
			}
			if (specifiedWeek > latestAvailable)
			{
				throw new InvalidOperationException($"Cannot add stats for week '{specifiedWeek}'. "
					+ $"The latest available week is '{latestAvailable}'.");
			}

			await _engine.Stats.AddForWeekAsync(specifiedWeek);
		}

		private Task RunUpdatePlayersAsync(UpdateRosteredPlayers.RunInfo runInfo)
		{
			return _engine.Player.UpdateCurrentlyRosteredAsync();
		}

		private async Task RunViewStateAsync()
		{
			Task<WeekInfo> latestWeekTask = _engine.GetLatestWeekAsync();
			Task<DataRepoState> dataRepoStateTask = _engine.GetDataRepoStateAsync();
			Task<List<WeekInfo>> updatedWeeksTask = _engine.GetAllUpdatedWeeksAsync();

			await Task.WhenAll(
				latestWeekTask,
				dataRepoStateTask,
				updatedWeeksTask);

			var dataRepoState = dataRepoStateTask.Result;

			WriteLine("┌");

			if (dataRepoState.Enabled)
			{
				Write("│ Data Repository last updated: ");
				CM.WriteLineColoredReset($"{dataRepoState.Timestamp:d}", ConsoleColor.White);
				WriteLine("│ Will attempt fetching core data from the repo first.");
				WriteLine("│ If unavailable, falls back to fetching from the original sources.");
			}
			else
			{
				WriteLine("│ Data Repository is currently disabled. ");
				WriteLine("│ Will fetch all core data directly from the original sources. ");
			}

			WriteLine("│");
			Write("│ Latest available NFL week: ");
			CM.WriteLineColoredReset(latestWeekTask.Result.ToString(), ConsoleColor.White);
			WriteLine("│");

			List<WeekInfo> updatedWeeks = updatedWeeksTask.Result;
			if (!updatedWeeks.Any())
			{
				WriteLine("│ No weeks have been updated in your database.");
			}
			else
			{
				var groupsBySeason = updatedWeeks
					.GroupBy(w => w.Season)
					.OrderBy(g => g.Key)
					.ToList();

				var seasonUpdateLines = new List<string>();

				foreach (var group in groupsBySeason)
				{
					var rangedWeeks = RangedListBuilder.Build(group.Select(w => w.Week).ToList());
					seasonUpdateLines.Add($"{group.Key}: {string.Join(", ", rangedWeeks)}");
				}

				WriteLine("│ The following weeks have been updated in your database:");
				foreach(var seasonUpdates in seasonUpdateLines)
				{
					WriteLine($"│ {seasonUpdates}");
				}
			}

			WriteLine("└");
		}
	}
}
