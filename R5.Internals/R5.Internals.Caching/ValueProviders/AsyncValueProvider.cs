using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace R5.Internals.Caching.ValueProviders
{
	// lazy loaded values resolved async
	public abstract class AsyncValueProvider<T>
	{
		private T _value { get; set; }
		private bool _isSet { get; set; }
		private string _valueLabel { get; set; }

		private SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

		protected AsyncValueProvider(string valueLabel)
		{
			_valueLabel = valueLabel;
		}

		public async Task<T> GetAsync()
		{
			if (_isSet)
			{
				return _value;
			}

			await _lock.WaitAsync();
			try
			{
				if (_isSet)
				{
					return _value;
				}

				_value = await ResolveValueAsync();
				_isSet = true;

				return _value;
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Failed to resolve async value for '{_valueLabel}'.", ex);
			}
			finally
			{
				_lock.Release();
			}
		}

		protected abstract Task<T> ResolveValueAsync();
	}
}
