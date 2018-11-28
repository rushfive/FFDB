//using R5.FFDB.Components.Configurations;
//using System;
//using System.IO;

//namespace R5.FFDB.Engine.ConfigBuilders
//{
//	public class FileDownloadConfigBuilder
//	{
//		private string _weekStatsDirectory { get; set; }
//		private string _playerDataDirectory { get; set; }

//		public FileDownloadConfigBuilder SetWeekStatsDirectory(string directoryPath)
//		{
//			if (string.IsNullOrWhiteSpace(directoryPath))
//			{
//				throw new ArgumentNullException(nameof(directoryPath), "Week Stats directory path must be provided.");
//			}
//			if (!directoryPath.EndsWith("\\"))
//			{
//				directoryPath += "\\";
//			}
//			if (!Directory.Exists(directoryPath))
//			{
//				throw new ArgumentException($"Directory path '{directoryPath}' doesn't exist.");
//			}

//			_weekStatsDirectory = directoryPath;
//			return this;
//		}

//		public FileDownloadConfigBuilder SetPlayerDataDirectory(string directoryPath)
//		{
//			if (string.IsNullOrWhiteSpace(directoryPath))
//			{
//				throw new ArgumentNullException(nameof(directoryPath), "Player Data directory path must be provided.");
//			}
//			if (!directoryPath.EndsWith("\\"))
//			{
//				directoryPath += "\\";
//			}
//			if (!Directory.Exists(directoryPath))
//			{
//				throw new ArgumentException($"Directory path '{directoryPath}' doesn't exist.");
//			}

//			_playerDataDirectory = directoryPath;
//			return this;
//		}

//		internal FileDownloadConfig Build()
//		{
//			Validate();

//			return new FileDownloadConfig(_playerDataDirectory, _weekStatsDirectory);
//		}

//		private void Validate()
//		{
//			if (string.IsNullOrWhiteSpace(_weekStatsDirectory))
//			{
//				throw new InvalidOperationException("Week Stats directory path must be set.");
//			}
//			if (string.IsNullOrWhiteSpace(_playerDataDirectory))
//			{
//				throw new InvalidOperationException("Player Data directory path must be set.");
//			}
//			if (_weekStatsDirectory == _playerDataDirectory)
//			{
//				throw new InvalidOperationException("Directory paths for Week Stats and Player Data must be different.");
//			}
//		}
//	}
//}
