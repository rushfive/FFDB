using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData
{
	public class SourceResult<TCoreData>
	{
		public TCoreData Value { get;  }
		public bool FetchedFromWeb { get;  }

		public SourceResult(
			TCoreData value,
			bool fetchedFromWeb)
		{
			Value = value;
			FetchedFromWeb = fetchedFromWeb;
		}
	}
}
