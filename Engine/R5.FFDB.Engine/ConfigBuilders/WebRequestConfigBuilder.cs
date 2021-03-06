﻿using R5.FFDB.Components.Configurations;
using System;
using System.Collections.Generic;

namespace R5.FFDB.Engine.ConfigBuilders
{
	/// <summary>
	/// Builder for the web request client used by the engine.
	/// </summary>
	public class WebRequestConfigBuilder
	{
		private int _throttleMilliseconds { get; set; } = 3000;
		private (int min, int max)? _randomizedThrottle { get; set; }
		private Dictionary<string, string> _headers { get; } = new Dictionary<string, string>();

		internal static WebRequestConfigBuilder WithDefaultBrowserHeaders()
		{
			return new WebRequestConfigBuilder().AddDefaultBrowserHeaders();
		}

		/// <summary>
		/// Sets a static delay amount to be used between HTTP requests.
		/// </summary>
		public WebRequestConfigBuilder SetThrottle(int milliseconds)
		{
			if (milliseconds < 0)
			{
				throw new ArgumentException("Throttle value must be a non-negative value.");
			}

			_throttleMilliseconds = milliseconds;
			return this;
		}

		/// <summary>
		/// Sets a min and max value for a randomized delay between HTTP requests.
		/// </summary>
		public WebRequestConfigBuilder SetRandomizedThrottle(int min, int max)
		{
			if (min < 0 || max < 0)
			{
				throw new ArgumentException("Throttle values must be non-negative values.");
			}
			if (max <= min)
			{
				throw new ArgumentException("Max value must be greater than min.");
			}

			_randomizedThrottle = (min, max);
			return this;
		}

		/// <summary>
		/// Add a custom HTTP header to be included in every request.
		/// </summary>
		public WebRequestConfigBuilder AddHeader(string key, string value)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				throw new ArgumentNullException(nameof(key), "Header key must be provided.");
			}

			_headers[key] = value;
			return this;
		}

		/// <summary>
		/// Adds a default "User-Agent" header in each request.
		/// </summary>
		public WebRequestConfigBuilder AddDefaultBrowserHeaders()
		{
			// todo:
			// https://html-agility-pack.net/knowledge-base/14005175/html-agility-pack--web-scraping--and-spoofing-in-csharp

			var headers = new List<(string, string)>
			{
				( "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36" )
			};

			foreach (var (key, value) in headers)
			{
				AddHeader(key, value);
			}

			return this;
		}

		internal WebRequestConfig Build()
		{
			Validate();

			return new WebRequestConfig(
				_throttleMilliseconds,
				_randomizedThrottle,
				_headers);
		}

		private void Validate()
		{
			if (!_randomizedThrottle.HasValue && _throttleMilliseconds < 0)
			{
				throw new InvalidOperationException("Failed to build web request config because "
					+ "the throttle value is invalid.");
			}
		}
	}
}
