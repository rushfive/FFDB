using R5.FFDB.Components.Pipelines.Players;
using System;
using System.Threading.Tasks;

namespace R5.FFDB.Engine.Processors
{
	public class PlayerProcessor
	{
		private IServiceProvider _serviceProvider { get; }

		public PlayerProcessor(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public Task UpdateCurrentlyRosteredAsync()
		{
			var context = new UpdateCurrentlyRosteredPipeline.Context();

			var pipeline = UpdateCurrentlyRosteredPipeline.Create(_serviceProvider);

			return pipeline.ProcessAsync(context);
		}

		public Task UpdateAllExistingAsync()
		{
			var context = new UpdateAllPipeline.Context();

			var pipeline = UpdateAllPipeline.Create(_serviceProvider);

			return pipeline.ProcessAsync(context);
		}
	}
}
