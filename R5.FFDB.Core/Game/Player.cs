using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Game
{
public	class Player
	{
		public Guid Id { get; set; }

		// NFL's player id
		public int NflId { get; set; }

		// some NFL legacy id, stil needed for some stuff
		public string EsbId { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }

		public int Team { get; set; } // id ref or some class?


	}
}
