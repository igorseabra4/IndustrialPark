using SharpDX;
using System;

namespace IndustrialPark
{
    public class SharpCamera
    {
        public const float DEFAULT_FOV = 60f;
        public const float DEFAULT_FAR_PLANE = 10000f;
        public const float DEFAULT_NEAR_PLANE = 0.1f;

        public Vector3 Position { get; private set; } = Vector3.Zero;
        public Vector3 Forward { get; private set; } = Vector3.ForwardRH;
        public Vector3 Right { get; private set; } = Vector3.Right;
        public Vector3 Up { get; private set; } = Vector3.Up;
        private float _yaw = 0;
        public float Yaw
        {
            get => _yaw;
            set
            {
                _yaw = value;
                Update();
                RaiseCameraChangedEvent();
            }
        }
        private float _pitch = 0;
        public float Pitch
        {
            get => _pitch;
            set
            {
                _pitch = value;
                Update();
                RaiseCameraChangedEvent();
            }
        }
        private float _roll = 0;
        public float Roll
        {
            get => _roll;
            set
            {
                _roll = value;
                Update();
                RaiseCameraChangedEvent();
            }
        }

        public float Speed
        {
            get;
            set
            {
                field = Math.Max(0.0001f, value);
            }
        } = DEFAULT_SPEED;
        public float SpeedRot
        {
            get;
            set
            {
                field = Math.Max(0.0001f, value);
            }
        } = DEFAULT_SPEED_ROT;

        public const float DEFAULT_SPEED = 1f;
        public const float DEFAULT_SPEED_ROT = 30f;
        public const float DEFAULT_SPEED_INCREMENT = 0.01f;
        public const float DEFAULT_SPEED_ROT_INCREMENT = 0.1f;
        public const float DEFAULT_MOUSE_SPEED_MULT = 1f / 6f;

        public float FieldOfView;
        public float AspectRatio;
        public float NearPlane;
        public float FarPlane;

        public event CameraChangedDelegate CameraChangedEvent;
        public delegate void CameraChangedDelegate(SharpCamera camera);
        public void RaiseCameraChangedEvent()
        {
            CameraChangedEvent?.Invoke(this);
        }

        internal void AddPositionForward(float factor)
        {
            Position += Forward * Speed * factor;
        }

        internal void AddPositionSideways(float factor)
        {
            Position -= Right * Speed * factor;
        }

        internal void AddPositionUp(float factor)
        {
            Position += Up * Speed * factor;
        }

        internal void AddYaw(float factor)
        {
            _yaw -= SpeedRot * factor / 60f;
        }

        internal void AddPitch(float factor)
        {
            _pitch -= SpeedRot * factor / 60f;
        }

        internal void AddRoll(float factor)
        {
            _roll -= SpeedRot * factor / 60f;
        }

        internal void ResetRoll()
        {
            _roll = 0;
        }

        internal void IncreaseCameraSpeed(float v)
        {
            Speed += v;
        }

        internal void IncreaseCameraRotationSpeed(float v)
        {
            SpeedRot += v;
        }

        public void Update()
        {
            _pitch %= 360;
            _yaw %= 360;
            _roll %= 360;

            Matrix rotation = Matrix.RotationYawPitchRoll(
                MathUtil.DegreesToRadians(Yaw),
                MathUtil.DegreesToRadians(Pitch),
                MathUtil.DegreesToRadians(Roll));

            Forward = Vector3.Normalize((Vector3)Vector3.Transform(Vector3.ForwardRH, rotation));
            Right = Vector3.Normalize((Vector3)Vector3.Transform(Vector3.Right, rotation));
            Up = Vector3.Normalize((Vector3)Vector3.Transform(Vector3.Up, rotation));
        }

        internal void Reset()
        {
            Position = Vector3.Zero;
            Forward = Vector3.ForwardRH;
            Right = Vector3.Right;
            Up = Vector3.Up;
            _yaw = 0;
            _pitch = 0;
            _roll = 0;

            FieldOfView = MathUtil.Pi / 3f;
            NearPlane = DEFAULT_NEAR_PLANE;
            FarPlane = DEFAULT_FAR_PLANE;

            Update();
        }

        public void SetPosition(Vector3 Position)
        {
            this.Position = Position;
            Update();
        }

        public Matrix ViewMatrix => Matrix.LookAtRH(Position, Position + Forward, Up);

        public Matrix ProjectionMatrix => Matrix.PerspectiveFovRH(FieldOfView, AspectRatio, NearPlane, FarPlane);

        public Matrix BiggerFovProjectionMatrix => Matrix.PerspectiveFovRH(1.3f * FieldOfView, AspectRatio, NearPlane, FarPlane);

        public override string ToString() => string.Format("Position: [{0:0.0000}, {1:0.0000}, {2:0.0000}] Rotation: [{3:0.0000}, {4:0.0000}, {5:0.0000}]",
                Position.X, Position.Y, Position.Z, Yaw, Pitch, Roll);

        private void SetCameraRotationFromVectors()
        {
            _pitch = MathUtil.RadiansToDegrees((float)Math.Asin(Math.Clamp(Forward.Y, -1f, 1f)));
            _yaw = 270f - MathUtil.RadiansToDegrees((float)Math.Atan2(Forward.Z, Forward.X));
            _yaw %= 360f;
            Matrix yawPitch = Matrix.RotationYawPitchRoll(
                MathUtil.DegreesToRadians(_yaw),
                MathUtil.DegreesToRadians(_pitch),
                0f);
            Vector3 expectedUp = (Vector3)Vector3.Transform(Vector3.Up, yawPitch);
            _roll = MathUtil.RadiansToDegrees(
                (float)Math.Atan2(
                    Vector3.Dot(Forward, Vector3.Cross(expectedUp, Up)),
                    Vector3.Dot(expectedUp, Up)));
            _roll %= 360f;
        }

        public void SetPositionCamera(AssetCAM cam)
        {
            Position = new Vector3(cam.PositionX, cam.PositionY, cam.PositionZ);
            Forward = new Vector3(cam.NormalizedForwardX, cam.NormalizedForwardY, cam.NormalizedForwardZ);
            Up = new Vector3(cam.NormalizedUpX, cam.NormalizedUpY, cam.NormalizedUpZ);
            Right = Vector3.Normalize(Vector3.Cross(Forward, Up));
            SetCameraRotationFromVectors();
            RaiseCameraChangedEvent();
        }

        public void SetPositionFlyEntry(FlyFrame cam)
        {
            Position = cam.CameraPosition;
            Forward = -cam.CameraNormalizedBackward;
            Up = cam.CameraNormalizedUp;
            Right = -cam.CameraNormalizedRight;
            SetCameraRotationFromVectors();
            RaiseCameraChangedEvent();
        }

        public void ShowAssetOnViewport(IClickableAsset asset)
        {
            if (asset is AssetCAM cam)
                SetPositionCamera(cam);
            else
                SetPosition(asset.GetBoundingBox().Center - (10 + asset.GetBoundingBox().Size) * Forward);
        }
    }
}