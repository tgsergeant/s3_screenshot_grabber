using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace s3grabber
{
    public partial class FormCutting : Form
    {
        private Bitmap bitmap;

        private Point dragStart;
        private Point curPos;

        private bool dragging;

        private Mutex mutex;

        public FormCutting(Bitmap b)
        {
            this.bitmap = b;

            bool flag;
            mutex = new Mutex(true, "s3grabber.FormCutting", out flag);
            if (!flag)
            {
                Cleanup();
                Close();
            }
            else
            {
                this.TopMost = true;
                this.DoubleBuffered = true;

                dragging = false;

                InitializeComponent();
                Rectangle screen = Grabber.GetScreenSize();
                this.Size = new Size(screen.Width, screen.Height);
                Show();
                this.Location = new Point(screen.X, screen.Y);
            }
        }

        private void Cleanup()
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
            if (mutex != null)
            {
                mutex.Close();
                mutex.Dispose();
                mutex = null;
            }
        }

        private void panelCutting_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(bitmap, 0, 0);
            using (Brush brush = new SolidBrush(Color.FromArgb(50, 33, 33, 33)))
            using (Pen pen = new Pen(Color.White))
            {
                StringFormat stringFormat = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                using (Font f = new Font("Arial", 20))
                {
                    e.Graphics.DrawString("Press ESCAPE to cancel", f, brush,  Screen.PrimaryScreen.WorkingArea, stringFormat);
                }

                if (dragging)
                {
                    Rectangle rec = new Rectangle(dragStart.X, dragStart.Y, curPos.X - dragStart.X, curPos.Y - dragStart.Y);

                    e.Graphics.DrawRectangle(pen, rec);
                    e.Graphics.FillRectangle(brush, rec);
                }
            }
        }

        private void FormCutting_Deactivate(object sender, EventArgs e)
        {
            Cleanup();
            Close();
        }

        private void FormCutting_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                if (dragging)
                {
                    dragging = false;
                    dragStart = Point.Empty;
                    curPos = Point.Empty;
                    panelCutting.Invalidate();
                }
                else
                {
                    Close();
                }
            }
        }

        private void panelCutting_MouseDown(object sender, MouseEventArgs e)
        {
            dragStart = new Point(e.X, e.Y);
            dragging = true;
            panelCutting.Invalidate();
        }

        public class CuttingPanel : Panel
        {
            public CuttingPanel()
            {
                SetStyle(ControlStyles.ResizeRedraw, true); 
                SetStyle(ControlStyles.UserPaint, true); 
                SetStyle(ControlStyles.DoubleBuffer, true); 
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            }
        }

        private void panelCutting_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                curPos.X = e.X;
                curPos.Y = e.Y;              
            }
            panelCutting.Invalidate();
        }

        private void panelCutting_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;

            int width = curPos.X - dragStart.X;
            int height = curPos.Y - dragStart.Y;

            if(width <= 0 || height <= 0 || dragStart.X + width > bitmap.Width  || dragStart.Y + height > bitmap.Height)
                return;

            Rectangle cutRect = new Rectangle(dragStart.X, dragStart.Y, width, height);
            Bitmap cutBitmap = bitmap.Clone(cutRect, bitmap.PixelFormat);

            dragStart = Point.Empty;
            curPos = Point.Empty;
            panelCutting.Invalidate();

            Hide();
            Grabber.EncodeAndUpload(cutBitmap);
            Cleanup();
            Close();
        }

        private void FormCutting_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cleanup();
        }

    }
}
