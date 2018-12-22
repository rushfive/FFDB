namespace R5.FFDB.DbProviders.PostgreSql.DatabaseProvider
{
	public class PostgresConfig
	{
		public string Host { get; set; }
		public string DatabaseName { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }

		public bool IsSecured => !string.IsNullOrWhiteSpace(Username)
			&& !string.IsNullOrWhiteSpace(Password);
	}
}
