using Refresh.GameServer.Types.Pins;

namespace Refresh.GameServer.Extensions;

public static class SerializedUserPinsExtensions
{
    public static Dictionary<long, int> FromSerializedProgressPins(this SerializedUserPins rawPins)
	{
        Dictionary<long, int> progressPins = [];

        for (int i = 0; i < rawPins.ProgressPins.Count; i += 2)
        {
            progressPins.Add(rawPins.ProgressPins[i], (int)rawPins.ProgressPins[i+1]);
            Console.WriteLine($"Combined ID {rawPins.ProgressPins[i]} and progress {rawPins.ProgressPins[i+1]} into one pin");
        }

        return progressPins;
	}

    public static Dictionary<long, int> FromSerializedAwardPins(this SerializedUserPins rawPins)
	{
		Dictionary<long, int> awardPins = [];

        for (int i = 0; i < rawPins.AwardPins.Count; i += 2)
        {
            awardPins.Add(rawPins.AwardPins[i], (int)rawPins.AwardPins[i+1]);
            Console.WriteLine($"Combined ID {rawPins.ProgressPins[i]} and awarded count {rawPins.ProgressPins[i+1]} into one pin");
        }

        return awardPins;
	}
}