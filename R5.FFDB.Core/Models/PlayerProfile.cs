using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Models
{
	// should only contain static type data, things that are
	// unlikely to change
	public class PlayerProfile
	{
		// Id examples are for Davantae Adams

		// Newest standard NFL Id.
		// eg: 2543495
		public string NflId { get; set; }

		// "Old" NFL Id. Lots of new stuff use their newer NflId, but this
		// is still needed in a lot of places.
		// eg: ADA218591
		public string EsbId { get; set; }

		// Id for GSIS database, not available to us but might need for mapping
		// eg: 00-0031381
		public string GsisId { get; set; }

		// eg: "http://static.nfl.com/static/content/public/static/img/getty/headshot/A/D/A/ADA218591.jpg"
		public string PictureUri { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		//public Position Position { get; set; }
		//public int? TeamId { get; set; }
		//public int Number { get; set; }
		public int Height { get; set; }
		public int Weight { get; set; }
		public DateTimeOffset DateOfBirth { get; set; }
		public string College { get; set; }
	}
}
