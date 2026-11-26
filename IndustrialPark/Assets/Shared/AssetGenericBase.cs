using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark.Assets;

public class AssetGenericBase : BaseAsset
{
    private const string categoryName = "Generic Base Asset";

    private Endianness endianness;

    [Category(categoryName)]
    public byte[] Data { get; set; }

    public AssetGenericBase(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, reader)
    {
        int len = AHDR.data.Length > 0 ? AHDR.data.Length : (int)reader.BaseStream.Length;
        Data = reader.ReadBytes(len - 8 - _links.Length * Link.sizeOfStruct);
        this.endianness = reader.endianness;
    }

    public override void Serialize(EndianBinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(Data);
        SerializeLinks(writer);
    }

    [Category(categoryName), Editor(typeof(AssetPropertyCollectionEditor), typeof(UITypeEditor))]
    public AssetID[] Data_AsHex
    {
        get
        {
            var values = new List<AssetID>();
            var reader = new EndianBinaryReader(Data, endianness);
            while (!reader.EndOfStream)
                values.Add(reader.ReadUInt32());
            return values.ToArray();
        }
        set
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                foreach (var f in value)
                    writer.Write(f);
                Data = writer.ToArray();
            }
        }
    }

    [Category(categoryName), Editor(typeof(AssetPropertyCollectionEditor), typeof(UITypeEditor))]
    public AssetSingle[] Data_AsFloat
    {
        get
        {
            var values = new List<AssetSingle>();
            var reader = new EndianBinaryReader(Data, endianness);
            while (!reader.EndOfStream)
                values.Add(reader.ReadSingle());
            return values.ToArray();
        }
        set
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                foreach (var f in value)
                    writer.Write(f);
                Data = writer.ToArray();
            }
        }
    }
}