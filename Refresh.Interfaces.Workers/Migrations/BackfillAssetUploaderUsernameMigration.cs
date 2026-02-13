using Refresh.Database.Models.Assets;
using Refresh.Workers;

namespace Refresh.Interfaces.Workers.Migrations;

public class BackfillAssetUploaderUsernameMigration : MigrationJob<GameAsset>
{
    protected override IQueryable<GameAsset> SortAndFilter(IQueryable<GameAsset> query)
    {
        return query
            .Where(a => a.OriginalUploader != null)
            .OrderBy(a => a.AssetHash);
    }

    protected override void Migrate(WorkContext context, GameAsset[] batch)
    {
        foreach (GameAsset asset in batch)
        {
            asset.OriginalUploaderUsername = asset.OriginalUploader?.Username;
        }

        context.Database.UpdateAssetsInDatabase(batch);
    }
}