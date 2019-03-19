using System;
using System.Collections.Generic;
using System.Text;

namespace R5.Internals.Abstractions.Pipeline
{
	public static class ProcessResult
	{
		public static Continue Continue => new Continue();

		public static End End => new End();
	}

	public abstract class ProcessStageResult { }
	public class Continue : ProcessStageResult { }
	public class End : ProcessStageResult { }
}
