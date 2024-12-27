using Refresh.GameServer.Types.Challenges.LbpHub;
using Refresh.GameServer.Types.Levels;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Database;

public partial class GameDatabaseContext // Challenges
{
    #region Challenge

    public GameChallenge CreateChallenge(SerializedChallenge createInfo, GameLevel level, GameUser user)
    {
        DateTimeOffset now = this._time.Now;

        // Create challenge
        GameChallenge challenge = new()
        {
            Name = createInfo.Name,
            Publisher = user,
            Level = level,
            StartCheckpointUid = createInfo.StartCheckpointUid,
            EndCheckpointUid = createInfo.EndCheckpointUid,
            Type = (GameChallengeType)createInfo.Criteria[0].Type,
            PublishDate = now,
            LastUpdateDate = now,
            ExpirationDate = DateTimeOffset.FromUnixTimeMilliseconds
            (
                now.ToUnixTimeMilliseconds() + 
                SerializedChallenge.ToUnixMilliseconds(createInfo.Expires)
            ),
        };
        
        this.AddSequentialObject(challenge);

        return challenge;
    }

    public void RemoveChallenge(GameChallenge challenge)
    {
        this.Write(() => {
            // Remove Scores
            this.GameChallengeScores.RemoveRange(s => s.Challenge == challenge);
        
            // Remove Challenge
            this.GameChallenges.Remove(challenge);
        });
    }

    public void RemoveChallengesByUser(GameUser user)
    {
        this.Write(() => {
            // Remove Scores on challenges by the user
            this.GameChallengeScores.RemoveRange(s => s.Challenge.Publisher == user);
        
            // Remove Challenges by user
            this.GameChallenges.RemoveRange(c => c.Publisher == user);
        });
    }

    public void RemoveChallengesForLevel(GameLevel level)
    {
        this.Write(() => {
            // Remove Scores on challenges in the level
            this.GameChallengeScores.RemoveRange(s => s.Challenge.Level == level);

            // Remove Challenges in the level
            this.GameChallenges.RemoveRange(c => c.Level == level);
        });
    }

    public GameChallenge? GetChallengeById(int challengeId)
        => this.GameChallenges.FirstOrDefault(c => c.ChallengeId == challengeId);

    private IEnumerable<GameChallenge> FilterChallenges(IEnumerable<GameChallenge> challenges, string? filter)
    {
        long nowInMilliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        return filter switch
        {
            "active" => challenges.Where(c => c.ExpirationDate.ToUnixTimeMilliseconds() > nowInMilliseconds),
            "expired" => challenges.Where(c => c.ExpirationDate.ToUnixTimeMilliseconds() <= nowInMilliseconds),
            _ => challenges,
        };
    }

    public IEnumerable<GameChallenge> GetChallenges(string? filter = null)
        => this.FilterChallenges(this.GameChallenges, filter).AsEnumerable();

    public IEnumerable<GameChallenge> GetNotUsersChallenges(GameUser user, string? filter = null)
        => this.FilterChallenges(this.GameChallenges.Where(c => c.Publisher != user), filter).AsEnumerable(); 

    public IEnumerable<GameChallenge> GetChallengesByUser(GameUser user, string? filter = null)
        => this.FilterChallenges(this.GameChallenges.Where(c => c.Publisher == user), filter).AsEnumerable();

    public int GetTotalChallengesByUser(GameUser user, string? filter = null)
        => this.FilterChallenges(this.GameChallenges.Where(c => c.Publisher == user), filter).Count();

    public IEnumerable<GameChallenge> GetChallengesByUsersMutuals(GameUser user, string? filter = null)
        => this.FilterChallenges(this.GameChallenges.Where(c => c.Publisher == user), filter).AsEnumerable();

    public int GetTotalChallengesByUsersMutuals(GameUser user, string? filter = null)
        => this.FilterChallenges(this.GameChallenges.Where(c => this.GetUsersMutuals(user).Contains(c.Publisher)), filter).Count();

