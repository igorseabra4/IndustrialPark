using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace IndustrialPark
{
    public class AssetSHRP : Asset
    {
        public override string AssetInfo => $"{Entries.Length} entries";

        private const string categoryName = "Shrapnel";

        [Category(categoryName)]
        public int Unknown { get; set; }
        [Category(categoryName), Editor(typeof(DynamicTypeDescriptorCollectionEditor), typeof(UITypeEditor))]
        public Shrapnel[] Entries { get; set; }

        public AssetSHRP(string assetName) : base(assetName, AssetType.Shrapnel)
        {
            Entries = new Shrapnel[0];
        }

        public AssetSHRP(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                int count = reader.ReadInt32();
                reader.ReadUInt32();
                Unknown = reader.ReadInt32();
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
                        default:
                            throw new Exception($"Unknown shrapnel entry type: {entryType}");
                    }
                }
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Entries.Length);
            writer.Write(assetID);
            writer.Write(Unknown);
            foreach (var e in Entries)
                e.Serialize(writer);
        }

        public void AddEntry(IShrapnelType type)
        {
            List<Shrapnel> list = Entries.ToList();

            switch (type)
            {
                case IShrapnelType.Particle:
                    list.Add(new ShrapnelEntry_Particle(game));
                    break;
                case IShrapnelType.Projectile:
                    list.Add(new ShrapnelEntry_Projectile(game));
                    break;
                case IShrapnelType.Lightning:
                    list.Add(new ShrapnelEntry_Lightning(game));
                    break;
                case IShrapnelType.Sound:
                    list.Add(new ShrapnelEntry_Sound(game));
                    break;
                case IShrapnelType.Shockwave:
                    list.Add(new ShrapnelEntry_Shockwave(game));
                    break;
                case IShrapnelType.Explosion:
                    list.Add(new ShrapnelEntry_Explosion(game));
                    break;
                case IShrapnelType.Distortion:
                    list.Add(new ShrapnelEntry_Distortion(game));
                    break;
                case IShrapnelType.Fire:
                    list.Add(new ShrapnelEntry_Fire(game));
                    break;
                default:
                    throw new ArgumentException($"Unknown shrapnel entry type: {type}");
            }

            Entries = list.ToArray();
        }
    }
}