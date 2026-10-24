using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark
{
    public class AssetSHRP : Asset
    {
        public override string AssetInfo => $"{Entries.Length} entries";

        private const string categoryName = "Shrapnel";

        [Category(categoryName), Editor(typeof(AssetPropertyCollectionEditor), typeof(UITypeEditor)), AssetPropertyCollectionOptions(allowCopy: false)]
        public Shrapnel[] Entries { get; set; }
        [Browsable(false)]
        public AssetID ThisAssetID { get; set; }
        private int _internalpointer { get; set; }

        public AssetSHRP(string assetName) : base(assetName, AssetType.Shrapnel)
        {
            Entries = new Shrapnel[0];
        }

        public AssetSHRP(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                int count = reader.ReadInt32();
                ThisAssetID = reader.ReadUInt32();
                _internalpointer = reader.ReadInt32();
                Entries = new Shrapnel[count];

                for (int i = 0; i < Entries.Length; i++)
                {
                    var entryType = (IShrapnelType)reader.ReadInt32();

                    switch (entryType)
                    {
                        case IShrapnelType.Particle:
                            Entries[i] = new ShrapnelEntry_Particle(reader, game);
                            break;
                        case IShrapnelType.Projectile:
                            Entries[i] = new ShrapnelEntry_Projectile(reader, game);
                            break;
                        case IShrapnelType.Lightning:
                            Entries[i] = new ShrapnelEntry_Lightning(reader, game);
                            break;
                        case IShrapnelType.Sound:
                            Entries[i] = new ShrapnelEntry_Sound(reader, game);
                            break;
                        case IShrapnelType.Shockwave:
                            Entries[i] = new ShrapnelEntry_Shockwave(reader, game);
                            break;
                        case IShrapnelType.Explosion:
                            Entries[i] = new ShrapnelEntry_Explosion(reader, game);
                            break;
                        case IShrapnelType.Distortion:
                            Entries[i] = new ShrapnelEntry_Distortion(reader, game);
                            break;
                        case IShrapnelType.Fire:
                            Entries[i] = new ShrapnelEntry_Fire(reader, game);
                            break;
                        case IShrapnelType.Light:
                            Entries[i] = new ShrapnelEntry_Light(reader, game);
                            break;
                        case IShrapnelType.Smoke:
                            Entries[i] = new ShrapnelEntry_Smoke(reader, game);
                            break;
                        case IShrapnelType.Goo:
                            Entries[i] = new ShrapnelEntry_Goo(reader, game);
                            break;
                        default:
                            throw new Exception($"Unknown shrapnel entry type: {entryType}");
                    }
                }
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Entries.Length);
            if (ThisAssetID == 0 || ThisAssetID == assetID)
                writer.Write(assetID);
            else
            {
                for (int i = 0; i < assetName.Length; i++)
                {
                    AssetID tryId = new AssetID(assetName.Substring(0, i));
                    if (tryId == ThisAssetID)
                        writer.Write(tryId);
                }
            }
            writer.Write(_internalpointer);
            foreach (var e in Entries)
                e.Serialize(writer);
        }

        public static Shrapnel CreateEntry(IShrapnelType type, Game game)
        {
            return type switch
            {
                IShrapnelType.Particle => new ShrapnelEntry_Particle(game),
                IShrapnelType.Projectile => new ShrapnelEntry_Projectile(game),
                IShrapnelType.Lightning => new ShrapnelEntry_Lightning(game),
                IShrapnelType.Sound => new ShrapnelEntry_Sound(game),
                IShrapnelType.Shockwave => new ShrapnelEntry_Shockwave(game),
                IShrapnelType.Explosion => new ShrapnelEntry_Explosion(game),
                IShrapnelType.Distortion => new ShrapnelEntry_Distortion(game),
                IShrapnelType.Fire => new ShrapnelEntry_Fire(game),
                IShrapnelType.Light => new ShrapnelEntry_Light(game),
                IShrapnelType.Smoke => new ShrapnelEntry_Smoke(game),
                IShrapnelType.Goo => new ShrapnelEntry_Goo(game),
                _ => throw new ArgumentException($"Unknown shrapnel entry type: {type}"),
            };
        }

        public static List<IShrapnelType> GetValidShrpTypes(Game game)
        {
            List<IShrapnelType> result = [
                IShrapnelType.Particle,
                IShrapnelType.Projectile,
                IShrapnelType.Lightning,
                IShrapnelType.Sound,
            ];
            if (game >= Game.Incredibles)
                result.AddRange([
                    IShrapnelType.Shockwave,
                    IShrapnelType.Explosion,
                    IShrapnelType.Distortion,
                    IShrapnelType.Fire,
                ]);
            if (game >= Game.ROTU)
                result.AddRange([
                    IShrapnelType.Light,
                    IShrapnelType.Smoke,
                    IShrapnelType.Goo,
                ]);
            return result;
        }
    }
}