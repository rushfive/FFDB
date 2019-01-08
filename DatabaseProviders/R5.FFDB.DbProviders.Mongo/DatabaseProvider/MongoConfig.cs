using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.Mongo.DatabaseProvider
{
	public class MongoConfig
	{
		public string ConnectionString { get; set; }
		public string Host { get; set; }
		public string DatabaseName { get; set; }
	}
}
