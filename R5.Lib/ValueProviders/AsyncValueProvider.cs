using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Lib.ValueProviders
{
	// lazy loaded values resolved async
	public abstract class AsyncValueProvider<T>
	{
		private T _value { get; set; }
		private bool _isSet { get; set; }
		private string _valueLabel { get; set; }

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

			try
			{
				_value = await ResolveValueAsync();
				_isSet = true;

				return _value;
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Failed to resolve async value for '{_valueLabel}'.", ex);
			}
		}

		protected abstract Task<T> ResolveValueAsync();
	}
}
