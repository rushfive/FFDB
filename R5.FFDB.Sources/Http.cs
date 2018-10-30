using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Sources
{
	// todo: more research on this, esp if im going to be making
	// parallel calls
	internal static class Http
	{
		internal static HttpClient Client = new HttpClient();

		internal static class Request
		{
			internal static Task<string> GetAsStringAsync(string uri)
			{
				return Http.Client.GetStringAsync(uri);
			}
		}
	}
}
