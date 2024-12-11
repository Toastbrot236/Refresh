namespace Refresh.GameServer.Types.Challenges.LbpHub;

public enum GameChallengeType : byte
{
    Time = 0,
    Score = 1,
    Multiplier = 2,
    Lives = 3,
    Prizes = 4,
}

public static class GameChallengeTypeExtensions
{
    public static byte ToSerializedType(this GameChallengeType type)
        => type switch
        {
            GameChallengeType.Time => 0,
            GameChallengeType.Score => 1,
            GameChallengeType.Multiplier => 2,
            GameChallengeType.Lives => 3,
            GameChallengeType.Prizes => 4,
            _ => throw new ArgumentOutOfRangeException(),
        };
}