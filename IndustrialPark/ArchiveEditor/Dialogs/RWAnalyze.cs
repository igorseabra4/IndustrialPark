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
            this.data = data;
            propertyGridData.SelectedObject = this.data;
        }
    }
}