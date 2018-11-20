using R5.FFDB.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Engine.SourceResolvers
{
	public interface ISourceResolver<TSource>
		where TSource : ISource
	{
		Task<TSource> GetAsync();
	}
}
