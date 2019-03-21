using R5.FFDB.Components.Pipelines.Teams;
using R5.Internals.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace R5.FFDB.Engine.Processors
{
	public class TeamProcessor
	{
		private IServiceProvider _serviceProvider { get; }

		public TeamProcessor(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		// Updates player-team mappings. Doesn't update player data
		public Task UpdateRosterMappingsAsync()
		{
			var context = new UpdateRosterMappingsPipeline.Context();

			var pipeline = _serviceProvider.Create<UpdateRosterMappingsPipeline>();

			return pipeline.ProcessAsync(context);
		}
	}
}
