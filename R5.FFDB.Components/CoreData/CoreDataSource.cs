using Newtonsoft.Json.Linq;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData
{

	//

	public interface IMapper<TIn, TOut>
	{
		TOut Map(TIn input);
	}

	public interface ICoreDataSource<TCoreData>
	{
		Task<TCoreData> GetAsync(WeekInfo week);
	}
	
	// TVersionedModel should represent whatever a week's worth of this source data is
	//  - eg if we're collecting a list of stats for a week, the model should contain a list of the stats
	// similarly, TCoreData should represent whatever represents a weeks worth of the core data
	//  in a lot of cases, its gonna be a list of something
	public abstract class CoreDataSource<TVersionedModel, TCoreData> : ICoreDataSource<TCoreData>
	{
		protected abstract bool SupportsFilePersistence { get; }
		
		private IMapper<string, TVersionedModel> _toVersionedMapper { get; }
		private IMapper<TVersionedModel, TCoreData> _toCoreDataMapper { get; }
		private ProgramOptions _programOptions { get; }
		private IDatabaseProvider _dbProvider { get; }

		protected CoreDataSource(
			IMapper<string, TVersionedModel> toVersionedMapper,
			IMapper<TVersionedModel, TCoreData> toCoreDataMapper,
			ProgramOptions programOptions,
			IDatabaseProvider dbProvider)
		{
			_toVersionedMapper = toVersionedMapper;
			_toCoreDataMapper = toCoreDataMapper;
			_programOptions = programOptions;
			_dbProvider = dbProvider;
		}

		public Task<TCoreData> GetAsync(WeekInfo week)
		{
			throw new NotImplementedException();
		}
	}

}
