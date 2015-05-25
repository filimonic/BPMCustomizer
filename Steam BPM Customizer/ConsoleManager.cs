using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Steam_BPM_Customizer
{
    class ConsoleManager
    {
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void HideConsole() {
            try {
                IntPtr consoleWindow = GetConsoleWindow();
                ShowWindow(consoleWindow,SW_HIDE);
            }
            catch
            {

            }
            
        }

    }
}
