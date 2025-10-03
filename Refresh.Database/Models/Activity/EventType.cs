using System.Xml.Serialization;
using Newtonsoft.Json.Converters;

namespace Refresh.Database.Models.Activity;

[JsonConverter(typeof(StringEnumConverter))]
/// <summary>
/// Contains both types shown in-game, API-exclusive custom events, and also many moderation-related events
/// as some sort of audit log (incase a user starts rapidly editing their stuff, or a mod starts causing trouble)
/// </summary>
public enum EventType : byte
{
    [XmlEnum("publish_level")] LevelUpload = 0,
    [XmlEnum("heart_level")] LevelFavourite = 1,
    [XmlEnum("unheart_level")] LevelUnfavourite = 2,
    [XmlEnum("heart_user")] UserFavourite = 3,
    [XmlEnum("unheart_user")] UserUnfavourite = 4,
    [XmlEnum("play_level")] LevelPlay = 5,
    [XmlEnum("rate_level")] LevelStarRate = 6, // as opposed to dpad rating. unused since we convert stars to dpad
    [XmlEnum("tag_level")] LevelTag = 7,
    [XmlEnum("comment_on_level")] PostLevelComment = 8,
    [XmlEnum("delete_level_comment")] DeleteLevelComment = 9,
    [XmlEnum("upload_photo")] PhotoUpload = 10,
    [XmlEnum("unpublish_level")] LevelUnpublish = 11,
    [XmlEnum("news_post")] NewsPost = 12,
    [XmlEnum("mm_pick_level")] LevelTeamPick = 13,
    [XmlEnum("dpad_rate_level")] LevelRate = 14,
    [XmlEnum("review_level")] LevelReview = 15,
    [XmlEnum("comment_on_user")] PostUserComment = 16,
    [XmlEnum("create_playlist")] PlaylistCreate = 17,
    [XmlEnum("heart_playlist")] PlaylistFavourite = 18,
    [XmlEnum("add_level_to_playlist")] PlaylistAddLevel = 19,
    [XmlEnum("score")] LevelScore = 20,

    // Custom events, mostly additional moderation events.
    
    // No Create event for pin progress and no profile pin events at all because that'd get very spammy very quickly
    // wwithout any benefit. However tracking deleted pin progresses is probably a useful moderation event.
    [XmlEnum("delete_pin_progress")] DeletePinProgress = 105,
    [XmlEnum("delete_photo")] DeletePhoto = 106,
    [XmlEnum("un_mm_pick_level")] LevelUnTeamPick = 107,
    // For some weird reason the lower two specifically don't exist in LBP2
    [XmlEnum("delete_review")] DeleteReview = 108,
    [XmlEnum("delete_user_comment")] DeleteUserComment = 109, 
    [XmlEnum("create_contest")] CreateContest = 110,
    [XmlEnum("delete_contest")] DeleteContest = 111,
    [XmlEnum("delete_score")] DeleteScore = 112,
    [XmlEnum("delete_user_scores")] DeleteUserScores = 113,
    // Useful if the user has submitted multiple cheated scores on a single level, only one is showing as a highscore at a time, 
    // but you don't want to snipe the user's other level scores because they're mostly legit
    [XmlEnum("delete_user_level_scores")] DeleteUserLevelScores = 114, 
    [XmlEnum("moderate_asset")] ModerateAsset = 115,
    [XmlEnum("unmoderate_asset")] UnmoderateAsset = 116,
    [XmlEnum("moderate_user")] ModerateUser = 117, // Description should also tell whether restricted, banned or anything else
    [XmlEnum("pardon_user")] PardonUser = 118,
    [XmlEnum("delete_challenge")] DeleteChallenge = 119,
    [XmlEnum("delete_user_challenges")] DeleteUserChallenges = 120,
    [XmlEnum("delete_challenge_score")] DeleteChallengeScore = 121,
    [XmlEnum("delete_user_challenge_scores")] DeleteUserChallengeScores = 122,
    [XmlEnum("create_challenge")] CreateChallenge = 123,
    [XmlEnum("create_challenge_score")] CreateChallengeScore = 124,
    [XmlEnum("add_playlist_to_playlist")] PlaylistAddPlaylist = 125,
    [XmlEnum("delete_playlist")] DeletePlaylist = 126,
    [XmlEnum("firstlogin")] UserFirstLogin = 127,
}