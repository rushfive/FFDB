using System.Threading.Tasks;

namespace R5.FFDB.Components.ValueProviders
{
	// lazy loaded values resolved async
	public abstract class AsyncValueProvider<T>
	{
		private T _value { get; set; }
		private bool _isSet { get; set; }

		public async Task<T> GetAsync()
		{
			if (_isSet)
			{
				return _value;
			}

			_value = await ResolveValueAsync();
			_isSet = true;

			return _value;
		}

		protected abstract Task<T> ResolveValueAsync();
	}
}
