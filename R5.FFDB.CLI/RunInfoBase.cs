using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.CLI
{
	public abstract class RunInfoBase
	{
		public abstract string CommandKey { get; }
		public string ConfigFilePath { get; set; }
		public bool SkipRosterFetch { get; set; }
	}
}
