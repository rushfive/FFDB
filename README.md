# NFL Database Engine

This program provides a simple CLI to an engine that allows you to easily create your own databases containing the core data needed to run your football and fantasy apps.

The data provided starts with the 2010 season and up until the present.

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

_The Engine is tighly-coupled with NFL's data sources and schemes_

100% of the data is sourced from either APIs hosted by the NFL, or scraped from some site. The data is pieced together using _IDs_ specifically used by the NFL (such as GSIS and ESB).

What does this mean? If the data sources are either shutdown or changed significantly, the Engine will stop working until either the codes updated or, in the worst case, a new data source is found to replace it.

If you're curious, you can read more about the Engine design further down to see how this might be accomplished.

---

#### Documentation Table of Contents

- [Using the CLI](#using-the-cli)
  - [persisting data files](#persisting-data-files)
  - [configuration file](#config-file)
  - [commands and options](#commands-and-options)
    - [initial-setup](#initial-setup)
    - [add-stats](#add-stats)
    - [update-players](#update-players)
    - [update-rosters](#update-rosters)
    - [view-stats](#view-stats)
- [The Engine](#the-engine)
  - [design overview](#design-overview)
  - [engine setup](#engine-setup)
  - [processors API](#processors-api)
- [Extending with DbProvider](#extending-with-dbprovider)
  - [logging](#logging)
  - [implementing the contract](#implementing-the-contract)




---

#### Using the CLI

To get started, head to the release page and download the latest version:

- (link to release page)

Or, clone the repo and run it yourself (the app was built using netcore2.2)

##### Persisting Data Files

Just a few notes about data files before diving in:

- Data fetched from the various sources can optionally be persisted to disk. This allows you to re-create databases faster by not making the same HTTP requests. For context, as of this writing, theres almost 3800 player records, each of which require a separate request to resolve. Yeah.. it's a lot.
- There's an additional option to fetch from a data repository I'm hosting on the side that includes the files needed. You'll still need to make HTTP requests to grab these, but it guarantees that if the sources go down, we'll at least have these records going forward.
- There's also _another_ additional option (har har) to persist the original source files. When the Engine receives the original data, it will first map it to a versioned format that's eventually used. You most likely won't need these original files, as they aren't necessary to rebuild databases from files (only the versioned ones are required).
- But wait, there's more! There's a _final additional_ option that..... just kidding. That is all.

##### Configuration File

There's a configuration file required to use the CLI. Below's the template:

```
{
  "RootDataPath": "",
  "WebRequest": {
    "ThrottleMilliseconds": 1000,
    "RandomizedThrottle": {
      "Min": 1000,
      "Max": 3000
    }
  },
  "Logging": {
    "Directory": "",
    "MaxBytes": null,
    "RollingInterval": "Day",
    "RollOnFileSizeLimit": false,
    "UseDebugLogLevel": false
  },
  "PostgreSql": {
    "Host": "",
    "DatabaseName": "",
    "Username": "",
    "Password": ""
  },
  "Mongo": {
    "ConnectionString": "",
    "DatabaseName": ""
  }
}
```

The only required sections are the`RootDataPath` and either the `PostgreSql` or `Mongo` configurations.

Make sure to set the other database section (that you're not using) to `null` or the Engine won't run.

The `WebRequest` section allows you to specify the throttling between HTTP requests (we should play nicely). You can either set the `ThrottleMilliseconds` to use the same delay for every request (requires you to set `RandomizedThrottle` to null), or you can define a min and max to use a randomized delay.

Logging is also optional, but highly-recommended. Set the section to `null` if you don't want it. Else, the only requirement is to set the `Directory` path. Logging configuration has been simplified into essentially `Information` and `Debug` levels. The Engine defaults to `Information`, and you can set `UseDebugLogLevel` to true if you want more details. Using the `Debug` level outputs a ton of things - you'll probably want to just use the default, unless you need to submit logs for an issue.

_Where should the config file be placed?_

By default, the CLI program will try to look in the same directory as the binary itself. If it's not going to be located there, you can always specify the path as an option using `--config=path\to\config.json`

##### Commands and Options

###### Initial Setup

Initializes the database tables/collections, adds the team entries (static), and adds all missing stats up to the current date and what's available.

This command allows you to create a database with all available data in one-go (as long as you don't include the `skip-stats` option).

Usage: `ffdb initial-setup`

Options:

- `skip-stats` - will skip adding all missing stats after running the initial database setup. Usage: `ffdb initial-setup --skip-stats`

###### Add Stats

Adds player stats, team stats, and matchups information for either one specified week, or for all missing.

Usage: `ffdb add-stats week 2018-1` or `ffdb add-stats missing`

Options:

- `save-to-disk` - save the versioned files to disk. This is what the Engine needs to create the database.
Usage: `ffdb initial-setup --skip-stats`
- `save-src-files` - save the original source response as a file. In most cases, this is the JSON or XML response from the request.
Usage: `ffdb initial-setup --save-src-files`

###### Update Players

Updates dynamic information for players currently rostered on a team. These include their:

- number
- position
- roster status (eg Active, Injured-reserve, etc.)

Usage: `ffdb update-players`

###### Update Rosters

Fetches the current roster information for every team, and updates the player-to-team mappings in the database.

Usage: `ffdb update-rosters`

###### View State

Displays some general state information such as the weeks already updated, the latest available NFL week, etc.

Usage: `ffdb view-state`

###### Global Options

There are 2 options that can be used with any of the commands above.

- `config | c` - sets the file path to the config file. The recommended approach to avoid having to use this is to simply have the file exist in the same directory as the CLI program binary. Usage: `ffdb <command> --config=path\to\config.json`
- `skip-roster | s` - will skip fetching the latest roster data for a command. Imagine you want to run 2 commands, one after another: adding stats for 2018-1, then adding stats for 2018-2 right after. Each of these commands requires the current roster information, but it doesn't make sense to fetch it twice within a few minutes of each other - the roster information is highly unlikely to have changed. Usage: `ffdb <command> --skip-roster`

---

#### The Engine

The engine is what does all the real work behind the scenes - the CLI is just an interface to it. Given that, the Engine is also released separately as its own package for those that would like to interact with it programmatically:

- (link to its nuget)

##### Design Overview

![alt text](/Documentation/EngineDataFlow.png)

The diagram above depicts how the various data is fetched. Here's a quick rundown:

1. A HTTP request is made to the data source. It's optionally saved to disk.
2. The response is mapped to a versioned model, and optionally saved to disk. By _versioned_ means that the model is specific to the version of the source. For example, player stats are currently fetched from NFL's fantasy API v2. When they deprecate v2 and move onto v3, we may also need to update our models, resulting in a new versioned model.
3. The versioned model is mapped to the core model used by the Engine.
4. The core models are passed to the configured `DbProvider`, which ultimately maps it to the database specific models (eg SQL or Document) and persists it to the database store.




---

#### Reporting Bugs and Issues

Stumbled across a bug? Program throwing some exception? Please enable debug level logging and provide the log file or relevant snippets.

It's an easy request to meet, and I'll most likely ignore/close the issue without it. Thanks!