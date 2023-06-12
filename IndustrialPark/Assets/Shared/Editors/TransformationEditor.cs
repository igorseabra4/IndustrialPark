using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace IndustrialPark
{
    public class TransformationEditor : UITypeEditor
    {
        public static bool isCopy;
        public static Transformation transformation;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (isCopy)
                Clipboard.SetText(JsonConvert.SerializeObject(transformation));
            else
            {
                try
                {
                    transformation = JsonConvert.DeserializeObject<Transformation>(Clipboard.GetText());
                    return "transformed";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"There was an error pasting the transformation from clipboard: ${ex.Message}. Are you sure you have a transformation copied?");
                }
            }

            return "untransformed";
        }
    }

    public struct Transformation
    {
        public float _positionX;
        public float _positionY;
        public float _positionZ;
        public float _yaw;
        public float _pitch;
        public float _roll;
        public float _scaleX;
        public float _scaleY;
        public float _scaleZ;
    }
}
