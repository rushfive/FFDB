using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData
{
	[Obsolete("can probably replace with new StaticCoreDataSource")]
	public interface ICoreDataSource
	{
		string Label { get; }

		// should throw if NOT healthy and be logged
		Task CheckHealthAsync();
	}
}
