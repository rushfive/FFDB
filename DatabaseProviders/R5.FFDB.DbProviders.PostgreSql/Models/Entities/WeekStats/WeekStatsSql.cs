using R5.FFDB.DbProviders.PostgreSql.Attributes;

namespace R5.FFDB.DbProviders.PostgreSql.Models.Entities.WeekStats
{
	public abstract class WeekStatsSql : SqlEntity
	{
		public abstract int Season { get; set; }
		public abstract int Week { get; set; }
	}
}
