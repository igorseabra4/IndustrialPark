using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark.Assets;

public class DynaEffectScreenWarp : RenderableDynaBase
{
    private const string dynaCategoryName = "effect:ScreenWarp";
    public override string TypeString => dynaCategoryName;
    protected override short constVersion => 1;

    [Category(dynaCategoryName)]
    public uint EffectType { get; set; }
    [Category(dynaCategoryName)]
    public FlagBitmask Flags { get; set; } = IntFlagsDescriptor(
        "Enabled",
        "Running");

    protected float _radius;
    [Category("Placement")]
    public AssetSingle Radius
    {
        get => _radius;
        set => _radius = value;
    }

    [Category(dynaCategoryName)]
    public AssetSingle Duration { get; set; }
    [Category(dynaCategoryName)]
    public AssetSingle RepeatDelay { get; set; }
    [Category(dynaCategoryName)]
    public AssetSingle Intensity { get; set; }
    [Category(dynaCategoryName)]
    public AssetSingle Frequency { get; set; }
    [Category(dynaCategoryName)]
    public AssetSingle TriggerDistance { get; set; }

    public DynaEffectScreenWarp(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.effect__ScreenWarp, reader)
    {
        EffectType = reader.ReadUInt32();
        Flags.FlagValueInt = reader.ReadUInt32();
        _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        _radius = reader.ReadSingle();
        Duration = reader.ReadSingle();
        RepeatDelay = reader.ReadSingle();
        Intensity = reader.ReadSingle();
        Frequency = reader.ReadSingle();
        TriggerDistance = reader.ReadSingle();
    }

    protected override void SerializeDyna(EndianBinaryWriter writer)
    {
        writer.Write(EffectType);
        writer.Write(Flags.FlagValueInt);
        writer.Write(_position.X);
        writer.Write(_position.Y);
        writer.Write(_position.Z);
        writer.Write(_radius);
        writer.Write(Duration);
        writer.Write(RepeatDelay);
        writer.Write(Intensity);
        writer.Write(Frequency);
        writer.Write(TriggerDistance);
    }

    protected override List<Vector3> vertexSource(SharpRenderer renderer) => renderer.sphereVertices;

    protected override List<RenderWareFile.Triangle> triangleSource(SharpRenderer renderer) => renderer.sphereTriangles;

    private BoundingSphere boundingSphere;

    public override void CalculateTransformation(SharpRenderer renderer)
    {
        world = Matrix.Scaling(_radius * 2f) * Matrix.Translation(_position);
        boundingSphere = new BoundingSphere(new Vector3(PositionX, PositionY, PositionZ), Radius);
        boundingBox = BoundingBox.FromSphere(boundingSphere);
    }

    public override void Draw(SharpRenderer renderer) => renderer.DrawSphere(boundingSphere, isSelected, renderer.trigColor);

    public static bool dontRender = false;
    protected override bool DontRender => dontRender;
}