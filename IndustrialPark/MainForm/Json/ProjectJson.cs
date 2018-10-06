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

        public bool renderLevelModel;
        public bool renderBOUL;
        public bool renderBUTN;
        public bool renderCAM;
        public bool renderDSTR;
        public bool renderMRKR;
        public bool renderMVPT;
        public bool renderPKUP;
        public bool renderPLAT;
        public bool renderPLYR;
        public bool renderSFX;
        public bool renderSIMP;
        public bool renderTRIG;
        public bool renderVIL;

        public ProjectJson()
        {
            hipPaths = new List<string>();
            TextureFolderPaths = new List<string>();
        }

        public ProjectJson(List<string> hipPaths, List<string> textureFolderPaths, Vector3 camPos, float yaw, float pitch, float speed, float speedRot,
            float fieldOfView, float farPlane, bool noCulling, bool wireframe, Color4 backgroundColor, Vector4 widgetColor, Vector4 trigColor,
            Vector4 mvptColor, Vector4 sfxColor, bool useLegacyAssetIDFormat, bool alternateNameDisplayMode, bool renderLevelModel,
            bool renderBOUL, bool renderBUTN, bool renderCAM, bool renderDSTR, bool renderMRKR, bool renderMVPT, bool renderPKUP, bool renderPLAT,
            bool renderPLYR, bool renderSFX, bool renderSIMP, bool renderTRIG, bool renderVIL)
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
            this.renderLevelModel = renderLevelModel;
            this.renderBOUL = renderBOUL;
            this.renderBUTN = renderBUTN;
            this.renderCAM = renderCAM;
            this.renderDSTR = renderDSTR;
            this.renderMRKR = renderMRKR;
            this.renderMVPT = renderMVPT;
            this.renderPKUP = renderPKUP;
            this.renderPLAT = renderPLAT;
            this.renderPLYR = renderPLYR;
            this.renderSFX = renderSFX;
            this.renderSIMP = renderSIMP;
            this.renderTRIG = renderTRIG;
            this.renderVIL = renderVIL;
        }
    }
}
