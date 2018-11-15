using SharpDX;
using System;

namespace IndustrialPark
{
    public class SharpCamera
    {
        public Vector3 Position = Vector3.Zero;
        private Vector3 Forward = Vector3.ForwardRH;
        private Vector3 Right = Vector3.Right;
        private Vector3 Up = Vector3.Up;
        private float _yaw = 0;
        public float Yaw
        {
            get => _yaw;
            set
            {
                _yaw = value;
                UpdateCamera();
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
                UpdateCamera();
                RaiseCameraChangedEvent();
            }
        }
        public float Speed;
        public float SpeedRot;

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
            UpdateCamera();
        }

        internal void AddPositionSideways(float factor)
        {
            Position -= Right * Speed * factor;
            UpdateCamera();
        }

        internal void AddPositionUp(float factor)
        {
            Position += Up * Speed * factor;
            UpdateCamera();
        }

        internal void AddYaw(float factor)
        {
            _yaw -= SpeedRot * factor;
            UpdateCamera();
        }

        internal void AddPitch(float factor)
        {
            _pitch -= SpeedRot * factor;
            UpdateCamera();
        }

        internal void IncreaseCameraSpeed(float v)
        {
            Speed += v / 10f;
            if (Speed < 1f)
                Speed = 1f;
        }

        internal void IncreaseCameraRotationSpeed(float v)
        {
            SpeedRot += v / 10f;
            if (SpeedRot < 1f)
                SpeedRot = 1f;
        }

        private void UpdateCamera()
        {
            _pitch = _pitch % 360;
            _yaw = _yaw % 360;

            Forward = (Vector3)Vector3.Transform(Vector3.ForwardRH, Matrix.RotationYawPitchRoll(MathUtil.DegreesToRadians(Yaw), MathUtil.DegreesToRadians(Pitch), 0));
            Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.Up));
            Up = Vector3.Normalize(Vector3.Cross(Right, Forward));
            RaiseCameraChangedEvent();
        }

        internal void Reset()
        {
            Position = Vector3.Zero;
            Forward = Vector3.ForwardRH;
            Right = Vector3.Right;
            Up = Vector3.Up;
            _yaw = 0;
            _pitch = 0;
            Speed = 5f;
            SpeedRot = 5f;

            FieldOfView = MathUtil.Pi / 3f;
            NearPlane = 0.1F;
            FarPlane = 10000F;

            UpdateCamera();
        }

        public void SetPosition(Vector3 Position)
        {
            this.Position = Position;
            UpdateCamera();
        }

        public Vector3 GetPosition()
        {
            return Position;
        }

        public Vector3 GetForward()
        {
            return Forward;
        }

        public Vector3 GetUp()
        {
            return Up;
        }

        public Matrix GetViewMatrix()
        {
            return Matrix.LookAtRH(Position, Position + Forward, Up);
        }

        public Matrix GetProjectionMatrix()
        {
            return Matrix.PerspectiveFovRH(FieldOfView, AspectRatio, NearPlane, FarPlane);
        }

        public Matrix GetBiggerFovProjectionMatrix()
        {
            return Matrix.PerspectiveFovRH(1.3f * FieldOfView, AspectRatio, NearPlane, FarPlane);
        }

        public string GetInformation()
        {
            return string.Format("Position: [{0:0.0000}, {1:0.0000}, {2:0.0000}] Rotation: [{3:0.0000}, {4:0.0000}]",
                Position.X, Position.Y, Position.Z, Yaw, Pitch);
        }

        public void SetPositionCamera(AssetCAM cam)
        {
            Position = new Vector3(cam.PositionX, cam.PositionY, cam.PositionZ);
            Forward = new Vector3(cam.NormalizedForwardX, cam.NormalizedForwardY, cam.NormalizedForwardZ);
            Up = new Vector3(cam.NormalizedUpX, cam.NormalizedUpY, cam.NormalizedUpZ);
            Right = Vector3.Normalize(Vector3.Cross(Forward, Up));
            _pitch = MathUtil.RadiansToDegrees((float)Math.Asin(Forward.Y));
            _yaw = 270f - MathUtil.RadiansToDegrees((float)Math.Atan2(Forward.Z, Forward.X));
            _yaw = _yaw % 360f;

            RaiseCameraChangedEvent();
        }
    }
}