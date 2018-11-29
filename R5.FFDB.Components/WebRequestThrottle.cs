using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components
{
	public class WebRequestThrottle
	{
		private Random _random { get; }
		public Func<int> Get { get; }

		public WebRequestThrottle(
			int throttle,
			(int min, int max)? randomizedThrottle)
		{
			if (!randomizedThrottle.HasValue)
			{
				Get = () => throttle;
			}
			else
			{
				_random = new Random();

				Get = () => _random.Next(
					randomizedThrottle.Value.min,
					randomizedThrottle.Value.max + 1);
			}
		}
	}
}
