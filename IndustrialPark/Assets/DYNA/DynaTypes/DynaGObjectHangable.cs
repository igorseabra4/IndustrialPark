using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectHangable : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:Hangable";

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID Object { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle PivotX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle PivotY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle PivotZ { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle HandleX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle HandleY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle HandleZ { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OnGravity { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OffGravity { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle MaxAngVel { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle MinArcDegrees { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask HangFlags { get; set; } = IntFlagsDescriptor();

        public DynaGObjectHangable(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__Hangable, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Object = reader.ReadUInt32();
                PivotX = reader.ReadSingle();
                PivotY = reader.ReadSingle();
                PivotZ = reader.ReadSingle();
                HandleX = reader.ReadSingle();
                HandleY = reader.ReadSingle();
                HandleZ = reader.ReadSingle();
                OnGravity = reader.ReadSingle();
                OffGravity = reader.ReadSingle();
                MaxAngVel = reader.ReadSingle();
                MinArcDegrees = reader.ReadSingle();
                HangFlags.FlagValueInt = reader.ReadUInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Object);
                writer.Write(PivotX);
                writer.Write(PivotY);
                writer.Write(PivotZ);
                writer.Write(HandleX);
                writer.Write(HandleY);
                writer.Write(HandleZ);
                writer.Write(OnGravity);
                writer.Write(OffGravity);
                writer.Write(MaxAngVel);
                writer.Write(MinArcDegrees);
                writer.Write(HangFlags.FlagValueInt);

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            Verify(Object, ref result);
            base.Verify(ref result);
        }
    }
}