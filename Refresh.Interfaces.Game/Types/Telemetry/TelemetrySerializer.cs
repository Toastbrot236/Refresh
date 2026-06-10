using NotEnoughLogs;
using Refresh.Common;
using Refresh.Core.Importing;
using Refresh.Interfaces.Game.Types.Telemetry.Binary;

namespace Refresh.Interfaces.Game.Types.Telemetry;

// Common revisions
    // LBP1 01.21 is 0x2 (Start is 0x0 instead of 0x1 in this version?)
    // LBP1 Deploy is 0x3 (Deploy is both before/after LBP1, branches)
    // LBP1 01.30-Final is 0xd
    // LBP2 Pre-Alpha is 0xe
    // LBP2 Move Beta is 0x19
    // LBP2 Final is 0x1f
    // LBP2 Vita Final is 0x1e
    // LBP2 Hub is 0x1e
    // LBP3 Alpha is 0x1b
    
// LBP1 only has telemetry messages up until E_TELEMETRY_EVENT_DLC_OWNED
    // Deploy only has up to E_TELEMETRY_EVENT_LEAVE_LEVEL, but why is anyone using deploy
// LBP2 has telemetry messages up until E_TELEMETRY_DCDS_ACTION
// LBP3 has whatever is after, I'm honestly not bothering to go through them right now.
// LBP Vita has telemetry messages up until E_TELEMETRY_GAME_PROGRESSION
public static class TelemetrySerializer
{
    public static TelemetryHeader? DeserializeHeader(MemoryBitStream stream, Logger? logger = null)
    {
        TelemetryHeader header = new();
        ushort revision = stream.ReadUInt16();

        header.Revision = revision;
        header.HashedPlayerId = stream.ReadUInt32();
        InlineHash hash = new();
        
        if (revision >= 0x12)
        {
            stream.ReadExactly(hash);
            header.LevelHash = hash;
        }
        
        if (revision >= 0x13)
        {
            header.SlotType = stream.ReadUInt32();
            header.SlotNumber = stream.ReadUInt32();
        }

        // All position messages have a CHash serialized before the
        // frame timestamp specifically between these two revisions and I don't
        // want to handle that case, all updated and beta builds currently in use
        // do not use these revisions, so I don't consider it a priority.
        if (revision is >= 0x10 and < 0x12)
            return null; // tell endpoint to cancel request
        
        // Between revisions 1 and 5, only the first 4 bytes of hashes were serialized
        // after these revisions, the full SHA1 is serialized.
        header.HasFullHash = revision >= 0x5;
        
        // Many messages have frame timestamps prepended after a certain revision.
        header.HasTimestamps = revision >= 0x1d;
        logger?.LogDebug(RefreshContext.Telemetry, $"telheader: rev {revision} hashedplayerid {header.HashedPlayerId} lvl hash {Convert.ToHexString(hash)} slottype {header.SlotType} slotnum {header.SlotNumber} hasfullhash {header.HasFullHash} hastimestamps {header.HasTimestamps}");
        return header;
    }

