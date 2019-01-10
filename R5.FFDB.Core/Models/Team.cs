using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Models
{
	public class Team
	{
		public int Id { get; set; }
		public string NflId { get; set; }
		public string Name { get; set; }
		public string ShortName { get; set; }
		public string Abbreviation { get; set; }
		public HashSet<string> PriorAbbreviations { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		public override string ToString()
		{
			return Abbreviation;
		}
	}
}
