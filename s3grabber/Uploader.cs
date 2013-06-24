using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitS3;
using System.Windows.Forms;
using System.IO;
using System.Media;
using System.Reflection;
using System.Net;

namespace s3grabber
{
    public class Uploader
    {

        public enum TestConnectivityResult
        {
            S3_EXCEPTION,
            EXCEPTION,
            BUCKET_DOESNT_EXIST,
            INACCESSIBLE,
            OK,
            UNKNOWN
        };

        static S3Service GetService()
        {
            S3Service service = new S3Service()
            {
                AccessKeyID = Program.Config.AccessKey,
                SecretAccessKey = Program.Config.SecretKey,
                Host = Program.Config.Region,
            };
            service.BeforeAuthorize += service_BeforeAuthorize;
            return service;
        }

        private static void service_BeforeAuthorize(object sender, S3RequestArgs e)
        {
            S3Request req = e.Request;
            req.ServicePoint.Expect100Continue = false;
            req.ReadWriteTimeout = 500;
            req.KeepAlive = false;
        }

        public static String GetObjectName()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var id = new string(
                Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray());

            return Program.Config.ObjectPrefix + id  + "." + Program.Config.ImageType.ToLower();
        }

        public static void Upload(Stream stream)
        {
            Upload(stream, GetObjectName());
        }

        public static void Upload(Stream stream, String objectName)
        {
            S3Service service = GetService();

            long len = stream.Position;
            stream.Seek(0, SeekOrigin.Begin);

            AddObjectRequest req = new AddObjectRequest(service, Program.Config.BucketName, objectName)
            {
                CannedAcl = CannedAcl.PublicRead,
                ContentType = "image/" + Program.Config.ImageType,
                ContentLength = len
            };

            IAsyncResult uploadAsync = req.BeginGetRequestStream(a =>
            {
                using (Stream outStream = req.EndGetRequestStream(a))
                {
                    try
                    {
                        var buffer = new byte[len > 65536 ? 65536 : len];
                        var position = 0;
                        while (position < stream.Length)
                        {
                            var read = stream.Read(buffer, 0, buffer.Length);
                            outStream.Write(buffer, 0, read);
                            position += read;
                        }
                        outStream.Flush();

                        stream.Close();
                        stream.Dispose();
                        stream = null;
                        buffer = null;

                        using (AddObjectResponse response = req.GetResponse())
                        {
                            string url = null;
                            if (Program.Config.CloudFront != null && Program.Config.CloudFront.Length > 0)
                            {
                                url = "http://" + Program.Config.CloudFront + "/" + objectName;
                            }
                            else
                            {
                                url = "http://" + Program.Config.Region + "/" + Program.Config.BucketName + "/" + objectName;
                            }
                            FormMain.DefaultInstance.Invoke((MethodInvoker)delegate
                            {
                                FormMain.DefaultInstance.UploadComplete(url);
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        FormMain.DefaultInstance.Invoke((MethodInvoker)delegate
                        {
                            FormMain.DefaultInstance.UploadFailed();
                        });
                    }
                    finally
                    {
                        //GC.Collect();
                    }
                }
            }, null);
        }

        public static TestConnectivityResult testConnectivity()
        {
            try
            {

                S3Service service = GetService();

                BucketAccess access = service.QueryBucket(Program.Config.BucketName);

                if (access.HasFlag(BucketAccess.NoSuchBucket))
                {
                    return TestConnectivityResult.BUCKET_DOESNT_EXIST;
                }
                else if (access.HasFlag(BucketAccess.NotAccessible))
                {
                    return TestConnectivityResult.INACCESSIBLE;
                }
                else if (access.HasFlag(BucketAccess.Accessible))
                {
                    return TestConnectivityResult.OK;
                }
                else
                {
                    return TestConnectivityResult.UNKNOWN;
                }
            }
            catch (S3Exception e)
            {
                switch (e.ErrorCode)
                {
                    case S3ErrorCode.PermanentRedirect:
                        MessageBox.Show("You are accessing this bucket via the wrong region.");
                        break;
                    case S3ErrorCode.InvalidAccessKeyId:
                    case S3ErrorCode.SignatureDoesNotMatch:
                        MessageBox.Show("There was a problem using your credentials, please check them");
                        break;
                    default:
                        MessageBox.Show("[" + e.ErrorCode + "]: " + e.Message);
                        break;
                }
                return TestConnectivityResult.S3_EXCEPTION;
            }
            catch (Exception e)
            {
                return TestConnectivityResult.EXCEPTION;
            }
        }

    }
}
