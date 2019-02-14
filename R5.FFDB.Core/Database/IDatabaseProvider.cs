namespace R5.FFDB.Core.Database
{
	public interface IDatabaseProvider
	{
		IDatabaseContext GetContext();
	}
}
