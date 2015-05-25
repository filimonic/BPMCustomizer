using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Steam_BPM_Customizer
{
    class BootStrapMonitor
    {
        private FileStream _bootstrapperFileStream;
        private Thread _loopThread;

        public BootStrapMonitor(string path)
        {
            _bootstrapperFileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _bootstrapperFileStream.Seek(0, SeekOrigin.End);
        }

        private void Loop() {
            List<char> buffer;
            buffer = new List<char>();
            while(true) {
                int dataByteInt = _bootstrapperFileStream.ReadByte();
                if (dataByteInt == -1)
                {
                    //Log("End..");
                    //Thread.Sleep(100);
                    continue;
                }
                char dataByte = (char)dataByteInt;
                //Log(String.Format("Added symbol [{0}]",dataByte));
                buffer.Add( dataByte );
                if (dataByte == '\n') {
                    Log("Got line!");
                    string dataString = new String(buffer.ToArray());
                    buffer.Clear();
                    OnLineAdded(dataString.Trim());
                }
            }
        }

        private void Log (string message) 
        {
           // Console.WriteLine("BootStrapMonitor::" + message);
        }

        protected virtual void OnLineAdded(string e)
        {
            Log(String.Format("Line added [{0}]", e));
            EventHandler<string> handler = LineAdded;
            if (handler != null)
            {
                Log(String.Format("Firing event {0}", e));
                handler(this, e);
            }
        }

        public void Start() 
        {
            _loopThread = new Thread(Loop);
            _loopThread.Start();
        }

        public event EventHandler<string> LineAdded;
        
    }
}
