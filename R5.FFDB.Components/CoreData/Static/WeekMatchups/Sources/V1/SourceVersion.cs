//using Microsoft.Extensions.DependencyInjection;
//using R5.FFDB.Core.Entities;
//using R5.FFDB.Core.Models;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace R5.FFDB.Components.CoreData.Static.WeekMatchups.Sources.V1
//{
//	public class SourceV1 : SourceVersion<WeekGameMatchup, WeekInfo>
//	{
//		public override int Version => 1;

//		public SourceV1(Func<ICoreDataSource<WeekGameMatchup, WeekInfo>> createSource)
//			: base(createSource)
//		{
//		}
//	}

//	//public class SourceVersionFactory
//	//{
//	//	private IServiceProvider _serviceProvider { get; }

//	//	public SourceVersionFactory(IServiceProvider serviceProvider)
//	//	{
//	//		_serviceProvider = serviceProvider;
//	//	}

//	//	public TSourceVersion Create<TSourceVersion, TEntity>()
//	//		where TSourceVersion : SourceVersion<TEntity>
//	//	{
//	//		Func<ICoreDataSource<TEntity>> createSource = () => ActivatorUtilities.CreateInstance<ICoreDataSource<TEntity>>(_serviceProvider);

//	//		return Activator.CreateInstance(typeof(TSourceVersion), new object[] { createSource }) as TSourceVersion;
//	//	}

//	//}
//}
