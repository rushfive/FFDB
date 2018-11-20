using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components
{
	public interface ISource
	{
		Task<bool> IsHealthyAsync();
	}
}
