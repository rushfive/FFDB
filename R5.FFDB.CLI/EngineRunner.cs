using R5.FFDB.CLI.Commands;
using R5.FFDB.Core.Models;
using R5.FFDB.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CM = R5.FFDB.CLI.ConsoleManager;
using static System.Console;

namespace R5.FFDB.CLI
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
				case CheckHealth.RunInfo _:
					await RunCheckHealthAsync();
					break;
				case UpdateRosters.RunInfo _:
					await RunRostersUpdateAsync();
					break;
				case AddStats.RunInfo addStats:
					await RunAddStatsAsync(addStats);
					break;
				case RemoveStats.RunInfo removeStats:
					await RunRemoveStatsAsync(removeStats);
					break;
				case UpdatePlayers.RunInfo updatePlayers:
					await RunUpdatePlayersAsync(updatePlayers);
					break;
				case ViewUpdated.RunInfo _:
					await RunViewUpdatedWeeksAsync();
					break;
				default:
					throw new ArgumentOutOfRangeException($"'{runInfo.GetType().Name}' is an invalid '{nameof(RunInfoBase)}' type.");
			}
		}

		private Task RunInitialSetupAsync(InitialSetup.RunInfo runInfo)
		{
			return _engine.RunInitialSetupAsync(forceReinitialize: runInfo.ForceReinitialize);
		}

		private async Task RunCheckHealthAsync()
		{
			try
			{
				await _engine.CheckSourcesHealthAsync();
				CM.WriteLineColored("Health check passed.", ConsoleColor.Green);
			}
			catch (Exception ex)
			{
				CM.WriteError($"Health check failed with error: {ex.Message}");
				Console.WriteLine(ex.StackTrace);
			}
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

		private async Task RunRemoveStatsAsync(RemoveStats.RunInfo runInfo)
		{
			if (!runInfo.Week.HasValue)
			{
				await _engine.Stats.RemoveAllAsync();
				return;
			}

			var specifiedWeek = runInfo.Week.Value;

			List<WeekInfo> allUpdatedWeeks = await _engine.GetAllUpdatedWeeksAsync();
			if (!allUpdatedWeeks.Contains(specifiedWeek))
			{
				throw new InvalidOperationException($"Cannot remove stats for week '{specifiedWeek}' because they don't exist.");
			}

			await _engine.Stats.RemoveForWeekAsync(specifiedWeek);
		}

		private async Task RunUpdatePlayersAsync(UpdatePlayers.RunInfo runInfo)
		{
			// todo
		}

		private async Task RunViewUpdatedWeeksAsync()
		{
			List<WeekInfo> weeks = await _engine.GetAllUpdatedWeeksAsync();

			if (!weeks.Any())
			{
				WriteLine("No weeks have been updated.");
				return;
			}

			var groupsBySeason = weeks
				.GroupBy(w => w.Season)
				.OrderBy(g => g.Key)
				.ToList();

			var seasonUpdateLines = new List<string>();

			foreach(var group in groupsBySeason)
			{
				var rangedWeeks = RangedListBuilder.Build(group.Select(w => w.Week).ToList());
				seasonUpdateLines.Add($"{group.Key}: {string.Join(", ", rangedWeeks)}");
			}

			WriteLine("The following weeks have been updated:");
			seasonUpdateLines.ForEach(WriteLine);
		}
	}
}
