using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Core.Models
{
	public class DepthChart
	{
		public int TeamId { get; set; }

		// Offense
		public DepthChartCategory QuarterBacks { get; } = new DepthChartCategory();
		public DepthChartCategory RunningBacks { get; } = new DepthChartCategory();
		public DepthChartCategory WideReceivers { get; } = new DepthChartCategory();
		public DepthChartCategory TightEnds { get; } = new DepthChartCategory();
		public DepthChartCategory LeftTackles { get; } = new DepthChartCategory();
		public DepthChartCategory LeftGuards { get; } = new DepthChartCategory();
		public DepthChartCategory Centers { get; } = new DepthChartCategory();
		public DepthChartCategory RightGuards { get; } = new DepthChartCategory();
		public DepthChartCategory RightTackles { get; } = new DepthChartCategory();

		// Defense
		public DepthChartCategory LeftDefensiveEnds { get; } = new DepthChartCategory();
		public DepthChartCategory LeftDefensiveTackles { get; } = new DepthChartCategory();
		public DepthChartCategory RightDefensiveTackles { get; } = new DepthChartCategory();
		public DepthChartCategory RightDefensiveEnds { get; } = new DepthChartCategory();
		public DepthChartCategory WeaksideLineBackers { get; } = new DepthChartCategory();
		public DepthChartCategory MiddleLinesBackers { get; } = new DepthChartCategory();
		public DepthChartCategory StrongsideLineBackers { get; } = new DepthChartCategory();
		public DepthChartCategory StrongSafeties { get; } = new DepthChartCategory();
		public DepthChartCategory FreeSafeties { get; } = new DepthChartCategory();
		public DepthChartCategory RightCornerBacks { get; } = new DepthChartCategory();
		public DepthChartCategory LeftCornerBacks { get; } = new DepthChartCategory();

		// Special Team
		public DepthChartCategory PlaceKickers { get; } = new DepthChartCategory();
		public DepthChartCategory Punters { get; } = new DepthChartCategory();
		public DepthChartCategory Holders { get; } = new DepthChartCategory();
		public DepthChartCategory PuntReturners { get; } = new DepthChartCategory();
		public DepthChartCategory KickReturners { get; } = new DepthChartCategory();
		public DepthChartCategory LongSnappers { get; } = new DepthChartCategory();
	}

	public class DepthChartCategory
	{
		// New list for each time a position type appears on the depth chart.
		// String values represent NFL Ids, the order represents depth chart rankings
		public List<List<string>> Players { get; private set; }

		public bool ContainsPlayers()
		{
			return Players != null && Players.Any(pl => pl.Any());
		}

		public void Update(List<List<string>> updatedPlayers)
		{
			Players = updatedPlayers;
		}
	}
}
