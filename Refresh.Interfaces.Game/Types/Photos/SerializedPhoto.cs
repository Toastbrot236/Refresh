using System.Xml.Serialization;
using Refresh.Core.Types.Data;
using Refresh.Database.Models.Photos;

namespace Refresh.Interfaces.Game.Types.Photos;

#nullable disable

[XmlRoot("photo")]
[XmlType("photo")]
public class SerializedPhoto : IDataConvertableFrom<SerializedPhoto, GamePhoto>
{
    [XmlAttribute("timestamp")]
    public long Timestamp { get; set; }

    [XmlElement("id")]
    public int PhotoId { get; set; } = 0;

    [XmlElement("author")]
    public string AuthorName { get; set; } = "";
    
    [XmlElement("small")] public string SmallHash { get; set; }
    [XmlElement("medium")] public string MediumHash { get; set; }
    [XmlElement("large")] public string LargeHash { get; set; }
    [XmlElement("plan")] public string PlanHash { get; set; }
    
    [XmlElement("slot")] public SerializedPhotoLevel Level { get; set; }
    
    [XmlArray("subjects")] public List<SerializedPhotoSubject> PhotoSubjects { get; set; }

    public static SerializedPhoto FromOld(GamePhoto old, DataContext dataContext)
    {
        SerializedPhoto newPhoto = new()
        {
            PhotoId = old.PhotoId,
            AuthorName = old.Publisher.Username,
            Timestamp = old.TakenAt.ToUnixTimeMilliseconds(),
            // NOTE: we usually would do `if psp, prepend psp/ to the hashes`,
            // but since we are converting the psp TGA assets to PNG in FillInExtraData, we don't need to!
            // also, I think the game would get mad if we did that
            LargeHash = dataContext.GetIconFromHash(old.LargeAssetHash),
            MediumHash = dataContext.GetIconFromHash(old.MediumAssetHash),
            SmallHash = dataContext.GetIconFromHash(old.SmallAssetHash),
            PlanHash = old.PlanHash,
            PhotoSubjects = new List<SerializedPhotoSubject>(old.Subjects.Count),
        };
        
        foreach (GamePhotoSubject subject in old.Subjects)
        {
            SerializedPhotoSubject newSubject = new()
            {
                Username = subject.User?.Username ?? subject.DisplayName,
                DisplayName = subject.DisplayName,
                BoundsList = string.Join(',', subject.Bounds),
            };

            newPhoto.PhotoSubjects.Add(newSubject);
        }

        return newPhoto;
    }

    public static IEnumerable<SerializedPhoto> FromOldList(IEnumerable<GamePhoto> oldList, DataContext dataContext)
        => oldList.Select(p => FromOld(p, dataContext)!);
}