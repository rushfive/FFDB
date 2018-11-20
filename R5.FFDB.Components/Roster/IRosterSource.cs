using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Roster
{
	public interface IRosterSource : ISource
	{
		Task<List<Core.Models.Roster>> GetAsync();
	}
}
