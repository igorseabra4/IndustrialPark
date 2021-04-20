using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetALST : Asset
    {
        [Category("Animation List")]
        public AssetID[] Animation_AssetIDs { get; set; }

        public AssetALST(string assetName) : base(assetName, AssetType.ALST)
        {
            Animation_AssetIDs = new AssetID[10];
        }

        public AssetALST(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);

            var assetIDs = new List<AssetID>();
            while (!reader.EndOfStream)
                assetIDs.Add(reader.ReadUInt32());
            Animation_AssetIDs = assetIDs.ToArray();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            foreach (var i in Animation_AssetIDs)
                writer.Write(i);
            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            foreach (var a in Animation_AssetIDs)
                if (a == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            if (Animation_AssetIDs.Length != 10)
                result.Add("ALST asset has a number of animation asset IDs different from 10");

            foreach(AssetID assetID in Animation_AssetIDs)
                Verify(assetID, ref result);
        }
    }
}