using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components
{
	public class SourceDataScrapeException : Exception
	{
		private string _sourceData { get; }

		public SourceDataScrapeException(string message, string sourceData)
			: base(message)
		{
			_sourceData = sourceData;
		}
	}
}
