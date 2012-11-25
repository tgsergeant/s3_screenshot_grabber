using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace s3grabber
{
    public partial class FormMain
    {

        public Dictionary<String, String> Regions
        {
            get
            {
                if (regions == null)
                {
                    regions = new Dictionary<String, String>();
                    regions.Add("US Standard", "s3.amazonaws.com");
                    regions.Add("US West (Oregon)", "s3-us-west-2.amazonaws.com");
                    regions.Add("US West (Northern California)", "s3-us-west-1.amazonaws.com");
                    regions.Add("EU (Ireland)", "s3-eu-west-1.amazonaws.com");
                    regions.Add("Asia Pacific (Singapore)", "s3-ap-southeast-1.amazonaws.com");
                    regions.Add("Asia Pacific (Tokyo)", "s3-ap-northeast-1.amazonaws.com");
                    regions.Add("Asia Pacific (Sydney)", "s3-ap-southeast-2.amazonaws.com");
                    regions.Add("South America (Sao Paulo)", "s3-sa-east-1.amazonaws.com");
                }
                return regions;
            }
        }

        public bool CheckSettings()
        {
            return
                Program.Config.AccessKey != null && Program.Config.AccessKey.Length > 0 &&
                Program.Config.SecretKey != null && Program.Config.SecretKey.Length > 0 &&
                Program.Config.BucketName != null && Program.Config.BucketName.Length > 0 &&
                Program.Config.ImageType != null && Program.Config.ImageType.Length > 0 &&
                Program.Config.Region != null && Program.Config.Region.Length > 0;
        }
        private void textBoxAccessKey_TextChanged(object sender, EventArgs e)
        {
            Program.Config.AccessKey = textBoxAccessKey.Text;
        }

        void Config_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Program.Config.Save();
        }

        private void textBoxSecretKey_TextChanged(object sender, EventArgs e)
        {
            Program.Config.SecretKey = textBoxSecretKey.Text;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Program.Config.BucketName = textBoxBucketName.Text;
        }

        private void textBoxObjectPrefix_TextChanged(object sender, EventArgs e)
        {
            Program.Config.ObjectPrefix = textBoxObjectPrefix.Text;
        }

        private void textBoxCloudFront_TextChanged(object sender, EventArgs e)
        {
            Program.Config.CloudFront = textBoxCloudFront.Text;
        }

        private void comboBoxRegion_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBoxRegion.SelectedItem != null)
            {
                Program.Config.Region = ((KeyValuePair<String, String>)comboBoxRegion.SelectedItem).Value;
            }
        }

        private void comboBoxImageType_SelectedValueChanged(object sender, EventArgs e)
        {
            Program.Config.ImageType = comboBoxImageType.Text;
        }

        private void trackBarQuality_ValueChanged(object sender, EventArgs e)
        {
            Program.Config.ImageQuality = trackBarQuality.Value.ToString();
        }

        private void checkBoxClipboard_CheckedChanged(object sender, EventArgs e)
        {
            Program.Config.CopyToClipboard = checkBoxClipboard.Checked;
        }

        private void checkBoxDing_CheckedChanged(object sender, EventArgs e)
        {
            Program.Config.DingOnUpload = checkBoxDing.Checked;
        }

        private void folderSelectTextBox_TextChanged(object sender, EventArgs e)
        {
            Program.Config.StorageFolder = folderSelectTextBox.Text;
        }

        private void LoadConfig()
        {
            textBoxAccessKey.Text = Program.Config.AccessKey;
            textBoxSecretKey.Text = Program.Config.SecretKey;
            textBoxBucketName.Text = Program.Config.BucketName;
            textBoxObjectPrefix.Text = Program.Config.ObjectPrefix;
            textBoxCloudFront.Text = Program.Config.CloudFront;

            comboBoxRegion.DataSource = new BindingSource(Regions, null);
            comboBoxRegion.DisplayMember = "Key";
            comboBoxRegion.ValueMember = "Value";

            if (Program.Config.Region == null || !Regions.ContainsValue(Program.Config.Region))
            {
                Program.Config.Region = Regions.Values.First();
            }
            comboBoxRegion.SelectedItem = Regions.First(kv => kv.Value.Equals(Program.Config.Region));

            comboBoxImageType.SelectedItem = Program.Config.ImageType;

            trackBarQuality.Value = int.Parse(Program.Config.ImageQuality);

            checkBoxDing.Checked = Program.Config.DingOnUpload;
            checkBoxClipboard.Checked = Program.Config.CopyToClipboard;

            folderSelectTextBox.Text = Program.Config.StorageFolder;
        }

    }
}
