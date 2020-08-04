using System.Windows.Forms;

namespace Thermor.Utility
{
    public class ListViewNF : ListView
    {
        public ListViewNF()
        {
            SetStyle(ControlStyles.DoubleBuffer |
                ControlStyles.OptimizedDoubleBuffer |
               ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }
    }
}
