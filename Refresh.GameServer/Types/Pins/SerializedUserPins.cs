namespace Refresh.GameServer.Types.Pins; 

#nullable disable

public partial class SerializedUserPins
{
	/// <summary>
	/// Pins which require their objective to be done multiple times in order to get achieved.
	/// Sent in the following pattern: progressType ID, progress, progressType ID, progress etc.
	/// In LBP2, the progress and award value of pins (for those which are listed in both) is equal.
	/// </summary>
	[JsonProperty(PropertyName = "progress")] public IList<long> ProgressPins { get; }

	/// <summary>
	/// Pins which can be achieved multiple times.
	/// Sent in the following pattern: progressType ID, achieved count, progressType ID, achieved count etc.
	/// In LBP2, the progress and award value of pins (for those which are listed in both) is equal.
	/// </summary>
	[JsonProperty(PropertyName = "awards")] public IList<long> AwardPins { get; }

	/// <summary>
	/// Only the IDs of Pins which should show up on the user's profile.
	/// </summary>
	[JsonProperty(PropertyName = "profile_pins")] public IList<long> ProfilePins { get; }
}
