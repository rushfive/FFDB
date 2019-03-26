using R5.FFDB.Components.Pipelines.Teams;
using R5.Internals.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace R5.FFDB.Engine.Processors
{
	/// <summary>
	/// Contains the Engine tasks specific to teams.
	/// </summary>
	public class TeamProcessor
	{
		private IServiceProvider _serviceProvider { get; }

		public TeamProcessor(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		/// <summary>
		/// Update the player-to-team mappings based off of the current roster information.
		/// </summary>
		public Task UpdateRosterMappingsAsync()
		{
			var context = new UpdateRosterMappingsPipeline.Context();

			var pipeline = _serviceProvider.Create<UpdateRosterMappingsPipeline>();

			return pipeline.ProcessAsync(context);
		}
	}
}
