using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Models
{
	public enum Position
	{
		QB,
		RB,
		FB,
		WR,
		TE,
		OL,
		C,
		G,
		LG,
		RG,
		T,
		LT,
		RT,
		K,
		KR,
		DL,
		DE,
		DT,
		NT,
		LB,
		ILB,
		OLB,
		MLB,
		DB,
		CB,
		FS,
		SS,
		S,
		P,
		PR,

		// added from scraping NFL roster pages
		OT,
		OG,
		LS,
		SAF // ???
	}

	// differs from what are used in stats
	// curr source: ESPN team depth chart pages
	public enum DepthChartPosition
	{
		// Offense
		QB,
		RB,
		WR,
		TE,
		LT,
		LG,
		C,
		RG,
		RT,
		// DEF
		LDE,
		LDT,
		RDT,
		RDE,
		WLB,
		MLB,
		SLB,
		SS,
		FS,
		RCB,
		LCB,
		// ST
		PK,
		P,
		H,
		PR,
		KR,
		LS
	}
}
