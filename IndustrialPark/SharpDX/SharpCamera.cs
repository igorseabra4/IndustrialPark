using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class SharpCamera
    {
        public Vector3 Position = Vector3.Zero;
        private Vector3 Forward = Vector3.ForwardRH;
        private Vector3 Right = Vector3.Right;
        private Vector3 Up = Vector3.Up;
        public float Yaw = 0;
        public float Pitch = 0;
        public float Speed = 5f;

        public float FieldOfView = MathUtil.PiOverFour;
        public float AspectRatio;
        public float NearPlane = 0.1F;
        public float FarPlane = 80000F;

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
            Yaw -= Speed * factor;
            UpdateCamera();
        }

        internal void AddPitch(float factor)
        {
            Pitch -= Speed * factor;
            UpdateCamera();
        }

        internal void IncreaseCameraSpeed(float v)
        {
            Speed += v / 10f;
            if (Speed < 1f)
                Speed = 1f;
            UpdateCamera();
        }

        private void UpdateCamera()
        {
            Pitch = Pitch % 360;
            Yaw = Yaw % 360;

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
            Yaw = 0;
            Pitch = 0;
            Speed = 5f;

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

        public void SetRotation(float Pitch, float Yaw)
        {
            this.Pitch = Pitch;
            this.Yaw = Yaw;
            UpdateCamera();
        }

        public void SetSpeed(float Speed)
        {
            this.Speed = Speed;
            UpdateCamera();
        }

        public Matrix GetViewMatrix()
        {
            return Matrix.LookAtRH(Position, Position + Forward, Up);
        }

        public Matrix GetProjectionMatrix()
        {
            return Matrix.PerspectiveFovRH(FieldOfView, AspectRatio, NearPlane, FarPlane);
        }

        public string GetInformation()
        {
            return String.Format("Position: [{0:0.0000}, {1:0.0000}, {2:0.0000}] Rotation: [{3:0.0000}, {4:0.0000}]",
                Position.X, Position.Y, Position.Z, Yaw, Pitch);
        }

    }
}