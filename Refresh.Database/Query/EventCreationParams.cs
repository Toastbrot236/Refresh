using Refresh.Database.Models.Activity;
using Refresh.Database.Models.Users;

namespace Refresh.Database.Query;

public class EventCreationParams
{
    public required EventType EventType { get; set; }
    public required GameUser Actor { get; set; }
    public required EventOverType OverType { get; set; }
    public bool IsModified { get; set; } = false;
    public string AdditionalInfo { get; set; } = "";
}