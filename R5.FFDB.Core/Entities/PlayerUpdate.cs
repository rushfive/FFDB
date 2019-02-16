using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Entities
{
	public class PlayerUpdate
	{
		public int? Number { get; set; }
		public Position? Position { get; set; }
		public RosterStatus? Status { get; set; }
	}
}
