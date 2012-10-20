using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace s3grabber
{
    public partial class FormMain : Form
    {

        private static FormMain defaultInstance;

        public static FormMain DefaultInstance
        {
            get { return defaultInstance; }
            set { defaultInstance = value; }
        }

        private string lastUrl;

        private Dictionary<String, String> regions;


        public FormMain()
        {
            InitializeComponent();
            LoadConfig();

            DefaultInstance = this;

            RegisterHooks(this.Handle);

            if (Environment.GetCommandLineArgs().Contains("/background"))
            {
                WindowState = FormWindowState.Minimized;
            }

            Program.Config.PropertyChanged += Config_PropertyChanged;
        }

        private void LaunchURL(string uri)
        {
            try
            {
                if (Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                {
                    System.Diagnostics.Process.Start(uri);
                }
            }
            catch (Exception e)
            {
                // Silently ignore
            }
        }


        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void notifyIcon_BalloonClick(object sender, EventArgs e)
        {
            LaunchURL(lastUrl);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void linkLabelCredentials_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LaunchURL("https://portal.aws.amazon.com/gp/aws/securityCredentials");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LaunchURL("http://s3.zor.io");
        }

        private void buttonTestCredentials_Click(object sender, EventArgs e)
        {
            if (CheckSettings())
            {
                labelTestResult.Text = Uploader.testConnectivity().ToString();
            }
            else
            {
                MessageBox.Show("Please check your settings first.");
            }
        }

        public void UploadFailed()
        {
            notifyIcon.ShowBalloonTip(1000, "Upload failed", "Please check settings and connectivity", ToolTipIcon.Error);
        }

        public void UploadComplete(String url)
        {
            if (Program.Config.CopyToClipboard)
            {
                Clipboard.SetText(url);
            }
            if (Program.Config.DingOnUpload)
            {
                using (SoundPlayer player = new SoundPlayer(Assembly.GetExecutingAssembly().GetManifestResourceStream("s3grabber.uploaded.wav")))
                {
                    player.Play();
                }
            }
            lastUrl = url;
            listImages.Items.Insert(0, url);
            listImages.SelectedIndex = 0;
            notifyIcon.ShowBalloonTip(1000, "Upload complete", url, ToolTipIcon.Info);
        }
 

        private void listImages_DoubleClick(object sender, EventArgs  e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            int index = listImages.IndexFromPoint(me.Location);
            if (index != ListBox.NoMatches)
            {
                LaunchURL(listImages.Items[index].ToString());
            }
        }

        #region FormMain DragDrop

        private void FormMain_DragDrop(object sender, DragEventArgs e)
        {
            IDataObject drop = e.Data;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string path in files)
            {
                try
                {
                    Image.FromFile(path).Dispose();

                    FileInfo f = new FileInfo(path);

                    FileStream fs = File.OpenRead(path);
                    fs.Seek(0, SeekOrigin.End);

                    Uploader.Upload(fs);
                }
                catch (Exception ex)
                {
                    notifyIcon.ShowBalloonTip(1000, "Error", "That file is not an image", ToolTipIcon.Error);
                }
            }
        }

        private void FormMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }
    }
        #endregion
}
