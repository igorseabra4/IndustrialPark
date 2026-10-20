using HipHopFile;
using SharpDX;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class ProjectJson
    {
        public static int getCurrentVersion => 7;
        public int version;

        public List<string> hipPaths;
        public List<Platform> scoobyPlatforms;
        public List<string> TextureFolderPaths;
        public List<uint> hiddenAssets;

        public Vector3 CamPos;
        public float Yaw;
        public float Pitch;
        public float FieldOfView;
        public float FarPlane;

        public Color4 BackgroundColor;
        public RendererColors Colors;

        public Vector3 Grid;

        public bool isDrawingUI;

        public Dictionary<string, bool> dontRender;

        public ProjectJson()
        {
            version = getCurrentVersion;

            hipPaths = new List<string>();
            scoobyPlatforms = new List<Platform>();
            TextureFolderPaths = new List<string>();
            hiddenAssets = new List<uint>();

            CamPos = new Vector3();
            Yaw = 0;
            Pitch = 0;

            FieldOfView = MathUtil.DegreesToRadians(SharpCamera.DEFAULT_FOV);
            FarPlane = SharpCamera.DEFAULT_FAR_PLANE;

            BackgroundColor = SharpRenderer.defaultBackgroundColor;
            Colors = RendererColors.Default;

            Grid = new Vector3(1f, 1f, 1f);

            isDrawingUI = false;

            dontRender = new Dictionary<string, bool>();
        }
    }
}
