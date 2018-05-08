using SharpDX;
using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ViewConfig : Form
    {
        public ViewConfig()
        {
            InitializeComponent();

            NumericCameraX.Maximum = Decimal.MaxValue;
            NumericCameraY.Maximum = Decimal.MaxValue;
            NumericCameraZ.Maximum = Decimal.MaxValue;
            NumericCameraPitch.Maximum = Decimal.MaxValue;
            NumericCameraYaw.Maximum = Decimal.MaxValue;
            NumericInterval.Maximum = Decimal.MaxValue;
            NumericDrawD.Maximum = Decimal.MaxValue;
            NumericFOV.Maximum = 179.9999M;

            NumericCameraX.Minimum = Decimal.MinValue;
            NumericCameraY.Minimum = Decimal.MinValue;
            NumericCameraZ.Minimum = Decimal.MinValue;
            NumericCameraPitch.Minimum = Decimal.MinValue;
            NumericCameraYaw.Minimum = Decimal.MinValue;
            NumericInterval.Minimum = 0.0001M;
            NumericDrawD.Minimum = 1;
            NumericFOV.Minimum = 0.0001M;

            NumericFOV.Value = (decimal)MathUtil.RadiansToDegrees(MathUtil.PiOverFour);
            NumericDrawD.Value = 50000;
        }

        public bool programIsUpdatingValues = false;

        private void ViewConfig_Load(object sender, EventArgs e)
        {
            TopMost = true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            if (e.CloseReason == CloseReason.FormOwnerClosing) return;

            e.Cancel = true;
            Hide();
        }
        
        private void NumericCamera_ValueChanged(object sender, EventArgs e)
        {
            if (!programIsUpdatingValues)
                SharpRenderer.Camera.SetPosition(new Vector3((float)NumericCameraX.Value, (float)NumericCameraY.Value, (float)NumericCameraZ.Value));
        }

        private void NumericCameraRot_ValueChanged(object sender, EventArgs e)
        {
            if (!programIsUpdatingValues)
                SharpRenderer.Camera.SetRotation((float)NumericCameraPitch.Value, (float)NumericCameraYaw.Value);
        }

        private void NumericInterval_ValueChanged(object sender, EventArgs e)
        {
            if (!programIsUpdatingValues)
                SharpRenderer.Camera.SetSpeed((float)NumericInterval.Value);
        }

        public void UpdateValues(Vector3 position, float yaw, float pitch, float speed)
        {
            programIsUpdatingValues = true;

            NumericCameraX.Value = (decimal)position.X;
            NumericCameraY.Value = (decimal)position.Y;
            NumericCameraZ.Value = (decimal)position.Z;
            NumericInterval.Value = (decimal)speed;
            NumericCameraPitch.Value = (decimal)pitch;
            NumericCameraYaw.Value = (decimal)yaw;

            programIsUpdatingValues = false;
        }

        private void NumericDrawD_ValueChanged(object sender, EventArgs e)
        {
            SharpRenderer.far = (float)NumericDrawD.Value;
        }

        private void NumericFOV_ValueChanged(object sender, EventArgs e)
        {
            if (NumericFOV.Value < 1)
                NumericFOV.Value = 1;
            SharpRenderer.fovAngle = MathUtil.DegreesToRadians((float)NumericFOV.Value);
        }
    }
}
