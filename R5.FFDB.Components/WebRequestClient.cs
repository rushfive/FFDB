using R5.FFDB.Components.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components
{
	public interface IWebRequestClient
	{
		void ReInitialize();
		Task<string> GetStringAsync(string uri, bool throttle = true);
	}

	public class WebRequestClient : IWebRequestClient
	{
		private static HttpClient _client;
		private static CookieContainer _cookieContainer;
		private static HttpClientHandler _clientHandler;
		private static Random _random;

		private WebRequestConfig _config { get; }

		public WebRequestClient(WebRequestConfig config)
		{
			_config = config;
		}

		public void ReInitialize()
		{
			_client?.Dispose();

			_cookieContainer = new CookieContainer();
			_clientHandler = new HttpClientHandler
			{
				UseCookies = true,
				CookieContainer = _cookieContainer
			};
			_client = new HttpClient(_clientHandler);

			foreach(KeyValuePair<string, string> header in _config.Headers)
			{
				_client.DefaultRequestHeaders.Add(header.Key, header.Value);
			}
		}

		public async Task<string> GetStringAsync(string uri, bool throttle = true)
		{
			if (_client == null)
			{
				ReInitialize();
			}

			if (throttle)
			{
				await Task.Delay(GetRequestThrottle());
			}

			return await _client.GetStringAsync(uri);
		}

		private int GetRequestThrottle()
		{
			if (!_config.RandomizedThrottle.HasValue)
			{
				return _config.ThrottleMilliseconds;
			}

			return _random.Next(
				_config.RandomizedThrottle.Value.min,
				_config.RandomizedThrottle.Value.max + 1);
		}
	}



	// todo: more research on this, esp if im going to be making
	// parallel calls
	//public static class ManagedHttpClient
	//{
	//	private static HttpClient _client;
	//	private static CookieContainer _cookieContainer;
	//	private static HttpClientHandler _clientHandler;
	//	private static Dictionary<string, string> _defaultHeaders;

	//	static ManagedHttpClient()
	//	{
	//		ReinitializeClient();
	//	}

	//	private static void ReinitializeClient()
	//	{
	//		_cookieContainer = new CookieContainer();
	//		_clientHandler = new HttpClientHandler
	//		{
	//			UseCookies = true,
	//			CookieContainer = _cookieContainer
	//		};
	//		_client = new HttpClient(_clientHandler);

	//		if (_defaultHeaders != null)
	//		{
	//			foreach (KeyValuePair<string, string> header in _defaultHeaders)
	//			{
	//				_client.DefaultRequestHeaders.Add(header.Key, header.Value);
	//			}
	//		}
	//	}

	//	public static HttpClient GetClient() => _client;
	//}
}
