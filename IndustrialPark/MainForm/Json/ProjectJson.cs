using SharpDX;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class ProjectJson
    {
        public List<string> hipPaths;
        public List<string> TextureFolderPaths;

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

        public bool UseLegacyAssetIDFormat;
        public bool AlternateNameDisplayMode;
        public bool isDrawingUI;
        
        public bool dontRenderLevelModel;
        public bool dontRenderBOUL;
        public bool dontRenderBUTN;
        public bool dontRenderCAM;
        public bool dontRenderDSTR;
        public bool dontRenderDYNA;
        public bool dontRenderMRKR;
        public bool dontRenderMVPT;
        public bool dontRenderPKUP;
        public bool dontRenderPLAT;
        public bool dontRenderPLYR;
        public bool dontRenderSFX;
        public bool dontRenderSIMP;
        public bool dontRenderTRIG;
        public bool dontRenderUI;
        public bool dontRenderUIFT;
        public bool dontRenderVIL;

        public ProjectJson()
        {
            hipPaths = new List<string>();
            TextureFolderPaths = new List<string>();

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

            UseLegacyAssetIDFormat = false;
            AlternateNameDisplayMode = false;
            isDrawingUI = false;

            dontRenderLevelModel = false;
            dontRenderBOUL = false;
            dontRenderBUTN = false;
            dontRenderCAM = false;
            dontRenderDSTR = false;
            dontRenderDYNA = false;
            dontRenderMRKR = false;
            dontRenderMVPT = false;
            dontRenderPKUP = false;
            dontRenderPLAT = false;
            dontRenderPLYR = false;
            dontRenderSFX = false;
            dontRenderSIMP = false;
            dontRenderTRIG = false;
            dontRenderUI = false;
            dontRenderUIFT = false;
            dontRenderVIL = false;
        }

        public ProjectJson(List<string> hipPaths, List<string> textureFolderPaths, Vector3 camPos, float yaw, float pitch, float speed, float speedRot,
            float fieldOfView, float farPlane, bool noCulling, bool wireframe, Color4 backgroundColor, Vector4 widgetColor, Vector4 trigColor,
            Vector4 mvptColor, Vector4 sfxColor, bool useLegacyAssetIDFormat, bool alternateNameDisplayMode, bool isDrawingUI, bool dontRenderLevelModel,
            bool dontRenderBOUL, bool dontRenderBUTN, bool dontRenderCAM, bool dontRenderDSTR, bool dontRenderDYNA, bool dontRenderMRKR, bool dontRenderMVPT,
            bool dontRenderPKUP, bool dontRenderPLAT, bool dontRenderPLYR, bool dontRenderSFX, bool dontRenderSIMP, bool dontRenderTRIG, bool dontRenderUI,
            bool dontRenderUIFT, bool dontRenderVIL)
        {
            this.hipPaths = hipPaths;
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
            AlternateNameDisplayMode = alternateNameDisplayMode;
            this.isDrawingUI = isDrawingUI;
            this.dontRenderLevelModel = dontRenderLevelModel;
            this.dontRenderBOUL = dontRenderBOUL;
            this.dontRenderBUTN = dontRenderBUTN;
            this.dontRenderCAM = dontRenderCAM;
            this.dontRenderDSTR = dontRenderDSTR;
            this.dontRenderDYNA = dontRenderDYNA;
            this.dontRenderMRKR = dontRenderMRKR;
            this.dontRenderMVPT = dontRenderMVPT;
            this.dontRenderPKUP = dontRenderPKUP;
            this.dontRenderPLAT = dontRenderPLAT;
            this.dontRenderPLYR = dontRenderPLYR;
            this.dontRenderSFX = dontRenderSFX;
            this.dontRenderSIMP = dontRenderSIMP;
            this.dontRenderTRIG = dontRenderTRIG;
            this.dontRenderUI = dontRenderUI;
            this.dontRenderUIFT = dontRenderUIFT;
            this.dontRenderVIL = dontRenderVIL;
        }
    }
}
