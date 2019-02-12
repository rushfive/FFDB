using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData
{
	public interface IAsyncMapper<TIn, TOut, TSourceKey>
	{
		Task<TOut> MapAsync(TIn input, TSourceKey sourceKey);
	}
}
