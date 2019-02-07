using Microsoft.Extensions.DependencyInjection;
using R5.FFDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.Static.WeekGameMap.Sources.V1
{
	public class SourceV1 : SourceVersion<WeekGameMapping>
	{
		public override int Version => 1;

		public SourceV1(Func<ICoreDataSource<WeekGameMapping>> createSource)
			: base(createSource)
		{
		}

		//private IServiceProvider _serviceProvider { get; }

		//public SourceVersion(IServiceProvider serviceProvider)
		//{
		//	_serviceProvider = serviceProvider;
		//}

		//public Func<ICoreDataSource<WeekGameMapping>> CreateSourceFactory()
		//{
		//	return () => ActivatorUtilities.CreateInstance<WeekGameMapSource>(_serviceProvider);
		//}

		// might not need these 2 create mapper factories IF
		// the only place the mappers are used are within the dataSource
		//public Func<ToEntityMapper> CreateToEntityMapperFactory()
		//{
		//	return () => ActivatorUtilities.CreateInstance<ToEntityMapper>(_serviceProvider);
		//}

		//public Func<ToVersionedModelMapper> CreateToVersionedMapperFactory()
		//{
		//	return () => ActivatorUtilities.CreateInstance<ToVersionedModelMapper>(_serviceProvider);
		//}
	}

	public class SourceVersionFactory
	{
		private IServiceProvider _serviceProvider { get; }

		public SourceVersionFactory(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public TSourceVersion Create<TSourceVersion, TEntity>()
			where TSourceVersion : SourceVersion<TEntity>
		{
			Func<ICoreDataSource<TEntity>> createSource = () => ActivatorUtilities.CreateInstance<ICoreDataSource<TEntity>>(_serviceProvider);

			return Activator.CreateInstance(typeof(TSourceVersion), new object[] { createSource }) as TSourceVersion;
		}

	}
}
