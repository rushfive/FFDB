## PostgreSql Schema

This document describes the schema for the current version _v1.0.0-alpha.1_

##### Quick Notes
- All player week stats use a `FLOAT8` datatype. I understand that this doesn't make sense for a lot of stat types, but when I was building this out initially I kept finding new stats that were of a double-precision type and running into exceptions. Keeping them all as `FLOAT8` simplifies the processing work the Engine has to do, and future proofs any additional stat types that may come out.
- Pretty much every table _doesn't_ have a single-column PK. For example, the various player stats tables make use of composite primary keys instead (eg combining player_id, season, and week which together uniquely identifies the entry)

There's several columns that serialize core enum values as strings. The definitions of these enums can be found in the `R5.FFDB.Core` project:

- [R5.FFDB.Core Documentation](../../Engine/R5.FFDB.Core/README.md)

---

### Tables

###### ffdb.player

Column | DataType | PK | FK | NotNull
---|---|---|---|---
id | UUID | yes | |
nfl_id | TEXT | | |
esb_id | TEXT | | |
gsis_id | TEXT | | |
first_name | TEXT | | | true
last_name | TEXT | | | 
position | TEXT | | | 
number | INT | | | 
status | TEXT | | | 
height | INT | | | 
weight | INT | | | 
date_of_birth | DATE | | | 
college | TEXT | | |

###### ffdb.team

Column | DataType | PK | FK | NotNull
---|---|---|---|---
id | INT | yes | |
nfl_id | TEXT | | | true
name | TEXT | | |
abbreviation | TEXT | | |

###### ffdb.player_team_map

Column | DataType | PK | FK | NotNull
---|---|---|---|---
player_id | UUID | composite | ffdb.player_id | true
team_id | INT | composite | ffdb.team_id | true

###### ffdb.week_stats_pass

Column | DataType | PK | FK | NotNull
---|---|---|---|---
player_id | UUID | composite | ffdb.player_id | true
team_id | INT | | ffdb.team_id | 
season | INT | composite | | true
week | INT | composite | | true
attempts | FLOAT8 | | | 
completions | FLOAT8 | | | 
yards | FLOAT8 | | | 
touchdowns | FLOAT8 | | | 
interceptions | FLOAT8 | | | 
sacked | FLOAT8 | | | 

###### ffdb.week_stats_rush

Column | DataType | PK | FK | NotNull
---|---|---|---|---
player_id | UUID | composite | ffdb.player_id | true
team_id | INT | | ffdb.team_id | 
season | INT | composite | | true
week | INT | composite | | true
attempts | FLOAT8 | | | 
yards | FLOAT8 | | | 
touchdowns | FLOAT8 | | | 

###### ffdb.week_stats_receive

Column | DataType | PK | FK | NotNull
---|---|---|---|---
player_id | UUID | composite | ffdb.player_id | true
team_id | INT | | ffdb.team_id | 
season | INT | composite | | true
week | INT | composite | |  true
catches | FLOAT8 | | | 
yards | FLOAT8 | | | 
touchdowns | FLOAT8 | | | 

###### ffdb.week_stats_return

Column | DataType | PK | FK | NotNull
---|---|---|---|---
player_id | UUID | composite | ffdb.player_id | true
team_id | INT | | ffdb.team_id | 
season | INT | composite | | true
week | INT | composite | |  true
yards | FLOAT8 | | | 
touchdowns | FLOAT8 | | | 

###### ffdb.week_stats_misc

Column | DataType | PK | FK | NotNull
---|---|---|---|---
player_id | UUID | composite | ffdb.player_id | true
team_id | INT | | ffdb.team_id | 
season | INT | composite | | true
week | INT | composite | |  true
fumble_recover_touchdowns | FLOAT8 | | | 
fumbles_lost | FLOAT8 | | | 
fumbles_total | FLOAT8 | | | 
two_point_conversions | FLOAT8 | | | 

###### ffdb.week_stats_kick

