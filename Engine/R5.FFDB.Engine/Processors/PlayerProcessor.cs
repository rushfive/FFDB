using R5.FFDB.Components.Pipelines.Players;
using R5.Internals.Extensions.DependencyInjection;
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

			var pipeline = _serviceProvider.Create<UpdateCurrentlyRosteredPipeline>();

			return pipeline.ProcessAsync(context);
		}
	}
}
