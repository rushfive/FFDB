namespace R5.FFDB.DbProviders.PostgreSql.Models.Entities
{
	public abstract class SqlEntity
	{
		// will return the proper SQL condition for the 'WHERE' clause
		public abstract string UpdateWhereClause();
	}
}
