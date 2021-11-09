using SharpDX;

namespace IndustrialPark
{
    /// <summary>
    /// Stores and allows for the manipulation of both a view and projection matrix.
    /// </summary>
    public class ProjectionMatrix
    {
        private float _fieldOfView = MathUtil.PiOverFour;
        private float _aspectRatio;
        private float _nearPlane = 0.1F;
        private float _farPlane = 80000F;

        /// <summary>
        /// Contains the matrix that converts view space coordinates to screen coordinates; squashing
        /// three dimensions into two.
        /// </summary>
        private Matrix _projectionMatrix;
        private bool _validProjectonMatrix;

        public ProjectionMatrix()
        {
        }

        public ProjectionMatrix(float farPlane, float fieldOfView)
        {
            FarPlane = farPlane;
            FieldOfView = fieldOfView;
        }

        /////////////
        // Properties
        /////////////

        /// <summary>
        /// The observable angle of the world from the tip/start of the camera.
        /// </summary>
        public float FieldOfView
        {
            get { return MathUtil.RadiansToDegrees(_fieldOfView); }
            set
            {
                _validProjectonMatrix = false;
                _fieldOfView = MathUtil.DegreesToRadians(value);
            }
        }

        /// <summary>
        /// Horizontal resolution divided by vertical resolution.
        /// </summary>
        public float AspectRatio
        {
            get { return _aspectRatio; }
            set
            {
                _validProjectonMatrix = false;
                _aspectRatio = value;
            }
        }

        /// <summary>
        /// Objects closer than this distance to the camera will not be rendered.
        /// </summary>
        public float NearPlane
        {
            get { return _nearPlane; }
            set
            {
                _validProjectonMatrix = false;
                _nearPlane = value;
            }
        }

        /// <summary>
        /// Objects farther than this distance to the camera will not be rendered.
        /// </summary>
        public float FarPlane
        {
            get { return _farPlane; }
            set
            {
                _validProjectonMatrix = false;
                _farPlane = value;
            }
        }

        /// <summary>
        /// Retrieves the projection matrix for the current class instance.
        /// </summary>
        /// <returns>The projection matrix</returns>
        public Matrix GetProjectionMatrix()
        {
            // No matrix creation needed if nothing changed.
            if (_validProjectonMatrix)
                return _projectionMatrix;

            _projectionMatrix = Matrix.PerspectiveFovRH(_fieldOfView, _aspectRatio, _nearPlane, _farPlane);
            _validProjectonMatrix = true;

            return _projectionMatrix;
        }
    }
}
