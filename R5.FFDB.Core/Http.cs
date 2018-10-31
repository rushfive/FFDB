using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Core
{
	// todo: more research on this, esp if im going to be making
	// parallel calls
	public static class Http
	{
		public static HttpClient Client = new HttpClient();

		public static class Request
		{
			public static Task<string> GetAsStringAsync(string uri)
			{
				return Http.Client.GetStringAsync(uri);
			}
		}
	}
}
