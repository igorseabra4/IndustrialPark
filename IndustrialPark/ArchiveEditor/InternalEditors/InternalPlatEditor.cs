using SharpDX;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalPlatEditor : Form, IInternalEditor
    {
        class Plat_Default
        {
            private AssetPLAT asset;
            public Plat_Default(AssetPLAT asset) { this.asset = asset; }

            [Category("Platform")]
            public int UnknownInt58
            {
                get => asset.Int58;
                set => asset.Int58 = value;
            }
            [Category("Platform")]
            public int UnknownInt5C
            {
                get => asset.Int5C;
                set => asset.Int5C = value;
            }
            [Category("Platform")]
            public int UnknownInt60
            {
                get => asset.Int60;
                set => asset.Int60 = value;
            }
            [Category("Platform")]
            public int UnknownInt64
            {
                get => asset.Int64;
                set => asset.Int64 = value;
            }
            [Category("Platform")]
            public float UnknownFloat58
            {
                get => asset.Float58;
                set => asset.Float58 = value;
            }
            [Category("Platform")]
            public float UnknownFloat5C
            {
                get => asset.Float5C;
                set => asset.Float5C = value;
            }
            [Category("Platform")]
            public float UnknownFloat60
            {
                get => asset.Float60;
                set => asset.Float60 = value;
            }
            [Category("Platform")]
            public float UnknownFloat64
            {
                get => asset.Float64;
                set => asset.Float64 = value;
            }
        }

        class Plat_Conveyor
        {
            private AssetPLAT asset;
            public Plat_Conveyor(AssetPLAT asset) { this.asset = asset; }

            [Category("Conveyor")]
            public float Speed
            {
                get => asset.Float58;
                set => asset.Float58 = value;
            }
            [Category("Conveyor")]
            public int UnknownInt5C
            {
                get => asset.Int5C;
                set => asset.Int5C = value;
            }
            [Category("Conveyor")]
            public int UnknownInt60
            {
                get => asset.Int60;
                set => asset.Int60 = value;
            }
            [Category("Conveyor")]
            public int UnknownInt64
            {
                get => asset.Int64;
                set => asset.Int64 = value;
            }
        }

        class Plat_Breakaway
        {
            private AssetPLAT asset;
            public Plat_Breakaway(AssetPLAT asset) { this.asset = asset; }

            [Category("Breakaway Platform")]
            public float BreakDelay
            {
                get => asset.Float58;
                set => asset.Float58 = value;
            }
            [Category("Breakaway Platform")]
            public AssetID UnknownInt5C
            {
                get => (uint)asset.Int5C;
                set => asset.Int5C = (int)(uint)value;
            }
            [Category("Breakaway Platform")]
            public float UnknownFloat60
            {
                get => asset.Float60;
                set => asset.Float60 = value;
            }
            [Category("Breakaway Platform")]
            public int UnknownInt64
            {
                get => asset.Int64;
                set => asset.Int64 = value;
            }
        }

        class Plat_Springboard
        {
            private AssetPLAT asset;
            public Plat_Springboard(AssetPLAT asset) { this.asset = asset; }

            [Category("Springboard")]
            public float BounceHeight1
            {
                get => asset.Float58;
                set => asset.Float58 = value;
            }
            [Category("Springboard")]
            public float BounceHeight2
            {
                get => asset.Float5C;
                set => asset.Float5C = value;
            }
            [Category("Springboard")]
            public float BounceHeight3
            {
                get => asset.Float60;
                set => asset.Float60 = value;
            }
            [Category("Springboard")]
            public float SlamHeight
            {
                get => asset.Float64;
                set => asset.Float64 = value;
            }
        }

        class Plat_Teeter
        {
            private AssetPLAT asset;
            public Plat_Teeter(AssetPLAT asset) { this.asset = asset; }

            [Category("Teeter-Totter")]
            public float UnknownFloat58
            {
                get => MathUtil.RadiansToDegrees(asset.Float58);
                set => asset.Float58 = MathUtil.DegreesToRadians(value);
            }
            [Category("Teeter-Totter")]
            public float MaxTiltAngle
            {
                get => MathUtil.RadiansToDegrees(asset.Float5C);
                set => asset.Float5C = MathUtil.DegreesToRadians(value);
            }
            [Category("Teeter-Totter")]
            public float TiltSpeed
            {
                get => asset.Float60;
                set => asset.Float60 = value;
            }
            [Category("Teeter-Totter")]
            public int UnknownInt64
            {
                get => asset.Int64;
                set => asset.Int64 = value;
            }
        }

        class Plat_Paddle
        {
            private AssetPLAT asset;
            public Plat_Paddle(AssetPLAT asset) { this.asset = asset; }

            [Category("Paddle")]
            public int CurrentSpinIndex
            {
                get => asset.Int58;
                set => asset.Int58 = value;
            }
            [Category("Paddle")]
            public float NumberOfSpins
            {
                get => asset.Float5C;
                set => asset.Float5C = value;
            }
            [Category("Paddle")]
            public float UnknownFloat60
            {
                get => asset.Float60;
                set => asset.Float60 = value;
            }
            [Category("Paddle")]
            public int UnknownInt64
            {
                get => asset.Int64;
                set => asset.Int64 = value;
            }
        }

        public InternalPlatEditor(AssetPLAT asset, ArchiveEditorFunctions archive)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archive = archive;

            propertyGridAsset.SelectedObject = asset;
            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";
            ChoosePlatSpecific();
        }

        private void InternalAssetEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            archive.CloseInternalEditor(this);
        }

        private AssetPLAT asset;
        private ArchiveEditorFunctions archive;

        public uint GetAssetID()
        {
            return asset.AHDR.assetID;
        }

        private void buttonFindCallers_Click(object sender, System.EventArgs e)
        {
            Program.MainForm.FindWhoTargets(GetAssetID());
        }

        private void propertyGridAsset_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            archive.UnsavedChanges = true;
            ArchiveEditorFunctions.UpdateGizmoPosition();
            ChoosePlatSpecific();
        }

        private void ChoosePlatSpecific()
        {
            switch (asset.PlatformType)
            {
                case PlatType.ConveyorBelt:
                    propertyGrid_PlatSpecific.SelectedObject = new Plat_Conveyor(asset);
                    break;
                case PlatType.BreakawayPlatform:
                    propertyGrid_PlatSpecific.SelectedObject = new Plat_Breakaway(asset);
                    break;
                case PlatType.Springboard:
                    propertyGrid_PlatSpecific.SelectedObject = new Plat_Springboard(asset);
                    break;
                case PlatType.TeeterTotter:
                    propertyGrid_PlatSpecific.SelectedObject = new Plat_Teeter(asset);
                    break;
                case PlatType.Paddle:
                    propertyGrid_PlatSpecific.SelectedObject = new Plat_Paddle(asset);
                    break;
                default:
                    propertyGrid_PlatSpecific.SelectedObject = new Plat_Default(asset);
                    break;
            }
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(AboutBox.WikiLink + asset.AHDR.assetType.ToString());
        }
    }
}
