using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components;
using R5.FFDB.Components.Extensions.JsonConverters;
using R5.FFDB.Components.Pipelines.Setup;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Models;
using R5.FFDB.Engine.Processors;
using R5.Internals.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.FFDB.Engine
{
	public class FfdbEngine
	{
		public StatsProcessor Stats { get;  }
		public TeamProcessor Team { get; }
		public PlayerProcessor Player { get; }
		
		private IServiceProvider _serviceProvider { get; }
		private IDatabaseProvider _databaseProvider { get; }
		private LatestWeekValue _latestWeekValue { get; }

		public FfdbEngine(
			IAppLogger logger,
			IServiceProvider serviceProvider,
			IDatabaseProvider databaseProvider,
			LatestWeekValue latestWeekValue)
		{
			_serviceProvider = serviceProvider;
			_databaseProvider = databaseProvider;
			_latestWeekValue = latestWeekValue;
			
			Stats = new StatsProcessor(serviceProvider);
			Team = new TeamProcessor(serviceProvider);
			Player = new PlayerProcessor(serviceProvider);
		}

		static FfdbEngine()
		{
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings
			{
				Formatting = Formatting.None,
				Converters = new List<JsonConverter>
				{
					new WeekInfoJsonConverter()
				}
			};
		}
		
		public Task RunInitialSetupAsync(bool skipAddingStats)
		{
			var context = new InitialSetupPipeline.Context
			{
				SkipAddingStats = skipAddingStats
			};

			var pipeline = _serviceProvider.Create<InitialSetupPipeline>();

			return pipeline.ProcessAsync(context);
		}

		public Task<bool> HasBeenInitializedAsync()
		{
			IDatabaseContext dbContext = _databaseProvider.GetContext();
			return dbContext.HasBeenInitializedAsync();
		}

		public Task<WeekInfo> GetLatestWeekAsync()
		{
			return _latestWeekValue.GetAsync();
		}

		public Task<List<WeekInfo>> GetAllUpdatedWeeksAsync()
		{
			IDatabaseContext dbContext = _databaseProvider.GetContext();
			return dbContext.UpdateLog.GetAsync();
		}
	}
}
