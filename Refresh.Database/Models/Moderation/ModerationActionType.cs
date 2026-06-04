using Newtonsoft.Json.Converters;

namespace Refresh.Database.Models.Moderation;

[JsonConverter(typeof(StringEnumConverter))]
public enum ModerationActionType : byte
{
    // Users
    UserModification = 0,
    UserDeletion,
    UserPunishment,
    UserPardon,
    PinProgressDeletion,
    UsernameDisallowance,
    UsernameReallowance,
    EmailAddressDisallowance,
    EmailAddressReallowance,
    EmailDomainDisallowance,
    EmailDomainReallowance,

    // Levels
    LevelModification = 20,
    LevelDeletion,
    LevelsByUserDeletion,

    // Playlists
    PlaylistModification = 40,
    PlaylistDeletion,
    PlaylistsByUserDeletion,

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