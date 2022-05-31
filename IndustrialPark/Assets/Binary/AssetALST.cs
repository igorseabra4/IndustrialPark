using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetALST : Asset
    {
        [Category("Animation List")]
        public AssetID[] Animations { get; set; }

        public AssetALST(string assetName) : base(assetName, AssetType.AnimationList)
        {
            Animations = new AssetID[10];
        }

        public AssetALST(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                var assetIDs = new List<AssetID>();
                while (!reader.EndOfStream)
                    assetIDs.Add(reader.ReadUInt32());
                Animations = assetIDs.ToArray();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                foreach (var i in Animations)
                    writer.Write(i);
                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            if (Animations.Length != 10)
                result.Add("ALST asset has a number of animation asset IDs different from 10");

            foreach (AssetID assetID in Animations)
                Verify(assetID, ref result);
        }
    }
}