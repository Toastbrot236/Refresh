namespace Refresh.GameServer.Types.Pins;

/// <summary>
/// Assigns a readable, generic name describing a pin to a "progressType"
/// (what the game uses to identify pins).
/// Only contains LBP2 pins which are relevant to the server (mostly online-related).
/// </summary>
/// <remarks>
/// Partially researched using the UpdateMyPins endpoint, mostly taken from
/// https://github.com/LittleBigRefresh/Docs/blob/main/Docs/pin-files/lbp3.json
/// </remarks>
public enum Lbp2GamePin : long
{
    // LBP2
    // Play - Story Mode (Category 0)
    ActiveXMinutesInStoryLevels = 3253443510,
    
    /*
    AceXUniqueStoryLevels = 3336712259,
    AceXStoryLevelsInARow = 2567681875,

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

    CollectXScoreBubblesInStory = 983075891,
    CollectPrizesInStoryPercentage = 501958375,
    */

    TopXPercentOfScoresAgainst50PlayersInOneStoryLevel = 191183438,
    Top25PercentOfScoresAgainst50PlayersInXStoryLevels = 3394094772,
    
    StoryOnlineMultiplayerXCoopLevelWins = 2529770882,
    StoryLocalMultiplayerXCoopLevelWins = 273149064,
    StoryXCoopLevelsCompletedInMultiplayer = 169261587,
    StoryOnlineMultiplayerXVersusLevelWins = 748145250,
    StoryLocalMultiplayerXVersusLevelWins = 1654785788,
    StoryXVersusLevelsCompletedInMultiplayer = 3999586963,

    /*
    DefeatXCreaturesInStory = 1328752708,
    StoryTouchXBouncePadsWithoutLanding = 3679887473,
    StoryXGrappleObjectsWithoutLanding = 3500753689,
    StoryHaveXSackbotsFollow = 356008882,
    StoryThrowXSackbotsIntoHazards = 840691180,
    DieInStoryLevel = 3231471803,
    DieXTimesInOneStoryLevel = 451708578,
    AchieveCertainMultiplier = 56389911,
    */

    // Play - General (Category 1)
    ActiveXMinutesInCommunityLevels = 510577794,
    CompleteXUniqueCommunityLevels = 1109950816,
    AceXUniqueCommunityLevels = 3631168967,
    CompleteXUniqueCoopCommunityLevels = 1524408084,
    CompleteXUniqueVersusCommunityLevels = 2249488698,
    CompleteXNewCommunityLevels = 3816103083,
    PlayTeamPickLevel = 2051925987,
    /*
    CompleteXCommunityLevelsWithGrapplingHook = 3114588174,
    CompleteXCommunityLevelsWithGrabinator = 386780997,
    CompleteXCommunityLevelsWithCreatinator = 3085864917,
    PlayXCommunityLevelsWithControllinator = 577722452,
    
    DieInCommunityLevels = 1600196991,
    DefeatXCreaturesInCommunity = 3551108040,
    */

    CollectXScoreBubblesInCommunity = 3779931447,
    CollectXPrizeBubblesInCommunity = 1904209067,

    TopXPercentOfScoresAgainst50PlayersInOneCommunityLevel = 2033315234,
    Top25PercentOfScoresAgainst50PlayersInCommunityLevels = 1700253570,

    CommunityOnlineMultiplayerXCoopLevelWins = 3918049422,
    CommunityLocalMultiplayerXCoopLevelWins = 932632090,
    CommunityXCoopLevelsCompletedInMultiplayer = 3286069963,
    CommunityOnlineMultiplayerXVersusLevelWins = 1478789882,
    CommunityLocalMultiplayerXVersusLevelWins = 2340488392,
    CommunityXVersusLevelsCompletedInMultiplayer = 3224297099,

    DiveInToRoomsInXLevels = 2679507356,
    DiveInMultiplayerXVersusLevelWins = 2964403462,
    DiveInMultiplayerXCoopLevelWins = 257360986,

    /*
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

    UseLevelLinks = 2506662226,
    PlayLbp2EveryWeekDay = 2517130892,
    PlayLbp2Early = 1402841543,
    PlayLbp2Late = 3549070251,
    PlayLbp2OnChristmas = 2623945106,
    
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
    */

