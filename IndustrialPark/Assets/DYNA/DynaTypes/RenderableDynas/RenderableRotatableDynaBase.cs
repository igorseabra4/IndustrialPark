using HipHopFile;
using Newtonsoft.Json;
using SharpDX;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace IndustrialPark
{
    public abstract class RenderableRotatableDynaBase : RenderableDynaBase, IRotatableAsset
    {
        private const string dynaCategoryName = "DYNA Placement";

        protected virtual bool inRadians => true;

        protected float _yaw;
        [Category(dynaCategoryName)]
        public AssetSingle Yaw
        {
            get => (inRadians) ? MathUtil.RadiansToDegrees(_yaw) : _yaw;
            set
            {
                if (inRadians)
                    _yaw = MathUtil.DegreesToRadians(value);
                else
                    _yaw = value;
                CreateTransformMatrix();
            }
        }

        protected float _pitch;
        [Category(dynaCategoryName)]
        public AssetSingle Pitch
        {
            get => (inRadians) ? MathUtil.RadiansToDegrees(_pitch) : _pitch;
            set
            {
                if (inRadians)
                    _pitch = MathUtil.DegreesToRadians(value);
                else
                    _pitch = value;
                CreateTransformMatrix();
            }
        }

        protected float _roll;
        [Category(dynaCategoryName)]
        public AssetSingle Roll
        {
            get => (inRadians) ? MathUtil.RadiansToDegrees(_roll) : _roll;
            set
            {
                if (inRadians)
                    _roll = MathUtil.DegreesToRadians(value);
                else
                    _roll = value;
                CreateTransformMatrix();
            }
        }

        public RenderableRotatableDynaBase(string assetName, DynaType dynaType, Vector3 position) : base(assetName, dynaType, position) { }
        public RenderableRotatableDynaBase(Section_AHDR AHDR, DynaType type, Game game, Endianness endianness) : base(AHDR, type, game, endianness) { }

        public override void CreateTransformMatrix()
        {
            if (inRadians)
                world = Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll) * Matrix.Translation(_position);
            else
                world = Matrix.RotationYawPitchRoll(
                    MathUtil.DegreesToRadians(_yaw),
                    MathUtil.DegreesToRadians(_pitch),
                    MathUtil.DegreesToRadians(_roll)) * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        public override void CopyTransformation()
        {
            var transformation = new Transformation()
            {
                _positionX = _position.X,
                _positionY = _position.Y,
                _positionZ = _position.Z,
                _yaw = _yaw,
                _pitch = _pitch,
                _roll = _roll,
            };
            Clipboard.SetText(JsonConvert.SerializeObject(transformation));
        }

        public override void PasteTransformation()
        {
            try
            {
                var transformation = JsonConvert.DeserializeObject<Transformation>(Clipboard.GetText());
                _position.X = transformation._positionX;
                _position.Y = transformation._positionY;
                _position.Z = transformation._positionZ;
                _yaw = transformation._yaw;
                _pitch = transformation._pitch;
                _roll = transformation._roll;
                CreateTransformMatrix();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"There was an error pasting the transformation from clipboard: ${ex.Message}. Are you sure you have a transformation copied?");
            }
        }
    }
}