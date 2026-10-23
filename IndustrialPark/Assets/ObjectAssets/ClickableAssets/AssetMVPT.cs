using HipHopFile;
using Newtonsoft.Json;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public class AssetMVPT : BaseAsset, IRenderableAsset, IClickableAsset, IScalableAsset, IAssetAddSelected, IAssetCopyPasteTransformation
    {
        private const string categoryName = "Move Point";

        private Vector3 _position;
        [Category(categoryName)]
        public AssetSingle PositionX
        {
            get => _position.X;
            set { _position.X = value; CreateTransformMatrix(); }
        }
        [Category(categoryName)]
        public AssetSingle PositionY
        {
            get => _position.Y;
            set { _position.Y = value; CreateTransformMatrix(); }
        }
        [Category(categoryName)]
        public AssetSingle PositionZ
        {
            get => _position.Z;
            set { _position.Z = value; CreateTransformMatrix(); }
        }

        [Category(categoryName), Description("Allows the object to follow a curved path through this point, calculated using the previous and next points. Set to 1 to enable.")]
        public byte BezierIndex { get; set; }

        [Category(categoryName)]
        public AssetByte Flg_Props { get; set; }

        [Category(categoryName), Description("How long the object waits at this point before continuing, in seconds. NPCs may wander around the point during this time if ZoneRadius is enabled.")]
        public AssetSingle Delay { get; set; }

        private float _zoneRadius;
        [Category(categoryName), Description("For NPCs, determines the radius within which they wander randomly while waiting at this point. Set to -1 to disable. Delay must be greater than 0 for wandering to occur, and wandering might have to be turned on for certain NPC types.")]
        public AssetSingle ZoneRadius
        {
            get => _zoneRadius;
            set { _zoneRadius = value; CreateTransformMatrix(); }
        }

        private float _arenaRadius;
        [Category(categoryName), Description("Applies only to NPCs. Determines the radius within which the NPC can detect the player. Set to -1 to disable. A move point with an ArenaRadius greater than 0 can optionally be used in combination with other move points as a player-detection trigger. Turn the point off and reference it as a next move point of the other move points used by the NPC. The NPC will not move to this point, but will use its position and ArenaRadius to detect the player.")]
        public AssetSingle ArenaRadius
        {
            get => _arenaRadius;
            set { _arenaRadius = value; CreateTransformMatrix(); }
        }

        [Category(categoryName), Description("Determines whether the point starts enabled or disabled. Disabled points cannot be selected as next destinations. Use the On and Off events to change the point's state, or Reset to restore its initial state. An NPC can still start at a disabled point if it is directly referenced in the NPC asset.")]
        public bool On { get; set; }
        
        [Category(categoryName), Description("Determines the likelihood of this point being selected as the next destination from a connected point. Higher values increase the likelihood of selection. Default: 10000.")]
        public ushort Weight { get; set; }

        [Category(categoryName), ValidReferenceRequired, Editor(typeof(AssetPropertyCollectionEditor), typeof(UITypeEditor)),
            Description("The points the object can move to after reaching this point and completing its Delay. The next destination is chosen randomly from the points that are currently enabled, taking their Weight values into account. If no eligible destination is found, the object remains at the current point.")]
        public AssetID[] NextMovePoints { get; set; }

        public AssetMVPT(string assetName, Vector3 position, Game game, AssetTemplate template) : base(assetName, AssetType.MovePoint, BaseAssetType.MovePoint)
        {
            _position = position;
            Weight = 10000;
            BezierIndex = 0;
            NextMovePoints = [];

            switch (template)
            {
                case AssetTemplate.MovePoint_Arena:
                    On = false;
                    Delay = 360;
                    ZoneRadius = 4;
                    ArenaRadius = 8;
                    break;
                case AssetTemplate.MovePoint:
                    On = true;
                    Delay = game >= Game.Incredibles ? 2 : 0;
                    ZoneRadius = -1;
                    ArenaRadius = -1;
                    break;
            }

            CreateTransformMatrix();
            ArchiveEditorFunctions.AddToRenderableAssets(this);
        }

        public AssetMVPT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Weight = reader.ReadUInt16();
                On = reader.ReadByte() != 0;
                BezierIndex = reader.ReadByte();
                Flg_Props = reader.ReadByte();
                reader.ReadByte(); // pad
                ushort pointCount = reader.ReadUInt16();

                if (game != Game.Scooby)
                    Delay = reader.ReadSingle();
                _zoneRadius = reader.ReadSingle();
                if (game != Game.Scooby)
                    _arenaRadius = reader.ReadSingle();

                NextMovePoints = new AssetID[pointCount];
                for (int i = 0; i < NextMovePoints.Length; i++)
                    NextMovePoints[i] = reader.ReadUInt32();

                CreateTransformMatrix();
                ArchiveEditorFunctions.AddToRenderableAssets(this);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(Weight);
            writer.Write((byte)(On ? 1 : 0));
            writer.Write(BezierIndex);
            writer.Write(Flg_Props);
            writer.Write((byte)0);
            writer.Write((ushort)NextMovePoints.Length);
            if (game != Game.Scooby)
                writer.Write(Delay);
            writer.Write(_zoneRadius);
            if (game != Game.Scooby)
                writer.Write(_arenaRadius);
            foreach (var i in NextMovePoints)
                writer.Write(i);

            SerializeLinks(writer);
        }

        public static bool DrawAllAsPoints = false;
        private Matrix world;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        [Browsable(false)]
        public bool SpecialBlendMode => true;
        private bool DrawAsPoint => DrawAllAsPoints || _arenaRadius <= 0f;

        public void CreateTransformMatrix()
        {
            world = DrawAsPoint
                ? Matrix.RotationX(MathUtil.PiOverTwo) * Matrix.Translation(_position + new Vector3(0f, 0.5f, 0f))
                : Matrix.Scaling(_arenaRadius * 2f) * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        public BoundingSphere boundingSphere;

        protected void CreateBoundingBox()
        {
            if (DrawAsPoint)
            {
                var vertices = new Vector3[SharpRenderer.pyramidVertices.Count];

                for (int i = 0; i < SharpRenderer.pyramidVertices.Count; i++)
                    vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.pyramidVertices[i], world);

                boundingBox = BoundingBox.FromPoints(vertices);
                boundingSphere = BoundingSphere.FromBox(boundingBox);
            }
            else
            {
                boundingSphere = new BoundingSphere(_position, _arenaRadius);
                boundingBox = BoundingBox.FromSphere(boundingSphere);
            }
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ShouldDraw(renderer))
                if (ray.Intersects(ref boundingSphere))
                {
                    return DrawAsPoint
                        ? TriangleIntersection(ray, SharpRenderer.pyramidTriangles, SharpRenderer.pyramidVertices, world)
                        : TriangleIntersection(ray, SharpRenderer.sphereTriangles, SharpRenderer.sphereVertices, world);
                }
            return null;
        }

        public bool ShouldDraw(SharpRenderer renderer)
        {
            if (isSelected)
                return true;
            if (dontRender)
                return false;
            if (isInvisible)
                return false;
            if (AssetMODL.renderBasedOnLodt && GetDistanceFrom(renderer.Camera.Position) > SharpRenderer.DefaultLODTDistance)
                return false;

            return renderer.frustum.Intersects(ref boundingBox);
        }

        public void Draw(SharpRenderer renderer)
        {
            if (DrawAsPoint)
                renderer.DrawPyramid(world, isSelected, 1f, renderer.mvptColor);
            else
                renderer.DrawSphere(world, isSelected, renderer.mvptColor);
        }

        public BoundingBox GetBoundingBox() => boundingBox;

        public float GetDistanceFrom(Vector3 cameraPosition) => Vector3.Distance(cameraPosition, _position) - Math.Max(0f, _arenaRadius);

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
            {
                dt.RemoveProperty("Delay");
                dt.RemoveProperty("ArenaRadius");
            }
            base.SetDynamicProperties(dt);
        }

        [Browsable(false)]
        public AssetSingle ScaleX
        {
            get => GetScale();
            set => SetScale(value);
        }
        [Browsable(false)]
        public AssetSingle ScaleY
        {
            get => GetScale();
            set => SetScale(value);
        }
        [Browsable(false)]
        public AssetSingle ScaleZ
        {
            get => GetScale();
            set => SetScale(value);
        }

        private float GetScale()
        {
            if (DrawAsPoint)
                return _arenaRadius;

            return 1f;
        }

        private void SetScale(float scale)
        {
            if (DrawAsPoint)
                ArenaRadius = scale;
        }

        [Browsable(false)]
        public string GetItemsText => "next";

        public void AddItems(List<uint> newItems)
        {
            var items = NextMovePoints.ToList();
            foreach (uint i in newItems)
                if (!items.Contains(i))
                    items.Add(i);
            NextMovePoints = items.ToArray();
        }

        public void CopyTransformation()
        {
            var transformation = new Transformation()
            {
                _positionX = _position.X,
                _positionY = _position.Y,
                _positionZ = _position.Z,
                _scaleSingle = GetScale(),
                _scaleSfxMin = GetScale(),
                _scaleSfxMax = GetScale(),
            };
            Clipboard.SetText(JsonConvert.SerializeObject(transformation));
        }

        public void PasteTransformation()
        {
            try
            {
                var transformation = JsonConvert.DeserializeObject<Transformation>(Clipboard.GetText());
                _position.X = transformation._positionX;
                _position.Y = transformation._positionY;
                _position.Z = transformation._positionZ;
                SetScale(transformation._scaleSingle);

                CreateTransformMatrix();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"There was an error pasting the transformation from clipboard: ${ex.Message}. Are you sure you have a transformation copied?");
            }
        }
    }
}