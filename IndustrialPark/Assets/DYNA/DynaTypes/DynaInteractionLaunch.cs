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
        public AssetID LaunchObject_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Target_AssetID { get; set; }
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
                LaunchObject_AssetID = reader.ReadUInt32();
                Target_AssetID = reader.ReadUInt32();
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
                writer.Write(LaunchObject_AssetID);
                writer.Write(Target_AssetID);
                writer.Write(Gravity);
                writer.Write(Height);
                writer.Write(LeavesBone);
                writer.Write(LaunchFlags.FlagValueInt);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID) =>
            LaunchObject_AssetID == assetID || Target_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            Verify(LaunchObject_AssetID, ref result);
            Verify(Target_AssetID, ref result);
            base.Verify(ref result);
        }

    }
}