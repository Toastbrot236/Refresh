namespace Refresh.Database.Models.Workers.Jobs;

/// <summary>
/// Used by the GameServer to give the cwlib-worker a list of adventure root resources to analyze and
/// get the remaining inner level data from,
/// while also informing the worker on which of the inner levels is considered "modded". 
/// The worker can then simply just set the corresponding GameInnerLevel's modded status using this.
/// </summary>
public class CompleteAdventureDataJobState
{
	[JsonProperty("AdventureRootHashes")] public List<string> AdventureRootHashes { get; set; } = [];

    // inner level root resource hash -> whether that resource or its dependencies are modded
	[JsonProperty("LevelModdedRelations")] public Dictionary<string, bool> LevelModdedRelations { get; set; } = [];
}
