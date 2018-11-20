using R5.FFDB.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Engine.SourceResolvers
{
	public abstract class SourceResolver<TSource>
		where TSource : ISource
	{
		private List<TSource> _configuredSources { get; } = new List<TSource>();
		private TSource _source { get; set; }
		private bool _sourceIsResolved => _source != null;

		protected abstract string SourceName { get; }

		protected SourceResolver(List<TSource> configuredSources)
		{
			_configuredSources = configuredSources;
		}

		public async Task<TSource> GetAsync()
		{
			if (_sourceIsResolved)
			{
				return _source;
			}

			if (!await ResolveSourceAsync())
			{
				throw new InvalidOperationException($"Failed to resolve a healthy source for {SourceName}.");
			}

			return _source;
		}

		private async Task<bool> ResolveSourceAsync()
		{
			foreach (var source in _configuredSources)
			{
				bool healthy = await source.IsHealthyAsync();
				if (healthy)
				{
					_source = source;
					return true;
				}
			}

			return false;
		}
	}
}
