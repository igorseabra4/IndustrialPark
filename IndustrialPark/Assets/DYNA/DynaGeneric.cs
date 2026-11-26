using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark.Assets;

public class DynaGeneric : AssetDYNA
{
    private const string dynaCategoryName = "Generic Dynamic";

    private Endianness endianness;

    public DynaGeneric(Section_AHDR AHDR, DynaType type, EndianBinaryReader reader) : base(AHDR, type, reader)
    {
        int len = AHDR.data.Length > 0 ? AHDR.data.Length : (int)reader.BaseStream.Length;
        Data = reader.ReadBytes(len - 16 - _links.Length * Link.sizeOfStruct);
        this.endianness = reader.endianness;
    }

    [Category(dynaCategoryName)]
    public byte[] Data { get; set; }

    [Category(dynaCategoryName), Editor(typeof(AssetPropertyCollectionEditor), typeof(UITypeEditor))]
    public AssetID[] Data_AsHex
    {
        get
        {
            var values = new List<AssetID>();
            using var reader = new EndianBinaryReader(Data, endianness);
            while (!reader.EndOfStream)
                values.Add(reader.ReadUInt32());
            return values.ToArray();
        }
        set
        {
            using var writer = new EndianBinaryWriter(endianness);
            foreach (var f in value)
                writer.Write(f);
            Data = writer.ToArray();
        }
    }

    [Category(dynaCategoryName), Editor(typeof(AssetPropertyCollectionEditor), typeof(UITypeEditor))]
    public AssetSingle[] Data_AsFloat
    {
        get
        {
            var values = new List<AssetSingle>();
            using var reader = new EndianBinaryReader(Data, endianness);
            while (!reader.EndOfStream)
                values.Add(reader.ReadSingle());
            return values.ToArray();
        }
        set
        {
            using var writer = new EndianBinaryWriter(endianness);
            foreach (var f in value)
                writer.Write(f);
            Data = writer.ToArray();
        }
    }

    protected override void SerializeDyna(EndianBinaryWriter writer)
    {
        writer.Write(Data);
    }
}