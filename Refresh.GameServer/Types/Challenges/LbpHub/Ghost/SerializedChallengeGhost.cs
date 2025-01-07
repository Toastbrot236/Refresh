using System.Text;
using System.Xml.Serialization;
using Bunkum.Core.Storage;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

[XmlRoot("ghost")]
[XmlType("ghost")]
public class SerializedChallengeGhost
{
    /// <summary>
    /// The checkpoints the player has passed during the challenge.
    /// </summary>
    [XmlElement("checkpoint")] public List<SerializedChallengeCheckpoint> Checkpoints { get; set; } = [];

    /// <summary>
    /// The ghost frames tracking the player's coordinates and rotation during the challenge to later
    /// be able to show their movement as a ghost in the game.
    /// </summary>
    [XmlElement("ghost_frame")] public List<SerializedChallengeGhostFrame> Frames { get; set; } = [];

    public static string? GetGhostAssetContent(string? ghostHash, IDataStore dataStore)
    {
        // If there is no hash to begin with, there is no ghost to get
        if (ghostHash == null)
            return null;

        // At this point we already know the hash sent in the SerializedChallengeAttempt refers to an existing ghost asset.
        // If reading from the ghost asset fails, return
        if (!dataStore.TryGetDataFromStore(ghostHash, out byte[]? ghostContentBytes) || ghostContentBytes == null)
            return null;

        // Return the content as a string
        return Encoding.ASCII.GetString(ghostContentBytes);
    }

    public static SerializedChallengeGhost? ToSerializedChallengeGhost(string? ghostContentString)
    {
        if (ghostContentString == null)
            return null;

        // Remove all duplicate "id" XML attributes in "metric" XML elements manually to keep the XmlSerializer happy
        string[] metricSubstrings = ghostContentString.Split("<metric");
        string fixedGhostContentString = metricSubstrings[0];

        // Start iterating from second substring, since the first one is the one before the opening tag for the first occuring metric element
        for(int i = 1; i < metricSubstrings.Length; i++)
        {
            string substring = metricSubstrings[i];
            string[] idSubstrings = substring.Split(" id=");

            // Usually all "id" XML attributes are set to the same value, so just take the value of the last attribute.
            // Also we don't even need the metrics for validation.
            fixedGhostContentString += "<metric id=" + idSubstrings.Last();
        }

        // Try to deserialize the ghost asset
        SerializedChallengeGhost? serializedGhost = null;
        try
        {
            XmlSerializer ghostSerializer = new(typeof(SerializedChallengeGhost));
            if (ghostSerializer.Deserialize(new StringReader(fixedGhostContentString)) is not SerializedChallengeGhost output)
                return null;

            serializedGhost ??= output;
        }
        catch
        {
            return null;
        }

        // Return the serialized ghost, which here can only be either null if deserialization failed or the wanted SerializedChallengeGhost
        return serializedGhost;
    }

    /// <summary>
    /// Does some simple checks on the given SerializedChallengeGhost and returns whether they were successful or not.
    /// </summary>
    /// <remarks>
    /// Unfortunately, there is no way to catch all kinds of corruptions possible by LBP hub, neither is there a reliable way to 
    /// correct corrupt ghost data either, so just try to do some easy checks on cases which are fortunately the more common ones.
    /// </remarks>
    public static bool IsGhostDataValid(SerializedChallengeGhost? challengeGhost, bool isFirstScore, GameChallenge challenge)
    {
        if (challengeGhost == null || challengeGhost.Checkpoints.Count < 1)
            return false;

        // Normally the game already takes care of ordering the checkpoints by time, but just in case
        IEnumerable<SerializedChallengeCheckpoint> checkpoints = challengeGhost.Checkpoints.OrderBy(c => c.Time);

        // The first checkpoint must be the start checkpoint of the challenge and the last checkpoint must be the end checkpoint
        if (checkpoints.First().Uid != challenge.StartCheckpointUid || checkpoints.Last().Uid != challenge.EndCheckpointUid)
            return false;

        // The end checkpoint cant appear more than once in a score which is not the first score, 
        // because the game immediately ends the challenge you are playing when you reach it
        if (!isFirstScore && checkpoints.Count(c => c.Uid == challenge.EndCheckpointUid) > 1)
            return false;

        // Checks successful, ghost is valid
        return true;
    }
}