    // Create (Category 2)
    ActiveXMinutesInCreateMode = 1928306284,

    /*
    PlaceSticker = 2644647509,
    PlaceStickerOnAnotherPlayer = 1491422953,
    PlaceControllinator = 1701977522,
    PlaceGrapplingHook = 441559590,
    PlaceGrabinator = 1714656567,
    PlaceCreatinator = 4135933263,
    PlaceEmitter = 938248997,
    PlaceGlobalLightingTweaker = 414013175,
    PlaceXLogicPiecesInCircuitboard = 1436394297,
    PlaceWater = 259701840,
    PlaceSackbot = 2023392314,
    ChangeSackbotBehaviour = 4285588472,
    RecordSackbotAct = 1754592240,
    CaptureObject = 99164704,
    CreateCompleteCostume = 2259179872,
    FillThermometer = 4014155199,
    TooManyCornersOnObject = 3854823973,

    WatchAllTutorials = 1138644437,
    */
    CreateLevelInOnlineMultiplayer = 7827313,
    
    PublishLevel = 3162761616,
    PublishVersusLevel = 678686527,
    PublishLevelWithLevelLinks = 1671263379,
    PublishLevelWithCustomIcon = 614160912,
    PublishedLevelIsXDaysOld = 3529196823,
    TotalPlaysOnOnePublishedLevel = 2726743844,
    TotalPlaysOnAllPublishedLevels = 1461886302,
    UniquePlaysOnOnePublishedLevel = 2820366538,
    UniquePlaysOnAllPublishedLevels = 3577962928,
    HeartsOnOnePublishedLevel = 3855761662,
    HeartsOnAllPublishedLevels = 1235998656,
    YaysOnOnePublishedLevel = 2086540124,
    YaysOnAllPublishedLevels = 2350171014,
    MostlyPositiveRatingAfter100RatingsOnOnePublishedLevel = 3142374916,
    MostlyPositiveRatingAfter100RatingsOnAllPublishedLevels = 975425892,

    // Share (Category 3)
    ActiveXMinutesInPod = 2772751697,

    UploadProfileComment = 668304829,
    UploadReview = 1690814737,
    YaysOnReview = 4193300003,
    FindLevelByReview = 3811162561,
    UploadPhoto = 3875888384,
    UploadPhotoContainingYouAndOnlinePlayers = 3615681011,
    HeartsOnYourProfile = 3765241666,
    HeartLevel = 628967857,
    HeartPlayer = 2657184976,

    SignIntoWebsite = 2691148325,
    HeartPlayerUsingWebsite = 1965011384,
    QueueLevelUsingWebsite = 2833810997,
    PlayLevelWhichWasQueuedUsingWebsite = 2334275886,
    
    PlayXCommunityLevelsWithBelow10Plays = 2778908462,
    Play3CommunityLevelsInMultiplayer = 2170314159,
    YayXCommunityLevelsWithBelow10Plays = 2778528358,
    YayXCommunityLevels = 1333342859,
    ViewAnotherPlayerProfile = 84783612,
    ViewAnotherPlayersRecentActivity = 3288819675,
    PlayLevelUsingRecentActivity = 2911421749,
    PublishLevelWithTitleIconLabelsDescription = 3360486592,

    /*
    SaveCostume = 1970710514,
    RandomizeCostume = 1778143657,
    */

    // Secret (Category 4)
    RunFansite = 947118302,
    AchieveTeamPick = 792777243,

    OnlineAtTheSameTimeAsXPlayers = 1516705701,
    PublishCoolTutorialOrLevel = 2625335534,
    WatchEntireCredits = 467756946,
    PlayOnAmysBirthday = 1362224114,
    PlayOnPembertonsBirthday = 1247187369,
    PlayOnMmsBirthday = 449185716,

    AchievePlatinum = 1675480291,
    AchieveCrown = 1441043795,

    PlaceRoseOnAnotherPlayerOnValentinesDay = 1351753636,
    WearPumpkinOnHalloween = 2575270278,
    WearTurkeyOnThanksgiving = 4196899426,
    WearChristmasCostumeOnChristmas = 1974089927,

    TouchMmDeveloper = 918699958,
    GetEmployedAtMm = 4167298582,
    BecomeGameDeveloper = 3706663457,

    // Move Pack (Category 5)

}