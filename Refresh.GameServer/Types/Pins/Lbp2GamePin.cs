namespace Refresh.GameServer.Types.Pins;

/// <summary>
/// assigns a readable, generic name describing a pin to a "progressType"
/// (what the game uses to identify pins).
/// </summary>
/// <remarks>
/// Partially researched using the UpdateMyPins endpoint, partially taken from
/// https://github.com/LittleBigRefresh/Docs/blob/main/Docs/pin-files/lbp3.json
/// </remarks>
public enum Lbp2GamePin : long
{
    // LBP2
    // Play - Story Mode (Category 0)
    PlayStoryLevelsDurationMinutes = 3253443510,
    AceUniqueStoryLevels = 3336712259,
    AceStoryLevelsInARow = 2567681875,

    CompleteDaVinki = 4096677602,
    AceDaVinki = 4098748268,
    CompleteLaboratory = 4127655781,
    AceLaboratory = 1519296437,
    CompleteFactory = 3994547166,
    AceFactory = 1125604704,
    CompleteAvalonia = 2280741953,
    AceAvalonia = 3604147529,
    CompleteAsylum = 2997692249,
    AceAsylum = 3002270865,
    CompleteCosmos = 3372476496,
    AceCosmos = 1880165174,
    CompleteStoryMode = 699011411,
    AceStory = 2807907583,

    CollectScoreBubblesInStory = 983075891,
    CollectPrizesInStory = 501958375,
    TopXPercentOfScoresAgainst50PlayersInOneStoryLevel = 191183438,
    Top25PercentOfScoresAgainst50PlayersInStoryLevels = 3394094772,
    
    OnlineMultiplayerStoryCoopLevelWins = 2529770882,
    LocalMultiplayerStoryCoopLevelWins = 273149064,
    MultiplayerStoryCoopGameCompletions = 169261587,
    OnlineMultiplayerStoryVersusLevelWins = 748145250,
    LocalMultiplayerStoryVersusLevelWins = 1654785788,
    MultiplayerStoryVersusGameCompletionss = 3999586963,

    DefeatCreaturesInStory = 1328752708,
    StoryTouchBouncePadsWithoutLanding = 3679887473,
    StoryGrappleObjectsWithoutLanding = 3500753689,
    StoryHaveSackbotsFollow = 356008882,
    StoryThrowSackbotsIntoHazards = 840691180,
    DieInStoryLevel = 3231471803,
    DieXTimesInOneStoryLevel = 451708578,
    AchieveCertainMultiplier = 56389911,

    // Play - General (Category 1)
    PlayCommunityLevelsDurationMinutes = 510577794,
    CompleteUniqueCommunityLevels = 1109950816,
    AceUniqueCommunityLevels = 3631168967,
    CompleteUniqueCoopCommunityLevels = 1524408084,
    CompleteUniqueVersusCommunityLevels = 2249488698,
    CompleteNewCommunityLevels = 3816103083,
    CompleteCommunityLevelsWithGrapplingHook = 3114588174,
    CompleteCommunityLevelsWithGrabinator = 386780997,
    CompleteCommunityLevelsWithCreatinator = 3085864917,
    PlayCommunityLevelsWithControllinator = 577722452,
    DieInCommunityLevels = 1600196991,
    DefeatCreaturesInCommunity = 3551108040,

    CollectScoreBubblesInCommunity = 3779931447,
    CollectPrizesInCommunity = 1904209067,

    TopXPercentOfScoresAgainst50PlayersInOneCommunityLevel = 2033315234,
    Top25PercentOfScoresAgainst50PlayersInCommunityLevels = 1700253570,

    OnlineMultiplayerCommunityCoopLevelWins = 3918049422,
    LocalMultiplayerCommunityCoopLevelWins = 932632090,
    MultiplayerCommunityCoopGameCompletions = 3286069963,
    OnlineMultiplayerCommunityVersusLevelWins = 1478789882,
    LocalMultiplayerCommunityVersusLevelWins = 2340488392,
    MultiplayerCommunityVersusGameCompletions = 3224297099,

    DiveInToRoomsInLevels = 2679507356,
    DiveInMultiplayerVersusLevelWins = 2964403462,
    DiveInMultiplayerCoopLevelWins = 257360986,

    UseLevelLinks = 2506662226,
    PlayLbp2EveryWeekDay = 2517130892,
    PlayLbp2Early = 1402841543,
    PlayLbp2Late = 3549070251,
    PlayLbp2OnChristmas = 2623945106,
    PlayTeamPickLevel = 2051925987,
    PickupGrapplingHook = 928959824,
    PickupGrabinator = 7235671,
    PickupCreatinator = 884941295,
    FireBounce = 1958003294,
    CompleteRaces = 1166509103,
    ActivateStickerSensors = 4006802074,
    Drown = 1441043796,
    HoldToRetry = 2826866402,
    LaunchSackbotOnBouncePad = 3326779194,
    AchieveMultipliersTotal = 548980327,
    
    

    WinRacesInMultiplayer = 4079912684,
    LoseOneRaceInMultiplayer = 4271962802,
    ChainOfHangingPlayers = 1157565082,
    GrappleAnotherPlayer = 3081538789,
    GrapplingHookChainOf4Players = 2860588161,
    SlapPlayer = 3847470522,
    SlapMultiplePlayersAtOnce = 753231045,
    ThrowAnotherPlayerWithGrabinator = 1858489148,
    ExplodeAnotherPlayer = 2029671630,
    DragAnotherPlayerIntoHazard = 3826013917,
    SlapAnotherPlayerIntoHazard = 3826190917,
    TakeGroupPhoto = 1990595250,

    // Create (Category 2)
    PlaceSticker = 2644647509,
    CreateCompleteCostume = 2259179872,
    CreateModeDurationMinutes = 1928306284,
    FillThermometer = 4014155199,
    TooManyCornersOnObject = 3854823973,
    PlaceControllinatorInCreateMode = 1701977522,
    

    TotalPlaysOnOnePublishedLevel = 2726743844,
    TotalPlaysOnAllPublishedLevels = 1461886302,
    UniquePlaysOnOnePublishedLevel = 2820366538,
    UniquePlaysOnAllPublishedLevels = 3577962928,
    HeartsOnOnePublishedLevel = 3855761662,
    HeartsOnAllPublishedLevels = 1235998656,
    YaysOnOnePublishedLevel = 2086540124,
    YaysOnAllPublishedLevels = 2350171014,
    PositiveRatingAfter100OnOnePublishedLevel = 3142374916,
    PositiveRatingAfter100OnAllPublishedLevels = 975425892,
}