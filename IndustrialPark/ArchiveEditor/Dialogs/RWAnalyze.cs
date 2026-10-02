using RenderWareFile;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class RWAnalyze : Form
    {
        private RWSection[] data;

        public RWAnalyze(RWSection[] data)
        {
            InitializeComponent();
            TopMost = true;
            this.data = data;
            propertyGridData.SelectedObject = this.data;
        }
    }
}