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
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                    Screen.PrimaryScreen.Bounds.Height,
                                    PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmp))
            {

                g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                 Screen.PrimaryScreen.Bounds.Y,
                                 0,
                                 0,
                                 Screen.PrimaryScreen.Bounds.Size,
                                 CopyPixelOperation.SourceCopy);
            }

            return bmp;
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
