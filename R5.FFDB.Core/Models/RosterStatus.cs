using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Models
{
	public enum RosterStatus
	{
		ACT, // active
		RES, // injured reserve
		NON, // non football-related injured reserve
		SUS, // suspended
		PUP, // physically unable to perform
		UDF, // unsigned draft pick
		EXE // exempt
	}
}
