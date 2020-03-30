using SharpDX;
using System.Collections.Generic;
using HipHopFile;

namespace IndustrialPark
{
    public class ProjectJson
    {
        public static int getCurrentVersion => 2;
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
        public bool renderBasedOnLodt;
        public bool renderBasedOnPipt;

        public bool dontRenderJSP;
        public bool dontDrawInvisible;

        public bool dontRenderBOUL;
        public bool dontRenderBUTN;
        public bool dontRenderCAM;
        public bool dontRenderDSTR;
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
        public bool dontRenderSFX;
        public bool dontRenderSIMP;
        public bool dontRenderTRIG;
        public bool dontRenderUI;
        public bool dontRenderUIFT;
        public bool dontRenderVIL;

        public bool persistentShinies;
        public bool hideHelp;

        public ProjectJson()
        {
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
            renderBasedOnLodt = false;
            renderBasedOnPipt = false;
            dontDrawInvisible = false;

            dontRenderJSP = false;
            dontRenderBOUL = false;
            dontRenderBUTN = false;
            dontRenderCAM = false;
            dontRenderDSTR = false;
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
            dontRenderSFX = false;
            dontRenderSIMP = false;
            dontRenderTRIG = false;
            dontRenderUI = true;
            dontRenderUIFT = true;
            dontRenderVIL = false;

            persistentShinies = true;
            hideHelp = false;
        }

        public ProjectJson(List<string> hipPaths, List<Platform> scoobyPlatforms, List<string> textureFolderPaths, Vector3 camPos, float yaw, float pitch, float speed, float speedRot,
            float fieldOfView, float farPlane, bool noCulling, bool wireframe, Color4 backgroundColor, Vector4 widgetColor, Vector4 trigColor,
            Vector4 mvptColor, Vector4 sfxColor, bool useLegacyAssetIDFormat, List<uint> hiddenAssets, bool isDrawingUI, Vector3 Grid,
            bool renderBasedOnLodt, bool renderBasedOnPipt, bool dontDrawInvisible, bool dontRenderJSP, bool dontRenderBOUL, 
            bool dontRenderBUTN, bool dontRenderCAM, bool dontRenderDSTR, bool dontRenderDYNA, bool dontRenderEGEN, bool dontRenderHANG,
            bool dontRenderLITE, bool dontRenderMRKR, bool dontRenderMVPT, bool dontRenderPEND, bool dontRenderPKUP, bool dontRenderPLAT,
            bool dontRenderPLYR, bool dontRenderSFX, bool dontRenderSIMP, bool dontRenderTRIG, bool dontRenderUI, bool dontRenderUIFT, 
            bool dontRenderVIL, bool persistentShinies, bool hideHelp)
        {
            version = getCurrentVersion;

            this.hipPaths = hipPaths;
            this.scoobyPlatforms = scoobyPlatforms;
            TextureFolderPaths = textureFolderPaths;
            CamPos = camPos;
            Yaw = yaw;
            Pitch = pitch;
            Speed = speed;
            SpeedRot = speedRot;
            FieldOfView = fieldOfView;
            FarPlane = farPlane;
            NoCulling = noCulling;
            Wireframe = wireframe;
            BackgroundColor = backgroundColor;
            WidgetColor = widgetColor;
            TrigColor = trigColor;
            MvptColor = mvptColor;
            SfxColor = sfxColor;
            UseLegacyAssetIDFormat = useLegacyAssetIDFormat;
            this.hiddenAssets = hiddenAssets;
            this.isDrawingUI = isDrawingUI;
            this.renderBasedOnLodt = renderBasedOnLodt;
            this.renderBasedOnPipt = renderBasedOnPipt;
            this.dontDrawInvisible = dontDrawInvisible;
            this.dontRenderJSP = dontRenderJSP;
            this.dontRenderBOUL = dontRenderBOUL;
            this.dontRenderBUTN = dontRenderBUTN;
            this.dontRenderCAM = dontRenderCAM;
            this.dontRenderDSTR = dontRenderDSTR;
            this.dontRenderDYNA = dontRenderDYNA;
            this.dontRenderEGEN = dontRenderEGEN;
            this.dontRenderHANG = dontRenderHANG;
            this.dontRenderLITE = dontRenderLITE;
            this.dontRenderMRKR = dontRenderMRKR;
            this.dontRenderMVPT = dontRenderMVPT;
            this.dontRenderPEND = dontRenderPEND;
            this.dontRenderPKUP = dontRenderPKUP;
            this.dontRenderPLAT = dontRenderPLAT;
            this.dontRenderPLYR = dontRenderPLYR;
            this.dontRenderSFX = dontRenderSFX;
            this.dontRenderSIMP = dontRenderSIMP;
            this.dontRenderTRIG = dontRenderTRIG;
            this.dontRenderUI = dontRenderUI;
            this.dontRenderUIFT = dontRenderUIFT;
            this.dontRenderVIL = dontRenderVIL;
            this.persistentShinies = persistentShinies;
            this.hideHelp = hideHelp;
            this.Grid = Grid;
        }
    }
}
