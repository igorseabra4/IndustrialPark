using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark.Assets;

public class AssetGeneric : Asset
{
    private const string categoryName = "Generic Asset";

    private Endianness endianness;

    [Category(categoryName)]
    public byte[] Data { get; set; }

    public AssetGeneric(string assetName, AssetType assetType, byte[] data) : base(assetName, assetType)
    {
        Data = data;
    }

    public AssetGeneric(Section_AHDR AHDR) : base(AHDR)
    {
        Data = AHDR.data;
        endianness = AHDR.Platform.Endianness();
    }

    public override void Serialize(EndianBinaryWriter writer)
    {
        writer.Write(Data);
    }

    [Category(categoryName)]
    public AssetID[] Data_AsHex
    {
        get
        {
            try
            {
                var values = new List<AssetID>();
                using var reader = new EndianBinaryReader(Data, endianness);
                while (!reader.EndOfStream)
                    values.Add(reader.ReadUInt32());
                return values.ToArray();
            }
            catch { return []; }
        }
        set
        {
            using var writer = new EndianBinaryWriter(endianness);
            foreach (var f in value)
                writer.Write(f);
            Data = writer.ToArray();
        }
    }

    [Category(categoryName)]
    public AssetSingle[] Data_AsFloat
    {
        get
        {
            try
            {
                var values = new List<AssetSingle>();
                using var reader = new EndianBinaryReader(Data, endianness);
                while (!reader.EndOfStream)
                    values.Add(reader.ReadSingle());
                return values.ToArray();
            }
            catch { return []; }
        }
        set
        {
            using var writer = new EndianBinaryWriter(endianness);
            foreach (var f in value)
                writer.Write(f);
            Data = writer.ToArray();
        }
    }
}