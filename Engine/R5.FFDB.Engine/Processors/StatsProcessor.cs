using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Pipelines.Stats;
using R5.FFDB.Core.Models;
using R5.Internals.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace R5.FFDB.Engine.Processors
{
	/// <summary>
	/// Contains the Engine tasks specific to stats.
	/// </summary>
	public class StatsProcessor
	{
		private IServiceProvider _serviceProvider { get; }

		public StatsProcessor(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		/// <summary>
		/// Add stats for every possible week that's missing in the database.
		/// </summary>
		public Task AddMissingAsync()
		{
			var context = new UpdateMissingPipeline.Context();

			var pipeline = _serviceProvider.Create<UpdateMissingPipeline>();

			return pipeline.ProcessAsync(context);
		}

		/// <summary>
		/// Add stats for the specified week.
		/// </summary>
		public Task AddForWeekAsync(WeekInfo week)
		{
			var context = new AddForWeekPipeline.Context
			{
				Week = week
			};

			var pipeline = _serviceProvider.Create<AddForWeekPipeline>();

			return pipeline.ProcessAsync(context);
		}

		/// <summary>
		/// Add stats for the specified week.
		/// </summary>
		public Task AddForWeekAsync(int season, int week) => AddForWeekAsync(new WeekInfo(season, week));
	}
}
