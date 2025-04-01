
using Refresh.GameServer.Extensions;
using Refresh.GameServer.Types.Pins;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Database;

public partial class GameDatabaseContext // Pins
{
    public void UpdateUserPins(GameUser user, SerializedUserPins pinsUpdate) 
    {
        Dictionary<long, int> progressPins = pinsUpdate.FromSerializedProgressPins();
        Dictionary<long, int> awardPins = pinsUpdate.FromSerializedAwardPins();

        this.Write(() => {

            foreach (KeyValuePair<long, int> progressPin in progressPins)
            {
                
            }

            foreach (KeyValuePair<long, int> awardPin in awardPins)
            {

            }

            foreach (long profilePins in pinsUpdate.ProfilePins)
            {
            
            }
        });
    }

    public UserPinProgressRelation UpdateUserPin(GameUser user, int progressTypeId, int progress)
    {
        UserPinProgressRelation? pinToUpdate = this.GetPinByUserAndId(progressTypeId, user);
        DateTimeOffset now = this._time.Now;

        if (pinToUpdate == null)
        {
            // New pin
            UserPinProgressRelation newPin = new()
            {
                ProgressTypeId = progressTypeId,
                Progress = progress,
                User = user,
                PublishDate = now,
                LastUpdateDate = now,
            };

            this.Write(() => {
                this.GamePinRelations.Add(newPin);
            });

            return newPin;
        }
        else
        {
            // Update pin
            this.Write(() => {
                pinToUpdate.Progress = progress;
                pinToUpdate.LastUpdateDate = now;
            });
            
            return pinToUpdate;
        }
    }

    public UserPinProgressRelation? GetPinByUserAndId(long progressTypeId, GameUser user)
        => this.GamePinRelations.FirstOrDefault(p => p.ProgressTypeId == progressTypeId && p.User == user);

}