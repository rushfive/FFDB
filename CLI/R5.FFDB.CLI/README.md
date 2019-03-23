# NFL Database Engine

This program provides a simple CLI to an engine that allows you to easily create your own databases containing the core data needed to run your football and fantasy apps.

The two databases natively supported by the CLI are:
- PostgreSql (v10.0.4)
- Mongo (v4.0.2)

If you'd like to use the engine to create a different database, or even postgres/mongo with your own customized schema, you can easily provide your own `IDbProvider` implementation to do so.

---

#### Core Data Types

Below's a list of data categories and stats supported:

- Players - names, physical profile like height and weight, and other misc college
- Teams
- Roster information - mappings between player-to-team
- Player Stats - split by season-and-week. Further categorized by type such as passing, rushing, etc.
- Team Stats - also split by season-and-week. Includes things such as points per quarters, passing yards, turnovers, etc.
- Week Matchups - entries for every game indicating the teams playing each other

The Player Stats also includes data for IDP.

---

#### A couple important notes:

_The Engine and CLI are currently in an alpha release state_

Although they're essentially feature complete, I have some uncertainties on how I've drawn up the database schemas for both Postgres and Mongo. I'll leave these two issue threads up for a period to get community input:

- (Link to Postgres issue)
- (Link to Mongo issue)

The official v1 release may include db schema changes so be aware that you may need to re-build your db on v1 (migrations won't be supported for the alpha-to-v1 change).

_Receiving targets are not included_

This is a stat I really wanted to include, and had spent a good amount of time trying to get working. NFL's site does include this information, but as it's loaded dynamically using JS, it turned to be too time-consuming and I didn't want to hold off on building the rest of the engine out.

I'm most likely _not_ going to be adding this in as a feature. After checking out a lot of football and fantasy related sites, it turns out most of them don't even include target numbers. For purposes of Fantasy Football, which is what I've built this for, receptions count is clearly the more critical stat, and is included.

Anyone is more than welcome to try adding the _targets_ stat within the alpha-release period - it would undoubtedly be appreciated by everyone. However, once the engine gets into a v1 state, it will become more difficult as db migrations aren't currently built in.

---

#### Documentation Table of Contents

- [Using the CLI](#using-the-cli)
  - [important concepts overview](#important-concepts-overview)
  - [configuration file](#config-file)
  - [commands and options](#commands-and-options)
- [The Engine](#the-engine)
  - [design overview](#design-overview)
  - [engine setup](#engine-setup)
- [Extending with DbProvider](#extending-with-dbprovider)

---

#### Using the CLI

To get started, you'll want to setup your config file