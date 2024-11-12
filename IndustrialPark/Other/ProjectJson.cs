using HipHopFile;
using SharpDX;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class ProjectJson
    {
        public static int getCurrentVersion => 5;
        public int version;

        public List<string> hipPaths;
        public List<Platform> scoobyPlatforms;
        public List<string> TextureFolderPaths;
        public List<uint> hiddenAssets;

        public Vector3 CamPos;
        public float Yaw;
        public float Pitch;
        public float Speed;
        public float SpeedRot;
        public float FieldOfView;
        public float FarPlane;

        public bool NoCulling;
        public bool Wireframe;

        public Color4 BackgroundColor;
        public Vector4 WidgetColor;
        public Vector4 TrigColor;
        public Vector4 MvptColor;
        public Vector4 SfxColor;

        public Vector3 Grid;

        public bool isDrawingUI;

        public Dictionary<string, bool> dontRender;

        public bool FogRender;
        public bool LightKitRender;
        public bool VertexColorRender;

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
            Speed = 5f;
            SpeedRot = 5f;

            FieldOfView = MathUtil.DegreesToRadians(60);
            FarPlane = 10000F;

            NoCulling = false;
            Wireframe = false;

            BackgroundColor = new Color4(0.05f, 0.05f, 0.15f, 1f);
            WidgetColor = new Vector4(0.2f, 0.6f, 0.8f, 0.55f);
            TrigColor = new Vector4(0.3f, 0.8f, 0.7f, 0.4f);
            MvptColor = new Vector4(0.7f, 0.2f, 0.6f, 0.5f);
            SfxColor = new Vector4(1f, 0.2f, 0.2f, 0.35f);

            Grid = new Vector3(1f, 1f, 1f);

            isDrawingUI = false;

            dontRender = new Dictionary<string, bool>();

            FogRender = true;
            LightKitRender = true;
            VertexColorRender = true;
        }
    }
}
