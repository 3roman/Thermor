using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ThermoMate.Utility
{
    public static class SingleInstance
    {
        private const int WS_SHOWNORMAL = 1;

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public static bool IsRunning()
        {
            return null != GetRunningInstance() ? true : false;
        }

        private static Process GetRunningInstance()
        {
            var currentProcess = Process.GetCurrentProcess();
            var sameNameProcesses = Process.GetProcessesByName(currentProcess.ProcessName);
            return sameNameProcesses.Where(process => process.Id != currentProcess.Id).
                FirstOrDefault(process => Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == currentProcess.MainModule.FileName);
        }

        public static void ShowRunningInstance()
        {
            var runningInstance = GetRunningInstance();
            ShowWindowAsync(runningInstance.MainWindowHandle, WS_SHOWNORMAL);
            SetForegroundWindow(runningInstance.MainWindowHandle);
        }
    }
}