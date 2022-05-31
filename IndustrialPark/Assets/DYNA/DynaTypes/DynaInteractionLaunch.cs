using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaInteractionLaunch : AssetDYNA
    {
        private const string dynaCategoryName = "interaction:Launch";

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public int LaunchType { get; set; }
        [Category(dynaCategoryName)]
        public AssetID LaunchObject { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Target { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Gravity { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Height { get; set; }
        [Category(dynaCategoryName)]
        public int LeavesBone { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask LaunchFlags { get; set; } = IntFlagsDescriptor();

        public DynaInteractionLaunch(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.interaction__Launch, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                LaunchType = reader.ReadInt32();
                LaunchObject = reader.ReadUInt32();
                Target = reader.ReadUInt32();
                Gravity = reader.ReadSingle();
                Height = reader.ReadSingle();
                LeavesBone = reader.ReadInt32();
                LaunchFlags.FlagValueInt = reader.ReadUInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(LaunchType);
                writer.Write(LaunchObject);
                writer.Write(Target);
                writer.Write(Gravity);
                writer.Write(Height);
                writer.Write(LeavesBone);
                writer.Write(LaunchFlags.FlagValueInt);

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            Verify(LaunchObject, ref result);
            Verify(Target, ref result);
            base.Verify(ref result);
        }

    }
}