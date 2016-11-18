using System;
using System.Runtime.InteropServices;
using System.Windows.Forms.Integration;

namespace GTurtle
{
    public class KeyPreviewElementHost : ElementHost
    {
        public KeyPreviewElementHost()
        {
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message m, System.Windows.Forms.Keys keyData)
        {
            bool processed = base.ProcessCmdKey(ref m, keyData);

            if (!processed)
            {
                SendMessage(Parent.Handle, m.Msg, m.WParam, m.LParam);
            }

            return processed;
        }
    }
}
