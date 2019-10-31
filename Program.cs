using System;
using System.Windows.Forms;
using ThermoMate.Utility;

namespace ThermoMate
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            if (SingleInstance.IsRunning())
            {
                SingleInstance.ShowRunningInstance();
            }
            else
            {
                CreateInstance();
            }
        }

        private static void CreateInstance()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var title = string.Format("XNote Ver{0}", Application.ProductVersion);
            Application.Run(new MainForm { Text = title });

        }
    }
}