    public IEnumerable<GameChallenge> GetChallengesForLevel(GameLevel level, string? filter = null)
        => this.FilterChallenges(this.GameChallenges.Where(c => c.Level == level), filter).AsEnumerable();

    public int GetTotalChallengesForLevel(GameLevel level, string? filter = null)
        => this.FilterChallenges(this.GameChallenges.Where(c => c.Level == level), filter).Count();

    #endregion
    

    #region Score

    public GameChallengeScore CreateChallengeScore(SerializedChallengeAttempt attempt, GameChallenge challenge, GameUser user)
    {
        DateTimeOffset now = DateTimeOffset.Now;

        // Create new score
        GameChallengeScore score = new()
        {
            Challenge = challenge,
            Publisher = user,
            Score = attempt.Score,
            GhostHash = attempt.GhostHash,
            PublishDate = now,
        };

        this.Write(() => 
        {
            // Add the new score
            this.GameChallengeScores.Add(score);
        });

        return score;
    }

    public void RemoveChallengeScore(GameChallengeScore score)
    {
        this.Write(() => 
        {
            this.GameChallengeScores.Remove(score);
        });
        
    }

    public void RemoveChallengeScoresByUser(GameUser user)
    {
        this.Write(() => 
        {
            this.GameChallengeScores.RemoveRange(s => s.Publisher == user);
        });
    }

    public GameChallengeScore? GetFirstScoreForChallenge(GameChallenge challenge)
        => this.GameChallengeScores.FirstOrDefault(s => s.Challenge == challenge);

    public IEnumerable<GameChallengeScore> GetChallengeScoresByUser(GameUser user)
        => this.GameChallengeScores.Where(s => s.Publisher == user).AsEnumerable();

    public int GetTotalScoresByUser(GameUser user)
        => this.GameChallengeScores.Where(s => s.Publisher == user).Count();

    public IEnumerable<GameChallengeScore> GetScoresForChallenge(GameChallenge challenge, bool orderByScore = true)
    {
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == challenge).AsEnumerable();

        if(orderByScore) return scores.OrderByDescending(s => s.Score);
        return scores;
    }

    public int GetTotalScoresForChallenge(GameChallenge challenge)
        => this.GameChallengeScores.Where(s => s.Challenge == challenge).Count();

    public IEnumerable<GameChallengeScore> GetScoresForChallengeByUser(GameChallenge challenge, GameUser user, bool orderByScore = true)
    {
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == challenge && s.Publisher == user).AsEnumerable();

        if(orderByScore) return scores.OrderByDescending(s => s.Score);
        return scores;
    }

    public int GetTotalScoresForChallengeByUser(GameChallenge challenge, GameUser user)
        => this.GameChallengeScores.Where(s => s.Challenge == challenge && s.Publisher == user).Count();

    public IEnumerable<GameChallengeScore> GetScoresForChallengeByUsersMutuals(GameChallenge challenge, GameUser user, bool orderByScore = true)
    {
        IEnumerable<GameUser> mutuals = this.GetUsersMutuals(user);
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == challenge);
        scores = scores.Where(s => mutuals.Contains(s.Publisher));

        if(orderByScore) return scores.OrderByDescending(s => s.Score);
        return scores;
    }

    public IEnumerable<SerializedChallengeScore> GetScoresAroundChallengeScore(GameChallengeScore score, int count)
    {
        if (count <= 2 || count % 2 != 1) 
            throw new ArgumentException("The number of scores must be odd and above 2.", nameof(count));

        List<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == score.Challenge)
            .OrderByDescending(s => s.Score)
            .ToList();

        return scores.Select((s, i) => SerializedChallengeScore.FromOld(s, i + 1)!)
            .Skip(Math.Min(scores.Count, scores.IndexOf(score) - count / 2)) // center user's score around other scores
            .Take(count)
            .AsEnumerable();
    }

    public GameChallengeScore? GetNewestScoreForChallengeByUser(GameChallenge challenge, GameUser user)
        => this.GameChallengeScores.Last(s => s.Challenge == challenge && s.Publisher == user);
            

    #endregion
}