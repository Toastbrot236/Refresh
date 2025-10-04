using System.Xml.Serialization;

namespace Refresh.Interfaces.Game.Types.Activity.SerializedEvents;

public class SerializedLevelUploadEvent : SerializedLevelEvent
{
    // 0 = modified, 1 = published
    [XmlElement("republish")] public int Republish { get; set; } = 1;

    public static SerializedLevelUploadEvent? FromSerializedLevelEvent(SerializedLevelEvent? e, bool isModified)
    {
        if (e == null)
            return null;
        
        return new SerializedLevelUploadEvent
        {
            Republish = isModified ? 0 : 1,

            Actor = e.Actor,
            LevelId = e.LevelId,
            Timestamp = e.Timestamp,
            Type = e.Type,
        };
    }
}