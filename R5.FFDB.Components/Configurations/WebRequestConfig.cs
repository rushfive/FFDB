using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.Configurations
{
	public class WebRequestConfig
	{
		public int ThrottleMilliseconds { get; }
		public (int min, int max)? RandomizedThrottle { get; }
		public Dictionary<string, string> Headers { get; }

		public WebRequestConfig(
			int throttleMilliseconds,
			(int min, int max)? randomizedThrottle,
			Dictionary<string, string> headers)
		{
			ThrottleMilliseconds = throttleMilliseconds;
			RandomizedThrottle = randomizedThrottle;
			Headers = headers;
		}
	}
}
