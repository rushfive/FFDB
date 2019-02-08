using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData
{
	// all core data sources require these associated types:
	// - StaticCoreDataSource implementation
	// - VersionedModel
	// - SourceToVersionedMapper (http/disk to versioned model)
	// - VersionedToEntityMapper

	//public interface ISourceVersion<TEntity>
	//{
	//	int Version { get; }
		
	//	Func<ICoreDataSource<TEntity>> CreateSource { get; }
	//}

	public abstract class SourceVersion<TCoreData, TKey>
	{
		public abstract int Version { get; }
		public Func<ICoreDataSource<TCoreData, TKey>> CreateSource { get; }

		protected SourceVersion(Func<ICoreDataSource<TCoreData, TKey>> createSource)
		{
			CreateSource = createSource;
		}
		
	}
}
