using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ConnectMarkers : Form
    {
        private AssetMRKR ConnectingMarkerA;
        private AssetMRKR ConnectingMarkerB;
        private List<Vector3> Positions = new List<Vector3>();
        public Func<IEnumerable<AssetMRKR>> getMarkers { get; }

        public ConnectMarkers(Func<IEnumerable<AssetMRKR>> getMarkers)
        {
            InitializeComponent();

            radioArc.CheckedChanged += (sender, e) => Setup();
            radioCircle.CheckedChanged += (sender, e) => Setup();
            radioLine.CheckedChanged += (sender, e) => Setup();

            this.getMarkers = getMarkers;

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown)
                return;
            if (e.CloseReason == CloseReason.FormOwnerClosing)
                return;

            e.Cancel = true;
        }

        private void Setup()
        {
            if (radioLine.Checked)
            {
                numericNumber.Minimum = 3;
                numericAngle.Visible = false;
                labelAngle.Visible = false;
                numericArc.Visible = false;
                labelArc.Visible = false;
            }
            else if (radioCircle.Checked)
            {
                numericNumber.Minimum = 2;
                numericAngle.Visible = true;
                labelAngle.Visible = true;
                numericArc.Visible = false;
                labelArc.Visible = false;
            }
            else if (radioArc.Checked)
            {
                numericNumber.Minimum = 3;
                numericAngle.Visible = true;
                labelAngle.Visible = true;
                numericArc.Visible = true;
                labelArc.Visible = true;
            }
        }

        public bool SetupConnectMarkers()
        {
            Positions.Clear();
            ConnectingMarkerA = null;
            ConnectingMarkerB = null;

            var markers = getMarkers();

            if (markers.Count() == 2)
            {
                markers = markers.OrderBy(m => m.assetName);
                ConnectingMarkerA = markers.First();
                ConnectingMarkerB = markers.Last();
                return true;
            }

            MessageBox.Show("You must have exactly 2 markers selected to use Connect Markers.");

            return false;
        }

        public void CreateTransformMatrix()
        {
            Positions.Clear();

            if (ConnectingMarkerA == null || ConnectingMarkerB == null)
                return;

            var number = (float)numericNumber.Value;
            var rotation = (float)numericAngle.Value;
            var arcAngle = (float)numericArc.Value;
            var startPos = new Vector3(ConnectingMarkerA.PositionX, ConnectingMarkerA.PositionY, ConnectingMarkerA.PositionZ);
            var endPos = new Vector3(ConnectingMarkerB.PositionX, ConnectingMarkerB.PositionY, ConnectingMarkerB.PositionZ);
            var direction = endPos - startPos;
            var length = direction.Length();

            if (length <= 0f)
                return;

            if (radioLine.Checked)
            {
                for (int i = 0; i < number; i++)
                    Positions.Add(Vector3.Lerp(startPos, endPos, i / (number - 1)));
            }
            else if (radioCircle.Checked)
            {
                if (number < 2)
                    return;

                var right = Vector3.Normalize(direction);
                var reference = MathF.Abs(Vector3.Dot(right, Vector3.UnitY)) < 0.999f ? Vector3.UnitY : Vector3.UnitX;
                var perpendicular = Vector3.Normalize(Vector3.Cross(right, reference));
                var angleRadians = rotation * (MathF.PI / 180f);
                var forward = perpendicular * MathF.Cos(angleRadians) + Vector3.Cross(right, perpendicular) * MathF.Sin(angleRadians);

                for (int i = 0; i < number; i++)
                {
                    var theta = 2f * MathF.PI * i / number;
                    var offset = right * (MathF.Cos(theta) * length) + forward * (MathF.Sin(theta) * length);
                    Positions.Add(startPos + offset);
                }
            }
            else if (radioArc.Checked)
            {
                if (number < 3)
                    return;

                direction = Vector3.Normalize(endPos - startPos);

                var up = Vector3.UnitY;
                var side = Vector3.Cross(direction, up);
                if (side.LengthSquared() < 0.000001f)
                    side = Vector3.Cross(direction, Vector3.UnitX);
                side = Vector3.Normalize(side);
                var twist = Matrix.RotationAxis(direction, rotation * (MathF.PI / 180f));
                side = Vector3.TransformNormal(side, twist);
                var normal = Vector3.Normalize(Vector3.Cross(direction, side));
                var arcRadians = arcAngle * (MathF.PI / 180f);

                if (MathF.Abs(arcRadians) < 0.0001f)
                {
                    for (int i = 0; i < number; i++)
                        Positions.Add(Vector3.Lerp(startPos, endPos, i / (number - 1)));
                }
                else
                {
                    var halfAngle = arcRadians / 2f;
                    var radius = length / (2f * MathF.Sin(halfAngle));
                    var center = (startPos + endPos) / 2f + normal * (radius * MathF.Cos(halfAngle));
                    var startRadius = startPos - center;

                    for (int i = 0; i < number; i++)
                    {
                        var theta = arcRadians * i / (number - 1);
                        var rotationMatrix = Matrix.RotationAxis(normal, theta);
                        var offset = Vector3.TransformNormal(startRadius, rotationMatrix);
                        Positions.Add(center + offset);
                    }
                }
            }
        }

        public void Draw(SharpRenderer renderer)
        {
            CreateTransformMatrix();
            foreach (var pos in Positions)
            {
                var world = Matrix.RotationX(MathUtil.PiOverTwo) * Matrix.Translation(pos + new Vector3(0f, 0.5f, 0f));
                renderer.DrawPyramid(world, false, 1f, color: renderer.colors.camColor);
            }
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            if (Program.MainForm.PlaceTemplatesAt(Positions, checkDeleteMarkers.Checked, ConnectingMarkerA, ConnectingMarkerB))
            {
                Positions.Clear();
                ConnectingMarkerA = null;
                ConnectingMarkerB = null;
                Hide();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Positions.Clear();
            ConnectingMarkerA = null;
            ConnectingMarkerB = null;
            Hide();
        }
    }
}