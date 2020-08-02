using System;
using System.Windows.Forms;
using Thermor.Utility;

namespace Thermor
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
            Application.Run(new MainForm());

        }
    }
}
