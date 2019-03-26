## Mongo Schema

This document describes the schema for the current version _v1.0.0-alpha.1_

##### Quick Notes
- All enums are stored as their string representations.

There's several fields that serialize core enum values as strings. The definitions of these enums can be found in the `R5.FFDB.Core` project:

- (link to Cores readme)


---

### Collections

Every collection stores the same document model that maps to a strongly typed C# class.

###### ffdb.player

Field | BSON Type | Notes
---|---|---
_id | string | string representation of a CLR `Guid` type
nflId | string | 
teamId | 32-bit int | Value is nullable. A null-value indicates that the player is currently not rostered on a team.
esbId | string | 
gsisId | string | 
firstName | string | 
lastName | string | 
position | string | string representation of the `Position` enum type. A null-value probably means the player isn't rostered on a team.
number | 32-bit int | A null-value probably indicates the player isn't rostered on a team.
status | string | string representation of the `RosterStatus` enum type. A null-value probably indicates the player isn't rostered on a team.
height | 32-bit int | 
weight | 32-bit int | 
dateOfBith | array | Stores 2 values, the first representing time elapsed from the epoch, and the second being a timezone offset.
college | string | A null-value probably means the player never attended a college.

---

###### ffdb.team

Field | BSON Type | Notes
---|---|---
_id | 32-bit int |
nflId | string | 
name | string | 
abbreviation | string | 

---

##### Week Stats 

The following 2 collections, `ffdb.weekStatsPlayer` and `ffdb.weekStatsDst`, store stats in a `Dictionary` structure, which is serialized as an `object` in Mongo (CLR `BsonDocument` type). Only the stat types the player/team have recorded for the week exist in the document.

The `ffdb.weekStatsPlayer` documents also contain some `bool` fields that indicate whether the player has stats of a certain category recorded for the week. This makes certain types of queries much easier to run.

###### ffdb.weekStatsPlayer

Field | BSON Type | Notes
---|---|---
playerId | string | string representation of a CLR `Guid` type
teamId | 32-bit int | there may exist some null-values if the Engine was unable to resolve which team the player was on for the week.
season | 32-bit int | 
week | 32-bit int | 
stats | object | the keys/fields represent a `MongoWeekStatType`, the value is recorded as a `double`
hasPass | bool | indicates whether this player has _any_ passing stat types recorded 
hasRush | bool | indicates whether this player has _any_ rushing stat types recorded 
hasReceive | bool | indicates whether this player has _any_ receiving stat types recorded 
hasReturn | bool | indicates whether this player has _any_ return stat types recorded 
hasMisc | bool | indicates whether this player has _any_ misc stat types recorded. The `misc` stat types are: FumbleRecoverTouchdowns, FumblesLost, FumblesTotal, and TwoPointConversions
hasKick | bool | indicates whether this player has _any_ kicking stat types recorded 
hasIdp | bool | indicates whether this player has _any_ IDP stat types recorded 

###### ffdb.weekStatsDst

Field | BSON Type | Notes
---|---|---
teamId | 32-bit int |
season | 32-bit int | 
week | 32-bit int | 
stats | object | the keys/fields represent a `MongoWeekStatType`, the value is recorded as a `double`

---

###### ffdb.weekStatsTeam

Field | BSON Type | Notes
---|---|---
teamId | 32-bit int |
season | 32-bit int | 
week | 32-bit int | 
pointsFirstQuarter | 32-bit int |
pointsSecondQuarter | 32-bit int |
pointsThirdQuarter | 32-bit int |
pointsFourthQuarter | 32-bit int |
pointsOverTime | 32-bit int |
pointsTotal | 32-bit int |
firstDowns | 32-bit int |
totalYards | 32-bit int |
passingYards | 32-bit int |
rushingYards | 32-bit int |
penalties | 32-bit int |
penaltyYards | 32-bit int |
turnovers | 32-bit int |
punts | 32-bit int |
puntYards | 32-bit int |
timeOfPossessionSeconds | 32-bit int |

---

###### ffdb.weekMatchup

Field | BSON Type | Notes
---|---|---
season | 32-bit int | 
week | 32-bit int | 
homeTeamId | 32-bit int |
awayTeamId | 32-bit int |
nflGameId | string |
gsisGameId | string |

---

###### ffdb.updateLog

Field | BSON Type | Notes
---|---|---
season | 32-bit int | 
week | 32-bit int | 
dateTime | array | Stores 2 values, the first representing time elapsed from the epoch, and the second being a timezone offset.