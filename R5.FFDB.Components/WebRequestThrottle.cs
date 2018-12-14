using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components
{
	public class WebRequestThrottle
	{
		private Random _random { get; }
		private Func<int> _getFunc { get; }

		public WebRequestThrottle(
			int throttle,
			(int min, int max)? randomizedThrottle = null)
		{
			if (!randomizedThrottle.HasValue)
			{
				_getFunc = () => throttle;
			}
			else
			{
				_random = new Random();

				_getFunc = () => _random.Next(
					randomizedThrottle.Value.min,
					randomizedThrottle.Value.max + 1);
			}
		}

		public Task DelayAsync()
		{
			return Task.Delay(_getFunc());
		}
	}
}
