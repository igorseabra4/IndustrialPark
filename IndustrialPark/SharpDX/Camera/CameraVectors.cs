using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace IndustrialPark
{
    /// <summary>
    /// The CameraVectors class calculates and provides the forward, right and up vector for a generic
    /// rotation of the camera.
    /// </summary>
    public struct CameraVectors
    {
        /// <summary>
        /// This is the forward unit vector (Z direction) used for the purpose
        /// of navigating through space. It is calculated by transforming the forward vector
        /// 0,0,-1 by a rotation matrix geenrated from Yaw, Pitch and Roll, then normalizing.
        /// </summary>
        public Vector3 _forwardVector;

        /// <summary>
        /// This is the left unit vector (X direction) used for the purpose of navigating through
        /// space. It is calculated by crossing the up vector 0,1,0 and the forward vector, then normalizing.
        /// </summary>
        public Vector3 _leftVector;

        /// <summary>
        /// This is the right unit vector (X direction) used for the purpose of navigating through
        /// space. It is calculated by crossing the forward & right vector, then normalizing.
        /// </summary>
        public Vector3 _upVector;

        /// <summary>
        /// Calculates the camera movement vectors from 
        /// </summary>
        /// <param name="yawDegrees">The direction that moves the camera left and right.</param>
        /// <param name="pitchDegrees">The direction that moves the camera up and down.</param>
        /// <param name="rollDegrees">Self explanatory.</param>
        public static CameraVectors FromYawPitchRoll(float yawRadians, float pitchRadians, float rollRadians)
        {
            CameraVectors cameraVectors = new CameraVectors();

            Matrix.RotationYawPitchRoll(yawRadians, pitchRadians, rollRadians, out Matrix result);

            cameraVectors._forwardVector = (Vector3)Vector3.Transform(Vector3.ForwardRH, result);
            cameraVectors._forwardVector.Normalize();

            cameraVectors._leftVector = Vector3.Cross(Vector3.Up, cameraVectors._forwardVector);
            cameraVectors._leftVector.Normalize();

            cameraVectors._upVector = Vector3.Cross(cameraVectors._forwardVector, cameraVectors._leftVector);
            cameraVectors._upVector.Normalize();

            return cameraVectors;
        }

    }
}
