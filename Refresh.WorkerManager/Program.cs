﻿using Bunkum.Core;
using Bunkum.Core.Storage;
using NotEnoughLogs;
using NotEnoughLogs.Behaviour;
using Refresh.Core.Configuration;
using Refresh.Database;
using Refresh.Database.Configuration;
using Refresh.Interfaces.Workers;
using Refresh.Workers;

LoggerConfiguration loggerConfiguration = new()
{
    Behaviour = new QueueLoggingBehaviour(),
#if DEBUG
    MaxLevel = LogLevel.Trace,
#else
    MaxLevel = LogLevel.Info,
#endif
};

using Logger logger = new(loggerConfiguration);

logger.LogInfo(BunkumCategory.Startup, "Starting up worker manager...");
logger.LogCritical(BunkumCategory.Startup, "The dedicated worker manager isn't complete yet! It will work in a debug setting, but do not use this in production yet.");

logger.LogInfo(BunkumCategory.Startup, "Initializing configs...");
ConfigStore configStore = new(logger);

logger.LogInfo(BunkumCategory.Startup, "Initializing database...");
using GameDatabaseProvider database = new(logger, configStore.Database);
database.Initialize();
logger.LogInfo(BunkumCategory.Startup, "Warming up database...");
database.Warmup();

logger.LogInfo(BunkumCategory.Startup, "Starting worker manager!");
WorkerManager manager = RefreshWorkerManager.Create(logger, new FileSystemDataStore(), database);

manager.Start();
manager.WaitForExit();