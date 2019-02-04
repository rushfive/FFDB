using R5.FFDB.Components.CoreData.Rosters.Values;
using R5.Lib.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Pipelines.CommonStages
{
	public interface IRosteredPlayersContext
	{
		List<string> RosteredNflIds { get; set; }
	}

	public class GetCurrentRosteredPlayerIds<TContext> : AsyncPipelineStage<TContext>
		where TContext : IRosteredPlayersContext
	{
		private RostersValue _rosters { get; }

		public GetCurrentRosteredPlayerIds(RostersValue rosters)
			: base("Get Currently Rostered Player Ids")
		{
			_rosters = rosters;
		}

		public override async Task<ProcessStageResult> ProcessAsync(TContext context)
		{
			List<string> ids = await _rosters.GetIdsAsync();

			context.RosteredNflIds = ids;

			return ProcessResult.Continue;
		}
	}
}
