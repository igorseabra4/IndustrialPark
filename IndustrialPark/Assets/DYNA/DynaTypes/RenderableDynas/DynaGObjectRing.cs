using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark.Assets;

public class DynaGObjectRing : RenderableRotatableDynaBase
{
    private const string dynaCategoryName = "game_object:Ring";
    public override string TypeString => dynaCategoryName;

    protected override short constVersion => 2;

    [Category(dynaCategoryName)]
    public int OffsetX { get; set; }
    [Category(dynaCategoryName)]
    public int OffsetY { get; set; }
    [Category(dynaCategoryName)]
    public int OffsetZ { get; set; }
    protected Vector3 _scale;
    [Category(dynaCategoryName)]
    public AssetSingle ScaleX
    {
        get => _scale.X;
        set => _scale.X = value;
    }
    [Category(dynaCategoryName)]
    public AssetSingle ScaleY
    {
        get => _scale.Y;
        set => _scale.Y = value;
    }
    [Category(dynaCategoryName)]
    public AssetSingle ScaleZ
    {
        get => _scale.Z;
        set => _scale.Z = value;
    }
    [Category(dynaCategoryName), Description("0 = half size shadow, 1 = full size shadow")]
    public int TriggerBoundsType { get; set; }
    [Category(dynaCategoryName)]
    public AssetSingle Radius { get; set; }
    [Category(dynaCategoryName)]
    public AssetSingle Width { get; set; }
    [Category(dynaCategoryName)]
    public AssetSingle Height { get; set; }
    [Category(dynaCategoryName)]
    public AssetSingle NormalTimer { get; set; }
    [Category(dynaCategoryName)]
    public AssetSingle RedTimer { get; set; }
    [Category(dynaCategoryName)]
    public AssetID DriverPlatform { get; set; }

    public static bool dontRender = false;
    protected override bool DontRender => dontRender;

    public DynaGObjectRing(string assetName, Vector3 position) : base(assetName, DynaType.game_object__Ring, position)
    {
        ScaleX = 1f;
        ScaleY = 1f;
        ScaleZ = 1f;
        TriggerBoundsType = 1;
        Radius = 3.5f;
        Width = 4f;
        Height = 4f;
        NormalTimer = 5f;
        RedTimer = -1f;
    }

    public DynaGObjectRing(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.game_object__Ring, reader)
    {
        _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        _yaw = reader.ReadSingle();
        _pitch = reader.ReadSingle();
        _roll = reader.ReadSingle();
        OffsetX = reader.ReadInt32();
        OffsetY = reader.ReadInt32();
        OffsetZ = reader.ReadInt32();
        _scale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        TriggerBoundsType = reader.ReadInt32();
        Radius = reader.ReadSingle();
        Width = reader.ReadSingle();
        Height = reader.ReadSingle();
        NormalTimer = reader.ReadSingle();
        RedTimer = reader.ReadSingle();
        DriverPlatform = reader.ReadUInt32();
    }

    protected override void SerializeDyna(EndianBinaryWriter writer)
    {
        writer.Write(_position.X);
        writer.Write(_position.Y);
        writer.Write(_position.Z);
        writer.Write(_yaw);
        writer.Write(_pitch);
        writer.Write(_roll);
        writer.Write(OffsetX);
        writer.Write(OffsetY);
        writer.Write(OffsetZ);
        writer.Write(_scale.X);
        writer.Write(_scale.Y);
        writer.Write(_scale.Z);
        writer.Write(TriggerBoundsType);
        writer.Write(Radius);
        writer.Write(Width);
        writer.Write(Height);
        writer.Write(NormalTimer);
        writer.Write(RedTimer);
        writer.Write(DriverPlatform);
    }

    public override void CalculateTransformation(SharpRenderer renderer)
    {
        world = ((renderer.Assets.GetFromRenderingDictionary(DynaGObjectRingControl.RingModelAssetID) is IAssetWithModel value) ? value.TransformMatrix : Matrix.Identity)
            * Matrix.Scaling(_scale)
            * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
            * Matrix.Translation(_position);

        boundingBox = CreateBoundingBox(vertexSource(renderer), world);
        Reset();
    }

    protected override List<Vector3> vertexSource(SharpRenderer renderer)
    {
        var model = renderer.Assets.GetModelFromRenderingDictionary(DynaGObjectRingControl.RingModelAssetID);
        return model?.vertexListG ?? renderer.cubeVertices;
    }

    protected override List<RenderWareFile.Triangle> triangleSource(SharpRenderer renderer)
    {
        var model = renderer.Assets.GetModelFromRenderingDictionary(DynaGObjectRingControl.RingModelAssetID);
        return model?.triangleList ?? renderer.cubeTriangles;
    }

    public override bool ShouldDraw(SharpRenderer renderer)
    {
        if (isSelected)
            return true;
        if (isInvisible)
            return false;
        if (dontRender)
            return false;

        if (AssetMODL.renderBasedOnLodt && GetDistanceFrom(renderer.Camera.Position) > AssetLODT.MaxDistanceTo(DynaGObjectRingControl.RingModelAssetID))
            return false;

        return renderer.frustum.Intersects(ref boundingBox);
    }

    public override void Draw(SharpRenderer renderer)
    {
        var world = LocalWorld();

        if (renderer.Assets.GetFromRenderingDictionary(DynaGObjectRingControl.RingModelAssetID) is IAssetWithModel value)
            value.Draw(renderer, world, Vector4.One, Vector3.Zero, isSelected);
        else
            renderer.DrawCube(world, isSelected);
    }

    public void Reset()
    {
        driver = DriverPlatform != 0 &&
            Program.ArchiveManager.ArchiveEditorContainingAsset(DriverPlatform) is ArchiveEditors.ArchiveEditor ae &&
            ae.archive.GetAsset(DriverPlatform) is EntityAsset entity ?
            entity : null;
    }

    protected EntityAsset driver = null;

    private Matrix LocalWorld()
    {
        if (EntityAsset.movementPreview && driver != null)
        {
            return Matrix.Scaling(_scale)
                * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                * Matrix.Translation(_position - new Vector3(driver.PositionX, driver.PositionY, driver.PositionZ))
                * driver.PlatLocalRotation()
                * Matrix.Translation(Vector3.TransformCoordinate(Vector3.Zero, driver.LocalWorld()));
        }

        return world;
    }
}