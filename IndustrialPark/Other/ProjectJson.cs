using SharpDX;
using System.Collections.Generic;
using HipHopFile;

namespace IndustrialPark
{
    public class ProjectJson
    {
        public static int getCurrentVersion => 3;
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

        public bool UseLegacyAssetIDFormat;
        public bool isDrawingUI;

        public bool dontRenderJSP;

        public bool dontRenderBOUL;
        public bool dontRenderBUTN;
        public bool dontRenderCAM;
        public bool dontRenderDSTR;
        public bool dontRenderDTRK;
        public bool dontRenderDYNA;
        public bool dontRenderEGEN;
        public bool dontRenderHANG;
        public bool dontRenderLITE;
        public bool dontRenderMRKR;
        public bool dontRenderMVPT;
        public bool dontRenderPEND;
        public bool dontRenderPKUP;
        public bool dontRenderPLAT;
        public bool dontRenderPLYR;
        public bool dontRenderSDFX;
        public bool dontRenderSFX;
        public bool dontRenderSIMP;
        public bool dontRenderSPLN;
        public bool dontRenderTRIG;
        public bool dontRenderUI;
        public bool dontRenderUIFT;
        public bool dontRenderVIL;
        
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

            FieldOfView = MathUtil.PiOverFour;
            FarPlane = 10000F;

            NoCulling = false;
            Wireframe = false;

            BackgroundColor = new Color4(0.05f, 0.05f, 0.15f, 1f);
            WidgetColor = new Vector4(0.2f, 0.6f, 0.8f, 0.55f);
            TrigColor = new Vector4(0.3f, 0.8f, 0.7f, 0.4f);
            MvptColor = new Vector4(0.7f, 0.2f, 0.6f, 0.5f);
            SfxColor = new Vector4(1f, 0.2f, 0.2f, 0.35f);

            Grid = new Vector3(1f, 1f, 1f);

            UseLegacyAssetIDFormat = false;
            isDrawingUI = false;

            dontRenderJSP = false;
            dontRenderBOUL = false;
            dontRenderBUTN = false;
            dontRenderCAM = false;
            dontRenderDSTR = false;
            dontRenderDTRK = false;
            dontRenderDYNA = false;
            dontRenderEGEN = false;
            dontRenderHANG = false;
            dontRenderLITE = false;
            dontRenderMRKR = false;
            dontRenderMVPT = false;
            dontRenderPEND = false;
            dontRenderPKUP = false;
            dontRenderPLAT = false;
            dontRenderPLYR = false;
            dontRenderSDFX = false;
            dontRenderSFX = false;
            dontRenderSIMP = false;
            dontRenderSPLN = false;
            dontRenderTRIG = false;
            dontRenderUI = true;
            dontRenderUIFT = true;
            dontRenderVIL = false;
        }
    }
}
