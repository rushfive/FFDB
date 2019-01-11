using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.Mongo.Models
{
	public class WeekStatsDocumentAdd
	{
		public WeekInfo Week { get; set; }
		public List<WeekStatsPlayerDocument> PlayerStats { get; } = new List<WeekStatsPlayerDocument>();
		public List<WeekStatsDstDocument> DstStats { get; } = new List<WeekStatsDstDocument>();
	}
}
