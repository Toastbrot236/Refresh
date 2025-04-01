namespace Refresh.GameServer.Types.Pins;

/// <summary>
/// Assigns a readable, generic name describing a LBP2 pin to a "progressType" (what the game uses to identify pins) 
/// for easier readability when unlocking them. 
/// Only individual pins relevant to the server are listed here (like, for example, pins the game expects to be awarded by the server, 
/// completing pin objectives in games which don't support pins (like LBP1), aswell as pins awarded for doing things using the website 
/// (ApiV3 in our case)). Use GamePinConfig for importing proper pin metadata, aswell as other pins not listed here.
/// </summary>
/// <remarks>
/// Partially researched using the UpdateMyPins endpoint, mostly taken from
/// https://github.com/LittleBigRefresh/Docs/blob/main/Docs/pin-files/lbp3.json
/// </remarks>
public enum Lbp2GamePin : long
{
    // Play - Story Mode (Category 0)
    ActiveXMinutesInStoryLevels = 3253443510,

    TopXPercentOfScoresAgainst50PlayersInOneStoryLevel = 191183438,
    Top25PercentOfScoresAgainst50PlayersInXStoryLevels = 3394094772,
    
    StoryOnlineMultiplayerXCoopLevelWins = 2529770882,
    StoryLocalMultiplayerXCoopLevelWins = 273149064,
    StoryXCoopLevelsCompletedInMultiplayer = 169261587,
    StoryOnlineMultiplayerXVersusLevelWins = 748145250,
    StoryLocalMultiplayerXVersusLevelWins = 1654785788,
    StoryXVersusLevelsCompletedInMultiplayer = 3999586963,

    // Play - General (Category 1)
    ActiveXMinutesInCommunityLevels = 510577794,
    CompleteXUniqueCommunityLevels = 1109950816,
    AceXUniqueCommunityLevels = 3631168967,
    CompleteXUniqueCoopCommunityLevels = 1524408084,
    CompleteXUniqueVersusCommunityLevels = 2249488698,
    CompleteXNewCommunityLevels = 3816103083,
    PlayTeamPickLevel = 2051925987,

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

    // Create (Category 2)
    ActiveXMinutesInCreateMode = 1928306284,
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
    UploadLevelReview = 1690814737,
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

    // Secret (Category 4)
    RunFansite = 947118302,
    AchieveTeamPick = 792777243,

    OnlineAtTheSameTimeAsXPlayers = 1516705701,
    PublishCoolTutorialOrLevel = 2625335534,

    TouchMmDeveloper = 918699958,
    GetEmployedAtMm = 4167298582,
    BecomeGameDeveloper = 3706663457,

    WinContraptionChallenge = 104630839,
    LbpMeetupVisitor = 896332631,
    Awesomesauce = 1785363595,

    // Move Pack (Category 5)
    UploadMoveControllerLevel = 3565237483,
    PlayMoveControllerLevelOnFriday = 2337093026,
    PlayXCommunityLevelsRequiringMoveController = 3672403750,
    SetLevelIconToPainting = 3492656560,
    SetProfileIconToPainting = 2003959265,
}