using R5.FFDB.Components.CoreData.PlayerProfile;
using R5.FFDB.Components.CoreData.PlayerProfile.Models;
using R5.FFDB.Components.ValueProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.Mappers
{
	public interface IPlayerIdMapper
	{
		string NflFromGsis(string gsisId);
		string NflFromEsb(string esbId);
	}
	public class PlayerIdMapper : IPlayerIdMapper
	{
		private PlayerProfiles _playerProfiles { get; }
		private Mappings _mappings { get; set; }

		public PlayerIdMapper(PlayerProfiles playerProfiles)
		{
			_playerProfiles = playerProfiles;
		}

		public string NflFromGsis(string gsisId)
		{
			InitializeMapsIfNotSet();

			if (!_mappings.GsisNflIdMap.TryGetValue(gsisId, out string nflId))
			{
				throw new InvalidOperationException($"Gsis id '{gsisId}' was not found in mappings.");
			}

			return nflId;
		}

		public string NflFromEsb(string esbId)
		{
			InitializeMapsIfNotSet();

			if (!_mappings.EsbNflIdMap.TryGetValue(esbId, out string nflId))
			{
				throw new InvalidOperationException($"Esb id '{esbId}' was not found in mappings.");
			}

			return nflId;
		}

		private void InitializeMapsIfNotSet()
		{
			if (_mappings != null)
			{
				return;
			}

			var mappings = new Mappings
			{
				GsisNflIdMap = new Dictionary<string, string>(),
				EsbNflIdMap = new Dictionary<string, string>()
			};
			
			_playerProfiles.Get()
				.ForEach(p =>
				{
					mappings.GsisNflIdMap[p.GsisId] = p.NflId;
					mappings.EsbNflIdMap[p.EsbId] = p.NflId;
				});

			_mappings = mappings;
		}

		private class Mappings
		{
			public Dictionary<string, string> GsisNflIdMap { get; set; }
			public Dictionary<string, string> EsbNflIdMap { get; set; }
		}
	}
}
