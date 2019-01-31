using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.Lib.Cache
{
	public abstract class ResolvableAsyncCache<TKey, TValue>
	{
		private Dictionary<TKey, TValue> _cache { get; } = new Dictionary<TKey, TValue>();

		public async Task<TValue> GetAsync(TKey key)
		{
			if (!_cache.TryGetValue(key, out TValue value))
			{
				value = await ResolveAsync(key);
				_cache[key] = value;
			}

			return value;
		}

		protected abstract Task<TValue> ResolveAsync(TKey key);
	}
}