    public static TelemetryUserExperienceMetrics DeserializeMetrics(MemoryBitStream stream, Logger? logger = null)
    {
        // These values are probably not accurate in terms of names,
        // well they could be close, since it seems they're probably(?)
        // the same as the LBP3 JSON versions, but who knows, it at least
        // is the correct data size.
        TelemetryUserExperienceMetrics metrics = new()
        {
            CurrentMspf = stream.ReadSingle(),
            AverageMspf = stream.ReadSingle(),
            HighMspf = stream.ReadSingle(),
            PredictApplied = stream.ReadUInt32(),
            PredictDesired = stream.ReadUInt32(),
            IsHost = stream.ReadBit(),
            IsCreate = stream.ReadBit(),
            NumPlayers = stream.ReadUInt32(),
            NumPs3s = stream.ReadUInt32(),
            AverageRttHost = stream.ReadSingle(),
            BandwidthUsage = stream.ReadSingle(),
            WorstPing = stream.ReadSingle(),
            WorstBandwidth = stream.ReadSingle(),
            WorstPacketLoss = stream.ReadSingle(),
            WorstPlayers = stream.ReadUInt32(),
            HttpBandwidthUp = stream.ReadSingle(),
            HttpBandwidthDown = stream.ReadSingle(),
            Frame = stream.ReadUInt32(),
            LastMgjFrame = stream.ReadUInt32(),
        };

        logger?.LogDebug(RefreshContext.Telemetry, $"user exp metrics 1: curMSPF {metrics.CurrentMspf} avgMSPF {metrics.AverageMspf} highMSPF {metrics.HighMspf} predictapplied {metrics.PredictApplied} predictdesired {metrics.PredictDesired}");
        logger?.LogDebug(RefreshContext.Telemetry, $"user exp metrics 2: ishost {metrics.IsHost} iscreate {metrics.IsCreate} numplayers {metrics.NumPlayers} numps3s {metrics.NumPs3s} avgRTThost {metrics.AverageRttHost} bwusage {metrics.BandwidthUsage}");
        logger?.LogDebug(RefreshContext.Telemetry, $"user exp metrics 3: worstping {metrics.WorstPing} worstbw {metrics.WorstBandwidth} worstpacketloss {metrics.WorstPacketLoss} worstplayers {metrics.WorstPlayers} httpbwup {metrics.HttpBandwidthDown} httpbwdown {metrics.HttpBandwidthDown}");
        logger?.LogDebug(RefreshContext.Telemetry, $"user exp metrics 3: frame {metrics.Frame} lastmgjframe {metrics.LastMgjFrame}");
        
        for (int i = 0; i < metrics.NumPlayers; ++i)
        {
            TelemetryPlayerNetStats stats = new()
            {
                Frame = stream.ReadUInt32(),
                Player = stream.ReadUInt32(),
                IsLocal = stream.ReadBit(),
                AvailableBandwidth = stream.ReadUInt32(),
                AvailableRnpBandwidth = stream.ReadUInt32(),
                AvailableGameBandwidth = stream.ReadSingle(),
                RecentTotalBandwidthUsed = stream.ReadUInt32(),
                TimeBetweenSends = stream.ReadSingle(),
            };
            logger?.LogDebug(RefreshContext.Telemetry, $"#{i} telplayernetstat 1: frame {stats.Frame} player {stats.Player} islocal {stats.IsLocal} availBW {stats.AvailableBandwidth}");
            logger?.LogDebug(RefreshContext.Telemetry, $"#{i} telplayernetstat 2: availrnpbw {stats.AvailableRnpBandwidth} availgamebw {stats.AvailableGameBandwidth}");
            logger?.LogDebug(RefreshContext.Telemetry, $"#{i} telplayernetstat 3: rectotalbx {stats.RecentTotalBandwidthUsed} timebetweensends {stats.TimeBetweenSends}");
            metrics.PlayerNetStats.Add(stats);
        }

        return metrics;
    }

    public static TelemetryInventoryItem DeserializeInventoryItem(MemoryBitStream stream, Logger? logger = null)
    {
        TelemetryInventoryItem item = new()
        {
            Action = stream.ReadUInt32(),
            Type = stream.ReadUInt32(),
        };

        uint numGuids = stream.ReadUInt32();
        for (int i = 0; i < numGuids; ++i)
        {
            item.Guids.Add(stream.ReadUInt32());
        }
            

        uint numHashes = stream.ReadUInt32();
        for (int i = 0; i < numHashes; ++i)
        {
            InlineHash hash = new();
            stream.ReadExactly(hash);
            item.Hashes.Add(hash);
        }

        return item;
    }

    public static TelemetryGameMessage DeserializeGameMessage(MemoryBitStream stream, ushort revision, Logger? logger = null)
    {
        TelemetryGameMessage msg = new()
        {
            // Probably important to note that the types get moved around depending on the version of the game,
            // for example EGMT_ALERT in LBP2 is 19, while in LBP3, it's 20
            Type = stream.ReadUInt32(),
        };
        
        // Some removed value, no builds seem to have this revision,
        // so it's probably not important to consider.
        if (revision < 0x15) stream.ReadUInt32();
        else msg.Key = stream.ReadUInt32();

        // This message has a max size of 40 bytes including the null terminator.
        msg.Message = stream.ReadString();

        return msg;
    }

    public static TelemetryPoppetState DeserializePoppetState(MemoryBitStream stream, ushort revision, Logger? logger = null)
    {
        TelemetryPoppetState poppet = new()
        {
            Mode = stream.ReadUInt32(),
            SubMode = stream.ReadUInt32(),
        };

        // Max size is 256 characters for whatever reason,
        // might contain other data in certain sub modes?
        if (revision >= 0x1d)
            poppet.Player = stream.ReadString();

        return poppet;
    }

    public static OpenPsid DeserializeOpenPsid(MemoryBitStream stream, Logger? logger = null)
    {
        return new()
        {
            Low = stream.ReadUInt64(),
            High = stream.ReadUInt64(),
        };
    }

    public static TelemetryPosition DeserializePosition(MemoryBitStream stream, ushort revision, Logger? logger = null)
    {
        TelemetryPosition pos = new()
        {
            X = stream.ReadSingle(),
            Y = stream.ReadSingle(),
            Layer = stream.ReadUInt32(),
        };
        
        // They already added the frame to most telemetry messages,
        // couldn't they have removed these duplicates?
        // Seems to always be the same as the prior frame value.
        if (revision >= 0x19)
            pos.Frame = stream.ReadUInt32();

        return pos;
    }
}