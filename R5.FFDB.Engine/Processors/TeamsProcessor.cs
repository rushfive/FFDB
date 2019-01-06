using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.Roster.Values;
using R5.FFDB.Core.Models;
using R5.FFDB.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Engine.Processors
{
	public class TeamsProcessor
	{
		private ILogger<TeamsProcessor> _logger { get; }
		private IDatabaseProvider _dbProvider { get; }
		private RostersValue _rostersValue { get; }
		private IProcessorHelper _helper { get; }

		public TeamsProcessor(
			ILogger<TeamsProcessor> logger,
			IDatabaseProvider dbProvider,
			RostersValue rostersValue,
			IProcessorHelper helper)
		{
			_logger = logger;
			_dbProvider = dbProvider;
			_rostersValue = rostersValue;
			_helper = helper;
		}

		public async Task UpdateRostersAsync()
		{
			_logger.LogInformation("Beginning rosters update in database.");

			IDatabaseContext dbContext = _dbProvider.GetContext();
			List<Roster> rosters = await _rostersValue.GetAsync();

			List<string> rosterNflIds = rosters.SelectMany(r => r.Players).Select(p => p.NflId).ToList();
			await _helper.AddPlayerProfilesAsync(rosterNflIds, dbContext);

			await dbContext.Team.UpdateRostersAsync(rosters);

			_logger.LogInformation("Successfully updated rosters in database.");
		}
	}
}
