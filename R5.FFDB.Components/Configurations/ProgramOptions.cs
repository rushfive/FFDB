﻿using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.Configurations
{
	public class ProgramOptions
	{
		public bool SkipRosterFetch { get; set; }
		public bool SaveToDisk { get; set; } // should be defaulted to TRUE
		public bool SaveOriginalSourceFiles { get; set; } // default = FALSE
	}
}
