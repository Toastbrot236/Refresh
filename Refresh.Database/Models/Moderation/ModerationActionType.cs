using Newtonsoft.Json.Converters;

namespace Refresh.Database.Models.Moderation;

[JsonConverter(typeof(StringEnumConverter))]
public enum ModerationActionType : byte
{
    // Users
    UserModification = 0,
    UserPlanetsReset,
    UserDeletion,
    UserRestriction,
    UserBan,
    UserPardon,
    PinProgressDeletion,

    // Levels
    LevelModification = 20,
    LevelDeletion,
    ReattributeLevelFromUser,
    ReattributeLevelToUser,

    // Playlists
    PlaylistModification = 40,
    PlaylistDeletion,

    // Photos
    PhotoDeletion = 60,
    PhotosByUserDeletion,

    // Scores
    ScoreDeletion = 80,
    ScoresByUserForLevelDeletion,
    ScoresByUserDeletion,

    // Reviews
    ReviewDeletion = 100,
    ReviewsByUserDeletion,
    
    // Comments
    LevelCommentDeletion = 120,
    LevelCommentsByUserDeletion,
    ProfileCommentDeletion,
    ProfileCommentsByUserDeletion,

    // Assets
    BlockAsset = 140,
    UnblockAsset,

    // Challenges
    ChallengeDeletion = 160,
    ChallengesByUserDeletion,
    ChallengeScoreDeletion,
    ChallengeScoresByUserDeletion,
}