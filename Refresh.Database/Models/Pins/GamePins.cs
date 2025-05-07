namespace Refresh.Database.Models.Pins;

/// <summary>
/// Stores the progress types of pins which either can or have to be awarded manually by the server,
/// either to award pins for objectives done in games which do not support pins, or to award pins which the
/// game does not award by itself, like website (API) related pins, certain secret pins, and other niche ones.
/// </summary>
public enum GamePins : long
{
    // Multiplayer (Story levels)
    WinXVersusStoryLevelsInOnlineMultiplayer = 748145250,
    WinXVersusStoryLevelsInLocalMultiplayer = 1654785788,
    CompleteXVersusStoryLevelsInAnyMultiplayer = 3999586963,

    // Multiplayer (Community levels)
    WinXVersusCommunityLevelsInOnlineMultiplayer = 1478789882,
    WinXVersusCommunityLevelsInLocalMultiplayer = 2340488392,
    CompleteXVersusCommunityLevelsInAnyMultiplayer = 3224297099,
    WinXCooperativeCommunityLevelsInOnlineMultiplayer = 3918049422,
    WinXCooperativeCommunityLevelsInLocalMultiplayer = 932632090,
    CompleteXCooperativeCommunityLevelsInAnyMultiplayer = 3286069963,

    // Level Leaderboards
    TopFourthOfXStoryLevelsWithOver50Scores = 3394094772,
    TopFourthOfXCommunityLevelsWithOver50Scores = 1700253570, 
    TopXOfAnyStoryLevelWithOver50Scores = 191183438,
    TopXOfAnyCommunityLevelWithOver50Scores = 2033315234,

    // Level Rating
    YayXCommunityLevelsWithUnder10Plays = 2778528358,
    YayXCommunityLevels = 1333342859,

    // Website
    SignIntoWebsite = 2691148325,
    HeartPlayerOnWebsite = 1965011384,
    QueueLevelOnWebsite = 2833810997,
}