Column | DataType | PK | FK | NotNull
---|---|---|---|---
player_id | UUID | composite | ffdb.player_id | true
team_id | INT | | ffdb.team_id | 
season | INT | composite | | true
week | INT | composite | |  true
pat_makes | FLOAT8 | | | 
pat_misses | FLOAT8 | | | 
zero_twenty_makes | FLOAT8 | | | 
twenty_thirty_makes | FLOAT8 | | | 
thirty_forty_makes | FLOAT8 | | | 
forty_fifty__makes | FLOAT8 | | | 
fifty_plus_makes | FLOAT8 | | | 
zero_twenty_misses | FLOAT8 | | | 
twenty_thirty_misses | FLOAT8 | | | 
thirty_forty_misses | FLOAT8 | | | 
forty_fifty_misses | FLOAT8 | | | 
fifty_plus_misses | FLOAT8 | | | 

###### ffdb.week_stats_dst

Column | DataType | PK | FK | NotNull
---|---|---|---|---
player_id | UUID | composite | ffdb.player_id | true
team_id | INT | | ffdb.team_id | 
season | INT | composite | | true
week | INT | composite | |  true
sacks | FLOAT8 | | | 
interceptions | FLOAT8 | | | 
fumbles_recovered | FLOAT8 | | | 
fumbles_forced | FLOAT8 | | | 
safeties | FLOAT8 | | | 
touchdowns | FLOAT8 | | | 
blocked_kicks | FLOAT8 | | | 
return_yards | FLOAT8 | | | 
return_touchdowns | FLOAT8 | | | 
points_allowed | FLOAT8 | | | 
yards_allowed | FLOAT8 | | | 

###### ffdb.week_stats_idp

Column | DataType | PK | FK | NotNull
---|---|---|---|---
player_id | UUID | composite | ffdb.player_id | true
team_id | INT | | ffdb.team_id | 
season | INT | composite | | true
week | INT | composite | |  true
tackles | FLOAT8 | | | 
assisted_tackles | FLOAT8 | | | 
sacks | FLOAT8 | | | 
interceptions | FLOAT8 | | | 
forced_fumbles | FLOAT8 | | | 
fumbles_recovered | FLOAT8 | | | 
interception_touchdowns | FLOAT8 | | | 
fumble_touchdowns | FLOAT8 | | | 
blocked_kick_touchdowns | FLOAT8 | | | 
blocked_kicks | FLOAT8 | | | 
safeties | FLOAT8 | | | 
passes_defended | FLOAT8 | | | 
interception_return_yards | FLOAT8 | | | 
fumble_return_yards | FLOAT8 | | | 
tackles_for_loss | FLOAT8 | | | 
quarterback_hits | FLOAT8 | | | 
sack_yards | FLOAT8 | | | 

###### ffdb.team_game_stats

Column | DataType | PK | FK | NotNull
---|---|---|---|---
team_id | INT | composite | ffdb.team_id | true
season | INT | composite | | true
week | INT | composite | |  true
points_first_quarter | INT | | |  true
points_second_quarter | INT | | |  true
points_third_quarter | INT | | |  true
points_fourth_quarter | INT | | |  true
points_overtime | INT | | |  true
points_total | INT | | |  true
first_downs | INT | | |  true
total_yards | INT | | |  true
passing_yards | INT | | |  true
rushing_yards | INT | | |  true
penalties | INT | | |  true
penalty_yards | INT | | |  true
turnovers | INT | | |  true
punts | INT | | |  true
punt_yards | INT | | |  true
time_of_posession | INT | | |  true

###### ffdb.week_game_matchup

Column | DataType | PK | FK | NotNull
---|---|---|---|---
season | INT | composite | | true
week | INT | composite | |  true
home_team_id | INT | composite | ffdb.team_id | true
away_team_id | INT | composite | ffdb.team_id | true
nfl_game_id | TEXT | | |  true
gsis_game_id | TEXT | | |  true

###### ffdb.update_log

Column | DataType | PK | FK | NotNull
---|---|---|---|---
season | INT | composite | | true
week | INT | composite | |  true
datetime | TIMESTAMPTZ | | | true

