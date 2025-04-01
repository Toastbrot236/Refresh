using System.Collections.Frozen;
using Bunkum.Core;
using Bunkum.Core.Services;
using NotEnoughLogs;
using Refresh.GameServer.Configuration;

namespace Refresh.GameServer.Types.Pins;

public class GamePinService : EndpointService
{
    public readonly FrozenSet<GamePin> Lbp2Pins;
    public readonly FrozenSet<GamePin> Lbp3Pins;
    public readonly FrozenSet<GamePin> LbpVitaPins;
    public readonly FrozenSet<GamePin> BetaBuildPins;

    public GamePinService(Logger logger, GamePinConfig config) : base(logger)
    {
        logger.LogDebug(BunkumCategory.Service, "GamePinService start constructing");

        this.Lbp2Pins = FromOldArray(config.Lbp2Pins, logger).ToFrozenSet();
        logger.LogDebug(BunkumCategory.Service, $"GamePinService Lbp2 pins: {this.Lbp2Pins.Count}, here they are:");

        foreach(GamePin pin in Lbp2Pins)
        {
            logger.LogDebug(BunkumCategory.Service, $"Lbp2 pin: progress Type: {pin.ProgressTypeId}, trans Name: {pin.TranslatedName}, trans Desc: {pin.TranslatedDescription}, Category: {pin.Category}, Target Value count: {pin.TargetValues.Length}");
        }

        this.Lbp3Pins = FromOldArray(config.Lbp3Pins).ToFrozenSet();
        logger.LogDebug(BunkumCategory.Service, $"GamePinService Lbp 3 pins: {this.Lbp3Pins.Count}, here they are:");

        foreach(GamePin pin in Lbp3Pins)
        {
            logger.LogDebug(BunkumCategory.Service, $"Lbp3 pin: progress Type: {pin.ProgressTypeId}, trans Name: {pin.TranslatedName}, trans Desc: {pin.TranslatedDescription}, Category: {pin.Category}, Target Value count: {pin.TargetValues.Length}");
        }

        this.LbpVitaPins = FromOldArray(config.LbpVitaPins).ToFrozenSet();
        logger.LogDebug(BunkumCategory.Service, $"GamePinService Vita pins: {this.LbpVitaPins.Count}");

        this.BetaBuildPins = FromOldArray(config.BetaBuildPins).ToFrozenSet();
        logger.LogDebug(BunkumCategory.Service, $"GamePinService beta build pins: {this.BetaBuildPins.Count}");
    }

    public static IEnumerable<GamePin> FromOldArray(ImportedPin[] importedPins, Logger? logger = null)
        => importedPins
            .DistinctBy(p => p.ProgressType)
            .Select(p => new GamePin
            {
                ProgressTypeId = p.ProgressType,
                Category = p.Category,
                TranslatedName = p.TranslatedName ?? $"Unnamed Pin with progress type ID {p.ProgressType}",
                TranslatedDescription = p.TranslatedDescription ?? $"Undescribed Pin with progress type ID {p.ProgressType}",
                TargetValues = importedPins
                    .Where(q => q.ProgressType == p.ProgressType)
                    .Select(q => q.TargetValue)
                    .ToArray(),
            });
}