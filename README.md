# NFL Database Engine

__Engine__

[![Nuget](https://img.shields.io/nuget/v/r5.ffdb.core.svg)](https://www.nuget.org/packages/R5.FFDB.Core/)

__ Core__

[![Nuget](https://img.shields.io/nuget/v/r5.ffdb.core.svg)](https://www.nuget.org/packages/R5.FFDB.Core/)

---

This program provides a simple CLI to an engine that allows you to easily create your own databases containing the core data needed to run your football and fantasy apps.

The data provided starts with the 2010 season and up until the present.

The two databases natively supported by the CLI are:
- PostgreSql (v10.0.4)
- Mongo (v4.0.2)

If you'd like to use the engine to create a different database, or even postgres/mongo with your own customized schema, you can easily provide your own `IDbProvider` implementation to do so.

---

#### Core Data Types

Below's a list of data categories and stats supported:

- Players - names, physical profile like height and weight, and other misc things like college
- Teams
- Roster information - mappings between player-to-team
- Player Stats - split by season-and-week. Further categorized by type such as passing, rushing, etc.
- Team Stats - also split by season-and-week. Includes things such as points per quarters, passing yards, turnovers, etc.
- Week Matchups - entries for every game indicating the teams playing each other

The Player Stats also includes data for IDP.

---

_The Engine and CLI are currently in an alpha release state_

Although they're essentially feature complete, I have some uncertainties on how I've drawn up the database schemas for both Postgres and Mongo. I'll leave these two issue threads up for a period to get community input:

- (Link to Postgres issue)
- (Link to Mongo issue)

The official v1 release may include db schema changes so be aware that you may need to re-build your db on v1 (migrations won't be supported for the alpha-to-v1 change).

---

#### Documentation Table of Contents

- [Using the CLI](#using-the-cli)
  - [Persisting Data Files](#persisting-data-files)
  - [Configuration File](#config-file)
  - [Commands and Options](#commands-and-options)
    - [initial-setup](#initial-setup)
    - [add-stats](#add-stats)
    - [update-players](#update-players)
    - [update-rosters](#update-rosters)
    - [view-stats](#view-stats)
- [The Engine](#the-engine)
  - [Design Overview](#design-overview)
  - [Engine Setup](#engine-setup)
  - [Engine and Processors API](#engine-and-processors-api)
- [Extending with DbProvider](#extending-with-database-provider)
- [Reporting Bugs and Issues](#reporting-bugs-and-issues)

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

1. An HTTP request is made to the data source. The response is optionally saved to disk.
2. The original source data is mapped to a versioned model, and optionally saved to disk. By _versioned_, I mean that the model is specific to the version of the source. For example, player stats are currently fetched from NFL's fantasy API v2. When they deprecate v2 and move onto v3, we may also need to update our models, resulting in a new _versioned_ model.
3. The versioned model is mapped to the core model used by the Engine.
4. The core models are passed to the configured `DbProvider`, which ultimately maps it to the database specific models (eg SQL or Document) and persists it to the database store.

The middle section labeled _FFDB Engine_ literally represents the stages that are handle by the Engine. Things were designed such that this nice boundary is created, and it's agnostic to the original data sources. It doesn't care where the data is coming from, or what the original format is, as long as it provides the correct mappers that can eventually turn things into the required core engine models.

_A data source dies away, and the Engine breaks. What now?_

Given the explanation above, the Engine itself wouldn't need any modifications. _Someone_ would need to find a new source for this data, and create a new implementation of the `ICoreDataSource` interface, and "that's it".

The reality is that this is a non-trivial task. Because the Engine currently relies on NFL's official player IDs, we would need a complete list of mappings for IDs between the new source and NFL's. This additional source could also be added ahead of time, with slight modifications to the Engine, to provide redundancy but that's a lot of extra work I'm unwilling to commit to at this time.

##### Engine Setup

To programmatically create the Engine, we use the `EngineSetup` class. Here's an example of the simplest valid setup you could use:

```
var setup = new EngineSetup();

setup
  .SetRootDataDirectoryPath(@"C:\path\to\data\dir\")
  .UseMongo(new MongoConfig
  {
    ConnectionString = "connection_string",
    DatabaseName = "db_name"
  });

FfdbEngine engine = setup.Create();
```

This would configure a `FfdbEngine` instance with a data path, using mongo as its data store. It would exclude some other configurable things such as logging.

Below's the complete list of methods available on the `EngineSetup` class.

###### SetRootDataDirectoryPath(string path)
Sets the path to the data directory where the files are optionally persisted.

###### UsePostgreSql(PostgresConfig config)
Sets the Engine to interface with a PostgreSql data store. The `PostgresConfig` class definition is:

```
public class PostgresConfig
{
  public string Host { get; set; }
  public string DatabaseName { get; set; }
  public string Username { get; set; }
  public string Password { get; set; }
}
```

###### UseMongo(MongoConfig config)
Sets the Engine to interface with a Mongo data store. The `MongoConfig` class definition is:

```
public class MongoConfig
{
  public string ConnectionString { get; set; }
  public string DatabaseName { get; set; }
}
```

###### UseCustomDbProvider(Func<IAppLogger, IDatabaseProvider> dbProviderFactory)
Sets the Engine to use a custom database provider that you implement. This is done by providing a factory function, which receives an `IAppLogger` instance that you can use for logging.

###### SkipRosterFetch()
Sets the engine to skip fetching roster information. The reason for doing so was described earlier in the docs.

###### SaveToDisk()
Will save the _versioned_ models to disk.

###### SaveOriginalSourceFiles()
Will save the original source data (HTTP response) to disk. Again, this is probably something you don't need (it takes up almost 300MB of space)

###### EnableFetchingFromDataRepo()
The data repository concept was described earlier. Use this method to enable it. On either the repo being disabled, or failures on fetch, the Engine will simply revert to fetching from the original source.

###### WebRequest.SetThrottle(int milliseconds)
Sets a static delay amount to be used between HTTP requests. Lets try to play nicely with the original sources.

###### WebRequest.SetRandomizedThrottle(int min, int max)
Set a min and max delay amount, also in milliseconds, to be used between HTTP requests.

###### WebRequest.AddHeader(string key, string value)
Add a custom HTTP header to be included for every request.

###### WebRequest.AddDefaultBrowserHeaders()
Adds a `User-Agent` header that will attempt to spoof the HTTP request as being from a browser.

Currently using `"User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36"`

###### Logging.SetLogDirectory(string directoryPath)
Set the directory path where log files will be stored. This is also the only method required to have any logging at all.

###### Logging.SetMaxBytes(long maxBytes)
Set the max bytes before a new log file is created.

###### Logging.SetRollingInterval(RollingInterval interval)
Set an interval (such as days, hours, minutes) between creations of new log files.

###### Logging.RollOnFileSizeLimit()
Will auto create new log files if the max bytes amount is reached.

###### Logging.UseDebugLogLevel()
Set to enable much more detailed logging. You probably don't want to use this unless you're providing logs for a bug/issue.

###### Logging.SetMessageTemplate(string template)
Set the log message template/format. This is somewhat specific to `Serilog`, which is the logging lib the Engine uses.

###### Logging.UseCustomLogger(Microsoft.Extensions.Logging.ILogger logger)
Provide your own `ILogger` instance to be used.

##### Engine and Processors API

The methods available on the Engine are located either on the `FfdbEngine` itself, or as processor class properties on the engine.

_FfdbEngine_

###### Task RunInitialSetupAsync(bool skipAddingStats)
Runs the intial setup including things such as creating database tables, adding stats, etc.

###### Task\<bool> HasBeenInitializedAsync()
Determines whether the database has been initialized (has the _initial setup_ been run successfully?)

###### Task\<WeekInfo> GetLatestWeekAsync()
Gets the latest available week, as officially determined by the NFL.

###### Task<List\<WeekInfo>> GetAllUpdatedWeeksAsync()
Gets the complete list of weeks already updated and existing in the database.

###### Task\<DataRepoState> GetDataRepoStateAsync()
Returns an object representing the current state of the data repository. The `DataRepoState` class is defined as:

```
public class DataRepoState
{
  public DateTime Timestamp { get; set; }
  public bool Enabled { get; set; }
}
```

The `Timestamp` represents when the repo was last updated. If `Enabled` is false, the Engine will not make requests to the data repo.

_StatsProcessor_

Access the following methods using `engine.Stats.MethodName()`

###### Task AddMissingAsync()
Adds all missing stats (those currently not existing in your database)

###### Task AddForWeekAsync(WeekInfo week)
###### Task AddForWeekAsync(int season, int week)
This overloaded method adds all stats for one specified week.

_TeamProcessor_

Access the following methods using `engine.Team.MethodName()`

###### Task UpdateRosterMappingsAsync()
Updates the player-to-team mapping information in the database.

_PlayerProcessor_

Access the following methods using `engine.Player.MethodName()`

###### Task UpdateCurrentlyRosteredAsync()
Updates the dynamic player information for those currently rostered on a team.

---

#### Extending with Database Provider

As mentioned before, you're not limited to using the natively-supported `PostgreSql` or `Mongo` options as your data store. The Engine simply takes in an instance of `IDatabaseProvider` to interface with whatever implementation is out there.

To do this, you'll need a reference to the _R5.FFDB.Core_ library, which can be fetched from nuget:

```
dotnet add package R5.FFDB.Core --version 1.0.0-alpha.1
```

Here, we'll walk through that interface and its contract, so you can understand not only the literal API the Engine expects to work with, but also the underlying behavior and assumptions that are relevant.

Here's the `IDatabaseProvider` interface definition:

```
public interface IDatabaseProvider
{
  IDatabaseContext GetContext();
}
```

Ah! So it's really not this interface that defines all the necessary functionality for the Engine to work. This has one single method, that returns an `IDatabaseContext`. Your `IDatabaseProvider` implementation will most likely take in configuration information to connect to the db, setup logging, etc. You can always reference how the built-in db providers were implemented as needed.

Lets explore this `IDatabaseContext` interface:

```
public interface IDatabaseContext
{
  Task InitializeAsync();
  Task<bool> HasBeenInitializedAsync();

  IPlayerDbContext Player { get; }
  IPlayerStatsDbContext PlayerStats { get; }
  ITeamDbContext Team { get; }
  ITeamStatsDbContext TeamStats { get; }
  IUpdateLogDbContext UpdateLog { get; }
  IWeekMatchupsDbContext WeekMatchups { get; }
}
```

###### Task InitializeAsync()
This should setup the database tables/collections, schemas, and whatever else you would classify as required initial work. Additionally, this should also add entries for all the NFL teams.

_Important design note:_

The database context methods, in general, should be implemented to simply try to add/create whatever is passed in as arguments. For example, if a method accepts a list of stats, it _should_ attempt to add all of them. It doesn't need to concern itself with whether or not some of the stats have _already_ been added or not. That logic is handled by the Engine, and makes it easier for you to implement your own database providers.

However, the initialize method, is the one exception. Because the intial setup tasks are entirely specific to a given database, it's up to you to make sure that it can be re-run many times without exceptions or undesired results. For example, if 5 of 10 tables had already been created before the program failed, re-running it should only attempt to create the remaining 5.

###### Task\<bool> HasBeenInitializedAsync()
This should return a `bool` indicating whether the database has been initialized. The Engine will use this to block certain commands until the setup has been complete.

The _IPlayerDbContext_ interfaces defines these methods:

###### Task<List\<Player>> GetAllAsync()
Returns a list of all players that currently exist in your database.

###### Task AddAsync(PlayerAdd player);
Take the argument that contains player information and adds it to your database.

###### Task UpdateAsync(Guid id, PlayerUpdate update);
Update the player given the information contained in the `PlayerUpdate` instance.

The _IPlayerStatsDbContext_ interfaces defines these methods:

###### Task<List\<string>> GetPlayerNflIdsAsync(WeekInfo week);
Return a list of NFL IDs for _all_ players that have played for a given week. If you store player stats in a single table/collection, you can simply return the player's NFL IDs if you store it in those entries. If the stats are spread out within multiple tables, you may need joins to extract this information.

###### Task AddAsync(List\<PlayerWeekStats> stats);
Take the list of player stats information and add it to your database.

The _ITeamDbContext_ interfaces defines these methods:

###### Task<List\<int>> GetExistingTeamIdsAsync();
Get the list of Team IDs for those currently existing in your database. If all have been added, this should always return a list of 32 ids.

###### Task AddAsync(List\<Team> teams);
Add the list of teams to your database.

###### Task UpdateRosterMappingsAsync(List\<Roster> rosters);
Take the list of roster information and update the player-to-team mappings in your database. 

The _ITeamStatsDbContext_ interfaces defines these methods:

###### Task<List\<TeamWeekStats>> GetAsync(WeekInfo week);
Return the list of team stats for a given week.

###### Task AddAsync(List\<TeamWeekStats> stats);
Take the list of team stats and add it to your database.

The _IUpdateLogDbContext_ interfaces defines these methods:

###### Task<List\<WeekInfo>> GetAsync();
Get a list of all the weeks that have already been updated for your database.

###### Task AddAsync(WeekInfo week);
Update your database to indicate that the given week has been complete updated. How you store this information is irrelevant to the Engine as it's an implementation detail.

###### Task\<bool> HasUpdatedWeekAsync(WeekInfo week);
Returns a `bool` indicating whether a given week has been updated in your database.

The _IWeekMatchupsDbContext_ interfaces defines these methods:

###### Task<List\<WeekMatchup>> GetAsync(WeekInfo week);
Returns the list of weekly matchups (between teams) for a given week.

###### Task AddAsync(List\<WeekMatchup> matchups);
Take the list of matchups and add it to your database.

---

#### Reporting Bugs and Issues

Stumbled across a bug? Program throwing some exception? Please enable debug level logging and provide the log file or relevant snippets.

It's an easy request to meet, and I'll most likely ignore/close the issue without it. Thanks!