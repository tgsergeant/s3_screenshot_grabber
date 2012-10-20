using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace s3grabber
{
    static class Program
    {

        public static Properties.Settings Config
        {
            get
            {
                return Properties.Settings.Default;
            }
        }

        private static Mutex m;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool flag;
            m = new Mutex(true, "s3grabber.Program", out flag);
            if (!flag)
            {
                MessageBox.Show("Another instance is already running, check the tray icon area.");
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain());
            }
        }

    }
}
