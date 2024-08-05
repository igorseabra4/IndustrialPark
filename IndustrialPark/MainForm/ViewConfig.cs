using SharpDX;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ViewConfig : Form
    {
        public static bool ProgramIsUpdatingValues { get; set; } = true;
        private static bool _invalidCameraValues { get; set; } = false;
        private Task _updateViewValuesTask;
        private CancellationTokenSource _cancellationTokenSource;

        /*
            ------------
            Constructors
            ------------
        */

        // Default value assignments moved to Program.MainForm.renderer.

        public ViewConfig()
        {
            InitializeComponent();
            Program.MainForm.renderer.Camera.CameraChangedEvent += CameraChanged;

            NumericIntervalRotation.Maximum = decimal.MaxValue;
            NumericInterval.Maximum = decimal.MaxValue;
        }

        /// <summary>
        /// Updates the GUI if the invalid flag has been set.
        /// </summary>
        /// <param name="obj"></param>
        private async void UpdateGUIValues(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_invalidCameraValues)
                {
                    NumericFOV.Invoke((MethodInvoker)UpdateValues);
                }

                try
                {
                    await Task.Delay(33, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Signal no longer valid camera values when event thrown.
        /// </summary>
        private void CameraChanged(SharpCamera camera)
        {
            _invalidCameraValues = true;
        }


        /*
             ------
             Events
             ------
        */

        private void ViewConfig_Load(object sender, EventArgs e)
        {
            TopMost = true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown)
                return;
            if (e.CloseReason == CloseReason.FormOwnerClosing)
                return;

            e.Cancel = true;
            Hide();
        }

        private void NumericCamera_ValueChanged(object sender, EventArgs e)
        {
            if (!ProgramIsUpdatingValues)
                Program.MainForm.renderer.Camera.SetPosition(new Vector3((float)NumericCameraX.Value, (float)NumericCameraY.Value, (float)NumericCameraZ.Value));
        }

        private void NumericCameraRot_ValueChanged(object sender, EventArgs e)
        {
            if (!ProgramIsUpdatingValues)
            {
                Program.MainForm.renderer.Camera.Pitch = (float)NumericCameraPitch.Value;
                Program.MainForm.renderer.Camera.Yaw = (float)NumericCameraYaw.Value;
            }
        }

        private void NumericInterval_ValueChanged(object sender, EventArgs e)
        {
            if (!ProgramIsUpdatingValues)
                Program.MainForm.renderer.Camera.Speed = (float)NumericInterval.Value;
        }

        private void NumericIntervalRotation_ValueChanged(object sender, EventArgs e)
        {
            if (!ProgramIsUpdatingValues)
                Program.MainForm.renderer.Camera.SpeedRot = (float)NumericIntervalRotation.Value;
        }

        private void NumericDrawD_ValueChanged(object sender, EventArgs e)
        {
            Program.MainForm.renderer.Camera.FarPlane = (float)NumericDrawD.Value;
        }

        private void NumericFOV_ValueChanged(object sender, EventArgs e)
        {
            if (NumericFOV.Value < 1)
                NumericFOV.Value = 1;
            Program.MainForm.renderer.Camera.FieldOfView = MathUtil.DegreesToRadians((float)NumericFOV.Value);
        }

        private void numericGrid_ValueChanged(object sender, EventArgs e)
        {
            if (!ProgramIsUpdatingValues)
                ArchiveEditorFunctions.Grid = new Vector3((float)numericGridX.Value, (float)numericGridY.Value, (float)numericGridZ.Value);
        }

        private void ViewConfig_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                UpdateValues();
                _cancellationTokenSource = new CancellationTokenSource();
                _updateViewValuesTask = Task.Run(() => UpdateGUIValues(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
            }
            else
            {
                _cancellationTokenSource?.Cancel();
                _updateViewValuesTask?.Wait();
            }

        }

        /*
            -------
            Methods
            -------
        */

        /// <summary>
        /// Obtains the values from the current Power Plant instance and applies them to the
        /// View Config.
        /// </summary>
        public void UpdateValues()
        {
            ProgramIsUpdatingValues = true;
            NumericFOV.Value = (decimal)MathUtil.RadiansToDegrees(Program.MainForm.renderer.Camera.FieldOfView);
            NumericDrawD.Value = (decimal)Program.MainForm.renderer.Camera.FarPlane;
            NumericInterval.Value = (decimal)Program.MainForm.renderer.Camera.Speed;
            NumericIntervalRotation.Value = (decimal)Program.MainForm.renderer.Camera.SpeedRot;
            NumericCameraX.Value = (decimal)Program.MainForm.renderer.Camera.Position.X;
            NumericCameraY.Value = (decimal)Program.MainForm.renderer.Camera.Position.Y;
            NumericCameraZ.Value = (decimal)Program.MainForm.renderer.Camera.Position.Z;

            NumericCameraYaw.Value = (decimal)Program.MainForm.renderer.Camera.Yaw;
            NumericCameraPitch.Value = (decimal)Program.MainForm.renderer.Camera.Pitch;

            numericGridX.Value = (decimal)ArchiveEditorFunctions.Grid.X;
            numericGridY.Value = (decimal)ArchiveEditorFunctions.Grid.Y;
            numericGridZ.Value = (decimal)ArchiveEditorFunctions.Grid.Z;

            ProgramIsUpdatingValues = false;
            _invalidCameraValues = false;
        }

        public void SetValues(Vector3 position, float yaw, float pitch, float speed, float speedrot)
        {
            if (!Visible)
                return;

            ProgramIsUpdatingValues = true;

            NumericCameraX.Value = (decimal)position.X;
            NumericCameraY.Value = (decimal)position.Y;
            NumericCameraZ.Value = (decimal)position.Z;
            NumericInterval.Value = (decimal)speed;
            NumericIntervalRotation.Value = (decimal)speedrot;
            NumericCameraPitch.Value = (decimal)pitch;
            NumericCameraYaw.Value = (decimal)yaw;

            ProgramIsUpdatingValues = false;
            _invalidCameraValues = false;
        }
    }
}
