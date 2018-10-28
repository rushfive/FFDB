using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace R5.FFDB.Sources
{
	// todo: more research on this, esp if im going to be making
	// parallel calls
	internal static class Http
	{
		internal static HttpClient Client = new HttpClient();
	}
}
