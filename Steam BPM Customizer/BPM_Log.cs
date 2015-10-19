using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steam_BPM_Customizer
{
    public class BPM_Log_EventArgs : EventArgs
    {
        private readonly string _message;
        public BPM_Log_EventArgs(string message)
        {
            _message = message;
        }
        public string Message
        {
            get
            {
                return _message;
            }
        }
    }
    public class BPM_Log
    {
        public static event EventHandler<BPM_Log_EventArgs> LogChanged;
        private static List<string> _log = new List<string>(1024);

        public static void Log(String message)
        {
            while (_log.Count > 1024)
            {
                _log.RemoveRange(0, 100);
            }
            _log.Add(message);
            Console.WriteLine("LOG::" + message);
            FireEventLogChanged(message);
        }

        private static void FireEventLogChanged(string message)
        {
            EventHandler<BPM_Log_EventArgs> handler = LogChanged;
            if (null != handler)
            {
                handler(null, new BPM_Log_EventArgs(message));
            }
        }
        public static String GetLog()
        {
            return String.Join("\r\n", _log.ToArray());
        }
    }
}
