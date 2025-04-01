namespace Refresh.GameServer.Types.Pins;

/// <summary>
/// Assigns a readable, generic name describing a LBP3 pin to a "progressType" (what the game uses to identify pins) 
/// for easier readability when unlocking them. 
/// Only individual pins relevant to the server are listed here (like, for example, pins the game expects to be awarded by the server, 
/// completing pin objectives in games which don't support pins (like LBP1), aswell as pins awarded for doing things using the website 
/// (ApiV3 in our case)). Use GamePinConfig for importing proper pin metadata, aswell as other pins not listed here.
/// </summary>
/// <remarks>
/// Partially researched using the UpdateMyPins endpoint, mostly taken from
/// https://github.com/LittleBigRefresh/Docs/blob/main/Docs/pin-files/lbp3.json
/// </remarks>
public enum Lbp3GamePin : long
{
    // Category 11
    DecoratePlanets = 1645300694,

    // Category 12
    PlayCommunityAdventure = 514355492,
    PublishAdventure = 453807463,
    HeartAdventure = 3282774144,
    UploadAdventureReview = 3256837034,

    CreatePlaylist = 1982842997,
    GetXHeartsOnOneOfYourPlaylists = 3036946958,

    // Category 15
    TeamPicked = 295684954,
}