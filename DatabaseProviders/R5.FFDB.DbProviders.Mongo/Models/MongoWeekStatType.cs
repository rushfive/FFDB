using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.Mongo.Models
{
	// keep camel-cased so it shows up correctly in mongo
	// mapped to Core.WeekStatType using their numeric values so
	// keep them the same
	public enum MongoWeekStatType
	{
		passAttempts = 2,
		passCompletions = 3,
		passYards = 5,
		passTouchdowns = 6,
		passInterceptions = 7,
		passSacked = 8,

		rushAttempts = 13,
		rushYards = 14,
		rushTouchdowns = 15,

		receiveCatches = 20,
		receiveYards = 21,
		receiveTouchdowns = 22,

		returnYards = 27,
		returnTouchdowns = 28,
		fumbleRecoverTouchdowns = 29,
		fumblesLost = 30,
		fumblesTotal = 31,
		twoPointConversions = 32,

		kickPatMakes = 33,
		kickPatMisses = 34,
		kickZeroTwentyMakes = 35,
		kickTwentyThirtyMakes = 36,
		kickThirtyFortyMakes = 37,
		kickFortyFiftyMakes = 38,
		kickFiftyPlusMakes = 39,
		kickZeroTwentyMisses = 40,
		kickTwentyThirtyMisses = 41,
		kickThirtyFortyMisses = 42,
		kickFortyFiftyMisses = 43,
		kickFiftyPlusMisses = 44,

		dstSacks = 45,
		dstInterceptions = 46,
		dstFumblesRecovered = 47,
		dstFumblesForced = 48,
		dstSafeties = 49,
		dstTouchdowns = 50,
		dstBlockedKicks = 51,
		dstReturnYards = 52,
		dstReturnTouchdowns = 53,
		dstPointsAllowed = 54,
		dstYardsAllowed = 62,

		idpTackles = 70,
		idpAssistedTackles = 71,
		idpSacks = 72,
		idpInterceptions = 73,
		idpForcedFumbles = 74,
		idpFumblesRecovered = 75,
		idpInterceptionTouchdowns = 76,
		idpFumbleTouchdowns = 77,
		idpBlockedKickTouchdowns = 78,
		idpBlockedKicks = 79,
		idpSafeties = 80,
		idpPassesDefended = 81,
		idpInterceptionReturnYards = 82,
		idpFumbleReturnYards = 83,
		idpTacklesForLoss = 84,
		idpQuarterBackHits = 85,
		idpSackYards = 86
	}
}
