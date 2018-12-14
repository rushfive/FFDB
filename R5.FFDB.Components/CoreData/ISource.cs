using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData
{
	public interface ISource
	{
		Task<bool> IsHealthyAsync();
	}
}
