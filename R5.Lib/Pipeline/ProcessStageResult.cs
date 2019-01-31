using System;
using System.Collections.Generic;
using System.Text;

namespace R5.Lib.Pipeline
{
	public static class ProcessResult
	{
		public static readonly ProcessStageResult Continue = new Continue();
		public static readonly ProcessStageResult End = new End();
	}

	public abstract class ProcessStageResult { }

	internal class Continue : ProcessStageResult { }

	internal class End : ProcessStageResult { }
}
