using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Steam_BPM_Customizer
{
    public partial class FormLog : Form
    {
        public FormLog()
        {
            InitializeComponent();
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.SelectionLength = 0;
            txtLog.ScrollToCaret();
            
        }



        public void Log(String mesage)
        {
            try
            {
                txtLog.Invoke((MethodInvoker)(() => this.txtLog.Text = mesage ));
            }
            catch { 

                    }
            

            
        }

        private void FormLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void txtLog_ControlAdded(object sender, ControlEventArgs e)
        {

        }

        private void BPM_Log_LogChanged(object sender, BPM_Log_EventArgs e)
        {
            string[] lines = txtLog.Lines;
            //MessageBox.Show("LL" + lines.Length);
            if (lines.Length > 1024) {
                string[] newLines = lines.Skip<string>(100).ToArray();
                txtLog.Lines = newLines;
            }
            
            try
            {
                if (txtLog.IsHandleCreated)
                {
                    txtLog.Invoke(new MethodInvoker(delegate { txtLog.Text += e.Message + "\r\n"; }));
                } else {
                    txtLog.Text += e.Message + "\r\n";
                }
                
                
            } catch
            {

            }
            
        }
    }
}
