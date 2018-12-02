using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.ErrorFileLog.ErrorTypes
{
	public class PlayerProfileFetchError : ErrorFile
	{
		[JsonProperty("nflId")]
		public string NflId { get; set; }
	}
}
