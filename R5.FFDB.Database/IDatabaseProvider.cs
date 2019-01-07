using R5.FFDB.Database.DbContext;

namespace R5.FFDB.Database
{
	public interface IDatabaseProvider
	{
		IDatabaseContext GetContext();
	}
}
