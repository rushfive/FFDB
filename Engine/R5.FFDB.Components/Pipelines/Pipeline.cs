using Microsoft.Extensions.Logging;
using R5.FFDB.Core;
using R5.Internals.Abstractions.Pipeline;
using R5.Internals.Extensions.DependencyInjection;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.Pipelines
{
	public abstract class Pipeline<TContext> : AsyncPipeline<TContext>
	{
		private IAppLogger _logger { get; }
		private IDisposable _contextProperty { get; set; }
		private Dictionary<Guid, IDisposable> _stageContextProperties { get; } = new Dictionary<Guid, IDisposable>();
		private IServiceProvider _serviceProvider { get; }

		protected Pipeline(
			IAppLogger logger,
			IServiceProvider serviceProvider,
			string name)
			: base(name)
		{
			_logger = logger;
			_serviceProvider = serviceProvider;
		}

		// Types should represent the implementations of AsyncPipelineStages
		protected abstract List<Type> Stages { get; }

		protected override List<AsyncPipelineStage<TContext>> GetStages()
		{
			var stages = new List<AsyncPipelineStage<TContext>>();

			foreach(var stageType in Stages)
			{
				var stage = _serviceProvider.Create(stageType) as AsyncPipelineStage<TContext>;
				stages.Add(stage);
			}

			return stages;
		}

		protected override void OnPipelineProcessStart(TContext context, string name)
		{
			_contextProperty = LogContext.PushProperty("PipelineStage", name);
			_logger.LogInformation("Pipeline started.");
		}

		protected override void OnPipelineProcessEnd(TContext context, string name)
		{
			_logger.LogInformation("Pipeline completed.");
			_contextProperty?.Dispose();
		}

		protected override Guid OnStageProcessStart(TContext context, string name)
		{
			var id = Guid.NewGuid();
			_stageContextProperties[id] = LogContext.PushProperty("PipelineStage", name);

			_logger.LogInformation("Stage started.");

			return id;
		}

		protected override void OnStageProcessEnd(Guid stageId, TContext context, string name)
		{
			_logger.LogInformation("Stage completed.");

			if (_stageContextProperties.TryGetValue(stageId, out IDisposable stageContext))
			{
				stageContext?.Dispose();
				_stageContextProperties.Remove(stageId);
			}
		}
	}
}
