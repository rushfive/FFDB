## R5.FFDB.Core

_v1.0.0-alpha.1_

---

This document describes the core data models used by the Engine.

These models are the same types the database providers interface with (ie they receive them as method arguments, and return them when requested by the Engine).

They really represent a boundary between the Engine and any of the data sources used to fetch NFL data. Every data source provides a `AsyncMapper` that maps the original data received from the source, into these models the Engine can work with.

---

#### Models and Types in this Document

###### Enumerations

  - [WeekStatType](#week-stat-type)
  - [RosterStatus](#roster-status)
  - [Position](#position)

###### Entities

  - [Player](#player)
  - [PlayerAdd](#player-add)
  - [PlayerUpdate](#player-update)
  - [PlayerWeekStats](#player-week-stats)
  - [Roster](#roster)
  - [Team](#team)
  - [TeamWeekStats](#team-week-stats)
  - [WeekMatchup](#week-matchup)

###### Other Important Types

  - [IAppLogger](#iapplogger)
  - [IDatabaseProvider](#database-provider)
  - [IDatabaseContext](#database-context)

---

##### Week Stat Type

This enum contains all the weekly player stat types that are provided from NFL's Fantasy Football API.

```
public enum WeekStatType
{
  Pass_Attempts = 2,
  Pass_Completions = 3,
  Pass_Yards = 5,
  Pass_Touchdowns = 6,
  Pass_Interceptions = 7,
  Pass_Sacked = 8,
  Rush_Attempts = 13,
  Rush_Yards = 14,
  Rush_Touchdowns = 15,
  Receive_Catches = 20,
  Receive_Yards = 21,
  Receive_Touchdowns = 22,
  Return_Yards = 27,
  Return_Touchdowns = 28,
  Fumble_Recover_Touchdowns = 29,
  Fumbles_Lost = 30,
  Fumbles_Total = 31,
  TwoPointConversions = 32,
  Kick_PAT_Makes = 33,
  Kick_PAT_Misses = 34,
  Kick_ZeroTwenty_Makes = 35,
  Kick_TwentyThirty_Makes = 36,
  Kick_ThirtyForty_Makes = 37,
  Kick_FortyFifty_Makes = 38,
  Kick_FiftyPlus_Makes = 39,
  Kick_ZeroTwenty_Misses = 40,
  Kick_TwentyThirty_Misses = 41,
  Kick_ThirtyForty_Misses = 42,
  Kick_FortyFifty_Misses = 43,
  Kick_FiftyPlus_Misses = 44,
  DST_Sacks = 45,
  DST_Interceptions = 46,
  DST_FumblesRecovered = 47,
  DST_FumblesForced = 48,
  DST_Safeties = 49,
  DST_Touchdowns = 50,
  DST_BlockedKicks = 51,
  DST_ReturnYards = 52,
  DST_ReturnTouchdowns = 53,
  DST_PointsAllowed = 54,
  DST_YardsAllowed = 62,
  IDP_Tackles = 70,
  IDP_AssistedTackles = 71,
  IDP_Sacks = 72,
  IDP_Interceptions = 73,
  IDP_ForcedFumbles = 74,
  IDP_FumblesRecovered = 75,
  IDP_InterceptionTouchdowns = 76,
  IDP_FumbleTouchdowns = 77,
  IDP_BlockedKickTouchdowns = 78,
  IDP_BlockedKicks = 79,
  IDP_Safeties = 80,
  IDP_PassesDefended = 81,
  IDP_InterceptionReturnYards = 82,
  IDP_FumbleReturnYards = 83,
  IDP_TacklesForLoss = 84,
  IDP_QuarterBackHits = 85,
  IDP_SackYards = 86
}
```

Their API stores stats in a JavaScript object as key-value-pairs, where the numeric values of the enum represent the keys they use.

---

###### Roster Status

This enum contains the different status types a player can have while being on a team.

```
public enum RosterStatus
{
  ACT, // Active
  RES, // Injured Reserve
  NON, // Non football-related Injured Reserve
  SUS, // Suspended
  PUP, // Physically Unable to Perform
  UDF, // Unsigned Draft Pick
  EXE // Exempt
}
```

This data is scraped from each team's individual roster page, and can be updated when you run the command:

```
ffdb update-players
```

---

###### Position

This enum represents the different position types a player can have (eg QB, RB, WR, etc.)

```
public enum Position
{
  QB,
  RB,
  FB,
  WR,
  TE,
  OL,
  C,
  G,
  LG,
  RG,
  T,
  LT,
  RT,
  K,
  KR,
  DL,
  DE,
  DT,
  NT,
  LB,
  ILB,
  OLB,
  MLB,
  DB,
  CB,
  FS,
  SS,
  S,
  P,
  PR,
  OT,
  OG,
  LS,
  SAF
}
```

Note that the different types may have some overlaps. For whatever reason, some teams like to use different labels for the same positions. FFDB is not going to attepmt to discriminate between those similar types, nor will it try to resolve which label is the "correct" one to use.

---

_The rest of the types on this document represent either core Entities (such as player or team) or other important types used by the Engine._

Their class or interface definitions will not be included here, check out their files if you're interested.

---

###### Player
The model returned from the `IDatabaseProvider` for the Engine to use.

---

###### PlayerAdd
The model provided by the Engine for the `IDatabaseProvider` to add to the database.

---

###### PlayerUpdate
The model provided by the Engine for the `IDatabaseProvider` to add to the database.

---

###### PlayerWeekStats
This model represents a single player's stats for a single week.

---

###### Roster
This model represents a team's current roster information. It includes a list of `RosterPlayer` objects for each player that's on the team.

---

###### Team
This model represents a single team. Because the data very rarely changes, it's been added directly into the code.

The `Teams` helper class, also found in this same Core project, contains many helper methods for interacting with the teams.

---

###### TeamWeekStats
This model represents a single team's various stats for a single game. It includes data such as points scored for the different quarters, total offensive yards, etc.

---

###### WeekMatchup
This simple model represents a single game, specifying which two teams played against each other.

---

###### IAppLogger
This is the logging abstraction that's used by the Engine. If you are using your own `IDatabaseProvider` implementation, you receive the instance of the logger to use in your factory function.

---

###### IDatabaseProvider
The interface that must be implemented for the Engine to store data.

##### IDatabaseContext
Defines the various methods that are required for the Engine to run its many different tasks.