using HipHopFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndustrialPark.SaveFile
{
    public static class SaveFileScene
    {
        public static string GenerateReport(ArchiveEditorFunctions archive, Platform platform, byte[] data)
        {
            var assetTypes = AssetOrder;
            var report = new StringBuilder();
            try
            {
                var bitReader = new BitReader(data, platform.Endianness());

                report.AppendLine("Save File Report");
                report.AppendLine();
                report.AppendLine($"Buffer: {string.Join("", bitReader.Buffer.Select(b => b ? '1' : '0'))} ");
                report.AppendLine();
                report.AppendLine($"Bit 1 = {bitReader.ReadBit()}");
                report.AppendLine($"OffsetX = {bitReader.ReadFloat()}");
                report.AppendLine($"OffsetY = {bitReader.ReadFloat()}");
                report.AppendLine();

                foreach (var assetType in assetTypes.Keys)
                {
                    var assets = GetPersistentAssetsOfType(archive, assetType);
                    foreach (var asset in assets)
                    {
                        report.AppendLine($"[{assetType}] {asset.assetName}");
                        foreach (var (property, dataType) in assetTypes[assetType])
                        {
                            var value =
                                dataType == DataType.Bit1 ? bitReader.ReadB1() :
                                dataType == DataType.Bit7 ? bitReader.ReadB7() :
                                dataType == DataType.U8 ? bitReader.ReadByte() :
                                dataType == DataType.U16 ? bitReader.ReadUInt16() :
                                dataType == DataType.U32 ? bitReader.ReadUInt32() :
                                dataType == DataType.Float ? bitReader.ReadFloat() :
                                0;
                            report.AppendLine($"- [{dataType}] {property} = {value}");
                        }
                        report.AppendLine();
                    }
                }
            }
            catch (Exception ex)
            {
                report.AppendLine($"Error generating report: {ex.Message}");
            }
            return report.ToString();
        }

        private static IEnumerable<Asset> GetPersistentAssetsOfType(ArchiveEditorFunctions archive, AssetType assetType)
        {
            var result = new List<Asset>();
            for (int i = 0; i < (archive.NoLayers ? 1 : archive.LayerCount); i++)
            {
                var layer = archive.GetAssetIDsOnLayer(i);
                foreach (var assetID in layer)
                {
                    var asset = archive.GetFromAssetID(assetID);
                    if (asset.assetType == assetType && asset is BaseAsset baseAsset && baseAsset.StateIsPersistent)
                    {
                        result.Add(asset);
                    }
                }
            }
            return result;
        }

        public enum DataType
        {
            Bit1,
            Bit7,
            U8,
            U16,
            U32,
            Float,
        }

        public static (string, DataType)[] EmptyFormat => new (string, DataType)[0];
        public static (string, DataType)[] BaseFormat => new (string, DataType)[] { ("Enabled", DataType.Bit1) };
        public static (string, DataType)[] TimerFormat => BaseFormat.Concat(new (string, DataType)[] { ("State", DataType.U8), ("Seconds Left", DataType.Float) }).ToArray();
        public static (string, DataType)[] CounterFormat => BaseFormat.Concat(new (string, DataType)[] { ("State", DataType.U8), ("Count", DataType.U16) }).ToArray();
        public static (string, DataType)[] EntFormat => BaseFormat.Concat(new (string, DataType)[] { ("Visible", DataType.Bit1) }).ToArray();
        public static (string, DataType)[] PickupFormat => EntFormat.Concat(new (string, DataType)[] { ("State", DataType.Bit7), ("Initially Visible", DataType.Bit1) }).ToArray();
        public static (string, DataType)[] ButtonFormat => EntFormat.Concat(new (string, DataType)[] { ("Pressing", DataType.Bit1), ("Pressed", DataType.Bit1) }).ToArray();
        public static (string, DataType)[] DestructibleObjectFormat => EntFormat.Concat(new (string, DataType)[] { ("State", DataType.U32) }).ToArray();
        public static (string, DataType)[] TeleportBoxFormat => EntFormat.Concat(new (string, DataType)[] { ("Open", DataType.Bit1), ("Current Player State", DataType.U32) }).ToArray();
        public static (string, DataType)[] TaskBoxFormat => new (string, DataType)[] { ("State", DataType.U8) };

        public static Dictionary<AssetType, (string, DataType)[]> AssetOrder => new Dictionary<AssetType, (string, DataType)[]>
        {
            { AssetType.Trigger, EntFormat },
            { AssetType.MovePoint, BaseFormat },
            { AssetType.Pickup, PickupFormat },
            { AssetType.SimpleObject, EntFormat },
            { AssetType.ParticleSystem, EmptyFormat },
            { AssetType.ParticleEmitter, EmptyFormat },
            { AssetType.Track, EmptyFormat },
            { AssetType.Platform, EntFormat },
            { AssetType.Pendulum, EntFormat },
            { AssetType.Hangable, EntFormat },
            { AssetType.DestructibleObject, DestructibleObjectFormat },
            { AssetType.Boulder, EmptyFormat },
            { AssetType.NPC, BaseFormat }, // todo
            { AssetType.Button, ButtonFormat },
            { AssetType.Player, EmptyFormat },
            { AssetType.Timer, TimerFormat },
            { AssetType.Counter, CounterFormat },
            { AssetType.SFX, BaseFormat },
            { AssetType.Group, BaseFormat },
            { AssetType.Portal, BaseFormat },
            { AssetType.Camera, BaseFormat },
            { AssetType.Surface, BaseFormat },
            { AssetType.Gust, BaseFormat },
            { AssetType.Volume, BaseFormat },
            { AssetType.Conditional, BaseFormat },
            { AssetType.LobMaster, EmptyFormat },
            { AssetType.Environment, BaseFormat },
            { AssetType.Dispatcher, BaseFormat },
            { AssetType.UserInterface, EntFormat },
            { AssetType.UserInterfaceFont, EntFormat },
            { AssetType.Fog, BaseFormat },
            { AssetType.Light, BaseFormat },
            { AssetType.CutsceneManager, BaseFormat },
            { AssetType.ElectricArcGenerator, EntFormat },
            { AssetType.Script, BaseFormat },
            { AssetType.DiscoFloor, BaseFormat },
            { AssetType.TeleportBox, TeleportBoxFormat },
            { AssetType.BusStop, EmptyFormat },
            { AssetType.TextBox, EmptyFormat },
            { AssetType.TalkBox, EmptyFormat },
            { AssetType.TaskBox, TaskBoxFormat },
            { AssetType.BoulderGenerator, EmptyFormat },
            { AssetType.Taxi, BaseFormat },
            { AssetType.HUDModel, EmptyFormat },
            { AssetType.HUDMeterFont, EmptyFormat },
            { AssetType.HUDMeterUnit, EmptyFormat },
            { AssetType.HUDText, EmptyFormat },
            { AssetType.BungeeHook, EmptyFormat },
            { AssetType.FlythroughObject, BaseFormat },
            { AssetType.CameraTweak, BaseFormat },
        };
    }
}