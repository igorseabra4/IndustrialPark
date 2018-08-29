using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace IndustrialPark
{
    public class ViewMatrix
    {
        /// <summary>
        /// Contains the matrix that converts camera coordinates from world space to view space,
        /// effectively parameterizing a camera.
        /// </summary>
        private Matrix _viewMatrix;

        // Our position and rotation ^-^
        private Vector3 _position;
        private CameraVectors _cameraVectors;
        private float _yaw;
        private float _pitch;

        // If set to false, instructs the view/projection matrix to be rebuilt on next acquisition.
        private bool _validViewMatrix;
        private bool _validCameraVectors;

        /// <summary>
        /// The left-right rotation component of the camera.
        /// Exposed as degrees, internally stored as radians.
        /// </summary>
        public float Yaw
        {
            get { return MathUtil.RadiansToDegrees(_yaw); }
            set
            {
                _validCameraVectors = false;
                _yaw = MathUtil.DegreesToRadians(value % 360);
            }
        }

        /// <summary>
        /// The up-down rotation component of the camera.
        /// Exposed as degrees, internally stored as radians.
        /// </summary>
        public float Pitch
        {
            get { return MathUtil.RadiansToDegrees(_pitch); }
            set
            {
                _validCameraVectors = false;

                // Further than looking directly up not allowed.
                if (value > 90F)
                    value = 90F;

                if (value < -90F)
                    value = -90F;

                // Set new pitch.
                _pitch = MathUtil.DegreesToRadians(value);
            }
        }

        /// <summary>
        /// Gets or sets the physical XYZ position
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
            set
            {
                _validViewMatrix = false;
                _position = value;
            }
        }

        /// <summary>
        /// Retrieves the view matrix for the current class instance.
        /// </summary>
        /// <returns>The view matrix.</returns>
        public Matrix GetViewMatrix()
        {
            if (_validViewMatrix && _validCameraVectors)
                return _viewMatrix;

            _cameraVectors = GetCameraVectors();
            _viewMatrix = Matrix.LookAtRH(_position, _position + _cameraVectors._forwardVector, Vector3.Up);
            _validViewMatrix = true;

            return _viewMatrix;
        }

        /// <summary>
        /// Retrieves the current camera vectors which represent the current forward, right and up directions
        /// for the camera.
        /// </summary>
        /// <returns></returns>
        public CameraVectors GetCameraVectors()
        {
            if (_validCameraVectors)
                return _cameraVectors;

            _cameraVectors = CameraVectors.FromYawPitchRoll(_yaw, _pitch, 0F);
            _validCameraVectors = true;
            return _cameraVectors;
        }

    }
}
