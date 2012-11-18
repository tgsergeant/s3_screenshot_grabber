using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace s3grabber
{
    public class Grabber
    {

        private static Bitmap CaptureScreen()
        {
            Rectangle size = GetScreenSize();

            Bitmap bmp = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                foreach (Screen s in Screen.AllScreens)
                {
                    g.CopyFromScreen(s.Bounds.X,
                                     s.Bounds.Y,
                                     s.Bounds.X - size.X, //Adjust so the top-left is at (0, 0)
                                     s.Bounds.Y - size.Y,
                                     s.Bounds.Size,
                                     CopyPixelOperation.SourceCopy);
                }
            }

            return bmp;
        }

        public static Rectangle GetScreenSize()
        {
            int maxwidth = 0;
            int maxheight = 0;
            int minX = 0, minY = 0;

            foreach (Screen s in Screen.AllScreens)
            {
                minX = Math.Min(minX, s.Bounds.X);
                minY = Math.Min(minY, s.Bounds.Y);
            }


            foreach (Screen s in Screen.AllScreens)
            {
                maxwidth = Math.Max(maxwidth, s.Bounds.X + s.Bounds.Width - minX);
                maxheight = Math.Max(maxheight, s.Bounds.Y + s.Bounds.Height - minY);

            }
            //This rectangle goes from the top left to the bottom right, including every screen
            return new Rectangle(minX, minY, maxwidth, maxheight);
        }

        private static ImageCodecInfo GetEncoder()
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo i in encoders)
            {
                if (String.Compare(i.FormatDescription, Program.Config.ImageType, true) == 0)
                {
                    return i;
                }
            }
            MessageBox.Show("That encoder (" + Program.Config.ImageType + " is not available on your system, please pick another");
            return null;
        }

        private static EncoderParameters GetEncoderParams()
        {
            EncoderParameters encoderParams = new EncoderParameters(1);
            EncoderParameter param = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, long.Parse(Program.Config.ImageQuality));
            encoderParams.Param[0] = param;
            return encoderParams;
        }

        private static MemoryStream EncodeBitmap(Bitmap bmp)
        {
            ImageCodecInfo encoder = GetEncoder();
            EncoderParameters encoderParams = GetEncoderParams();

            if (encoder == null)
            {
                return null;
            }

            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, encoder, encoderParams);
            bmp.Dispose();

            return ms;
        }

        public static void EncodeAndUpload(Bitmap bmp)
        {
            try
            {
                if (bmp == null)
                {
                    bmp = CaptureScreen();
                }
                if (bmp == null)
                {
                    throw new Exception("Unable to capture a bitmap.");
                }

                MemoryStream ms = EncodeBitmap(bmp);

                if (ms != null)
                {
                    Uploader.Upload(ms);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public static void CaptureAndCrop()
        {
            try
            {
                FormCutting form = new FormCutting(CaptureScreen());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

    }
}
