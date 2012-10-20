using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace s3grabber
{
    public partial class FormMain
    {

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHooks(this.Handle);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Constants.WM_HOTKEY_MSG_ID)
            {
                if (!CheckSettings())
                {
                    MessageBox.Show("Settings incomplete, please check them.");
                    return;
                }
                // Full screenshot
                if ((int)m.WParam == 0)
                {
                    Grabber.EncodeAndUpload(null);
                }
                // Partial screenshot
                else if ((int)m.WParam == 1)
                {
                    Grabber.CaptureAndCrop();
                }
            }
            base.WndProc(ref m);
        }

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public static void RegisterHooks(IntPtr handle)
        {
            RegisterHotKey(handle, 0, Constants.CTRL + Constants.SHIFT, (int)Keys.D3);
            RegisterHotKey(handle, 1, Constants.CTRL + Constants.SHIFT, (int)Keys.D4);
        }

        public static void UnregisterHooks(IntPtr handle)
        {
            UnregisterHotKey(handle, 0);
            UnregisterHotKey(handle, 1);
        }

        public static class Constants
        {
            public const int NOMOD = 0x0000;
            public const int ALT = 0x0001;
            public const int CTRL = 0x0002;
            public const int SHIFT = 0x0004;
            public const int WIN = 0x0008;

            public const int WM_HOTKEY_MSG_ID = 0x0312;
        }
    }
}
