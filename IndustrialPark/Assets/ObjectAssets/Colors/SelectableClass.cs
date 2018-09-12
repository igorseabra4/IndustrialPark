using System.ComponentModel;        // EditorAttribute, CategoryAttribute, DefaultValueAttribute, DescriptionAttribute
using System.Drawing.Design;        // UITypeEditor

namespace ColorDialogExample
{
    // This is an example of a class for the PropertyGrid to select.
    public class SelectableClass
    {
        private MyColor _Color = new MyColor();
        [EditorAttribute(typeof(MyColorEditor), typeof(UITypeEditor))]
        [CategoryAttribute("Design"), DefaultValueAttribute(typeof(MyColor), "0 0 0"), DescriptionAttribute("Example of a ColorDialog in a PropertyGrid.")]
        public MyColor Color
        {
            get { return _Color; }
            set { _Color = value; }
        }
    }
}
