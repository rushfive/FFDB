using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Lib.Pipeline
{
	public static class ProcessResult
	{
		//public static Continue<TReturn> ContinueWith<TReturn>(TReturn value) => new Continue<TReturn>(value);

		public static Continue Continue => new Continue();

		//public static End<TReturn> EndWith<TReturn>(TReturn value) => new End<TReturn>(value);

		public static End End => new End();
	}

	//public abstract class ProcessStageResult<TReturn>
	//{
	//	public TReturn ReturnValue { get; }

	//	protected ProcessStageResult(TReturn value)
	//	{
	//		ReturnValue = value;
	//	}
	//}
	public abstract class ProcessStageResult { }
	public class Continue : ProcessStageResult { }
	public class End : ProcessStageResult { }


	//public class Continue<TReturn> : ProcessStageResult<TReturn>
	//{
	//	public Continue(TReturn value) : base(value) { }
	//}

	//public class End<TReturn> : ProcessStageResult<TReturn>
	//{
	//	public End(TReturn value) : base(value) { }
	//}
}
