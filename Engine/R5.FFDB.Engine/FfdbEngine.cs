using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components;
using R5.FFDB.Components.Extensions.JsonConverters;
using R5.FFDB.Components.Http;
using R5.FFDB.Components.Pipelines.Setup;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Models;
using R5.FFDB.Engine.Processors;
using R5.Internals.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.FFDB.Engine
{
	/// <summary>
	/// The FFDB Engine that interfaces with the data sources and db providers.
	/// </summary>
	public class FfdbEngine
	{
		/// <summary>
		/// Processor for stats related tasks.
		/// </summary>
		public StatsProcessor Stats { get;  }

		/// <summary>
		/// Processor for team related tasks.
		/// </summary>
		public TeamProcessor Team { get; }

		/// <summary>
		/// Processor for player related tasks.
		/// </summary>
		public PlayerProcessor Player { get; }
		
		private IServiceProvider _serviceProvider { get; }
		private IDatabaseProvider _databaseProvider { get; }
		private LatestWeekValue _latestWeekValue { get; }
		private IWebRequestClient _webRequestClient { get; }

		public FfdbEngine(
			IAppLogger logger,
			IServiceProvider serviceProvider,
			IDatabaseProvider databaseProvider,
			LatestWeekValue latestWeekValue,
			IWebRequestClient webRequestClient)
		{
			_serviceProvider = serviceProvider;
			_databaseProvider = databaseProvider;
			_latestWeekValue = latestWeekValue;
			_webRequestClient = webRequestClient;
			
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
		
		/// <summary>
		/// Runs the initial database setup (eg creating tables).
		/// </summary>
		/// <param name="skipAddingStats">If true, will skip adding stats after the initial setup.</param>
		public Task RunInitialSetupAsync(bool skipAddingStats)
		{
			var context = new InitialSetupPipeline.Context
			{
				SkipAddingStats = skipAddingStats
			};

			var pipeline = _serviceProvider.Create<InitialSetupPipeline>();

			return pipeline.ProcessAsync(context);
		}

		/// <summary>
		/// Returns whether the database has already been initialized.
		/// </summary>
		public Task<bool> HasBeenInitializedAsync()
		{
			IDatabaseContext dbContext = _databaseProvider.GetContext();
			return dbContext.HasBeenInitializedAsync();
		}

		/// <summary>
		/// Returns the latest available week as officially determined by the NFL.
		/// </summary>
		public Task<WeekInfo> GetLatestWeekAsync()
		{
			return _latestWeekValue.GetAsync();
		}

		/// <summary>
		/// Returns the list of all weeks already updated in the database.
		/// </summary>
		public Task<List<WeekInfo>> GetAllUpdatedWeeksAsync()
		{
			IDatabaseContext dbContext = _databaseProvider.GetContext();
			return dbContext.UpdateLog.GetAsync();
		}

		/// <summary>
		/// Returns information regarding the state of the data repository.
		/// </summary>
		public async Task<DataRepoState> GetDataRepoStateAsync()
		{
			string uri = @"https://raw.githubusercontent.com/rushfive/FFDB.Data/master/state.json";

			string response = await _webRequestClient.GetStringAsync(uri, throttle: false);

			return JsonConvert.DeserializeObject<DataRepoState>(response);
		}
	}

	public class DataRepoState
	{
		[JsonProperty("timestamp")]
		public DateTime Timestamp { get; set; }

		[JsonProperty("enabled")]
		public bool Enabled { get; set; }
	}
}
