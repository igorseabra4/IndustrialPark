using HipHopFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class AssetSPLP : BaseAsset
    {
        public override string AssetInfo => $"{HexUIntTypeConverter.StringFromAssetID(SplineID)}";

        public bool Exclusive { get; set; }
        public AssetByte Used { get; set; }
        public bool HasHover { get; set; }  
        public AssetSingle Speed { get; set; }
        public AssetSingle HoverTime { get; set; }
        public AssetSingle HoverPoint_X { get; set; }
        public AssetSingle HoverPoint_Y { get; set; }
        public AssetSingle HoverPoint_Z { get; set; }
        public AssetID SplineID { get; set; }
        public AssetID ForwardPath { get; set; }
        public AssetID BackwardsPath { get; set; }
        public AssetID[] ForwardIDs { get; set; }
        public AssetID[] BackwardsIDs { get; set; }

        public AssetSPLP(string assetName) : base(assetName, AssetType.SplinePath, BaseAssetType.SplinePath)
        {
            ForwardIDs = new AssetID[0];
            BackwardsIDs = new AssetID[0];
        }

        public AssetSPLP(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;
                Exclusive = reader.ReadByteBool();
                Used = reader.ReadByte();
                HasHover = reader.ReadByteBool();
                reader.ReadByte();
                short forwardCount = reader.ReadInt16();
                short backwardCount = reader.ReadInt16();
                Speed = reader.ReadSingle();
                HoverTime = reader.ReadSingle();

                reader.endianness = Endianness.Little;
                HoverPoint_X = reader.ReadSingle();
                HoverPoint_Y = reader.ReadSingle();
                HoverPoint_Z = reader.ReadSingle();
                reader.endianness = endianness;

                SplineID = reader.ReadUInt32();

                ForwardPath = reader.ReadUInt32();
                BackwardsPath = reader.ReadUInt32();

                ForwardIDs = new AssetID[forwardCount];
                for (int i = 0; i < forwardCount; i++)
                    ForwardIDs[i] = reader.ReadUInt32();

                BackwardsIDs = new AssetID[backwardCount];
                for (int i = 0; i < backwardCount; i++)
                    BackwardsIDs[i] = reader.ReadUInt32();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Exclusive);
            writer.Write(Used);
            writer.Write(HasHover);
            writer.Write((byte)0);
            writer.Write((short)ForwardIDs.Length);
            writer.Write((short)BackwardsIDs.Length);
            writer.Write(Speed);
            writer.Write(HoverTime);

            Endianness oldEndian = writer.endianness;
            writer.endianness = Endianness.Little;
            writer.Write(HoverPoint_X);
            writer.Write(HoverPoint_Y);
            writer.Write(HoverPoint_Z);
            writer.endianness = oldEndian;

            writer.Write(SplineID);
            writer.Write(ForwardPath);
            writer.Write(BackwardsPath);

            foreach (AssetID forid in ForwardIDs)
                writer.Write(forid);
            foreach (AssetID backid in BackwardsIDs)
                writer.Write(backid);
        }
    }
}
