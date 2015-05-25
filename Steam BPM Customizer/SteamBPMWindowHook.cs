using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace Steam_BPM_Customizer
{

    class SteamBPMWindowHook
    {
        public const int CREATED = 10;
        public const int DESTROYED = 20;
        private Dictionary<IntPtr, string> _windowPreviousList;
        private Dictionary<IntPtr, string> _windowCurrentList;
        private bool _isRunung;
        private string _lookupClassName;
        public SteamBPMWindowHook(string lookupClassName)
        {
            _windowPreviousList = new Dictionary<IntPtr, string>();
            _windowCurrentList = new Dictionary<IntPtr, string>();
            _isRunung = false;
            _lookupClassName = lookupClassName;

        }

        public event EventHandler<int> StateChanged;
        private delegate bool EnumDelegate(IntPtr hWnd, int lParam);
        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool _EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetClassName", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int _GetClassName(IntPtr hwnd, StringBuilder lpClassName, int nMaxCount);

        public void Start()
        {
            Thread _runningThread = new Thread(Run);
            _runningThread.Start();
        }
        private void Run() 
        {
            _isRunung = true;
            while (_isRunung)
            {
                UpdateWindows();
                if (_windowPreviousList.Count == 0)
                {
                    foreach (KeyValuePair<IntPtr,string> windowData in _windowCurrentList) {
                        _windowPreviousList.Add(windowData.Key, windowData.Value);
                    }
                }
                else
                {
                    ProcessWindowsDifference(_lookupClassName);
                }
                Thread.Sleep(250);
            }
            _isRunung = false;
            _windowPreviousList.Clear();
            _windowCurrentList.Clear();
        }

        private void ProcessWindowsDifference(string lookupClassName)
        {
            bool windowWasBefore = _windowPreviousList.Values.Contains(lookupClassName);
            bool windowIsNow = _windowCurrentList.Values.Contains(lookupClassName);
            if (windowWasBefore == windowIsNow)
            {
                return; //state not changed
            }
            else if (windowWasBefore)
            {
                if (StateChanged != null)
                {
                    StateChanged(this, DESTROYED);
                }
            }
            else
            {
                if (StateChanged != null)
                {
                    StateChanged(this, CREATED);
                }
            }
        }

        public void Stop()
        {
            _isRunung = false;
            Thread.Sleep(500);
        }

        private void UpdateWindows()
        {
            _windowPreviousList.Clear();
            _windowPreviousList = new Dictionary<IntPtr,string>(_windowCurrentList);
            _windowCurrentList.Clear();
            IntPtr hCurrentDesktop = IntPtr.Zero;
            EnumDelegate enumWindowsProc = new EnumDelegate(EnumWindowsProc);
            _EnumDesktopWindows(hCurrentDesktop,enumWindowsProc,IntPtr.Zero);
        }

        private string GetWindowClassName(IntPtr hWnd)
        {
            StringBuilder className = new StringBuilder(255);
            int classNameLength = _GetClassName(hWnd, className, className.Capacity);
            className.Length = classNameLength;
            return className.ToString();

        }

        private bool EnumWindowsProc(IntPtr hWnd, int lParam)
        {
            string windowClassName = GetWindowClassName(hWnd);
            _windowCurrentList.Add(hWnd, windowClassName);
            return true;
        }
    }
}
