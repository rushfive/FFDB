using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.CLI.Commands
{
	public abstract class RunInfoBase
	{
		public abstract string CommandKey { get; }
		public string ConfigFilePath { get; set; }
		public bool SkipRosterFetch { get; set; }
		public bool SaveToDisk { get; set; }
		public bool SaveOriginalSourceFiles { get; set; }
	}
}
