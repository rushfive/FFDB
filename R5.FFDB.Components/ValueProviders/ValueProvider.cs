using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.ValueProviders
{
	public abstract class ValueProvider<T>
	{
		private T _value { get; set; }
		private bool _isSet { get; set; }
		private string _valueLabel { get; set; }

		protected ValueProvider(string valueLabel)
		{
			_valueLabel = valueLabel;
		}

		public T Get()
		{
			if (_isSet)
			{
				return _value;
			}

			try
			{
				_value = ResolveValue();
				_isSet = true;

				return _value;
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Failed to resolve value for '{_valueLabel}'.", ex);
			}
		}

		public void Set(T value)
		{
			if (_isSet)
			{
				throw new InvalidOperationException("Value has already been set.");
			}

			_value = value;
			_isSet = true;
		}

		protected abstract T ResolveValue();
	}
}
