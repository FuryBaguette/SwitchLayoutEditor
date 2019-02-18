using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Net;
using System.IO;
using System.IO.Compression;

namespace BflytPreview
{
    public partial class UpdateForm : Form
    {

        string NewVersion;
        
        public UpdateForm()
        {
            InitializeComponent();
        }

        private void checkVersionBtn_Click(object sender, EventArgs e)
        {
            System.Xml.XmlDocument UpdateInfo = new System.Xml.XmlDocument();
            UpdateInfo.LoadXml(GetWebPage("https://furybaguette.github.io/SwitchLayoutEditor/EditorAutoUpdate.xml"));
            string Url = UpdateInfo.SelectSingleNode("//url").InnerText;
            NewVersion = UpdateInfo.SelectSingleNode("//version").InnerText;

            UpdateLabelText(updateLabel, "Latest version: " + NewVersion + "\nCurrent Version: " + Form1.AppVersion);
            //UpdateLabelText(changelogLabel, "Changelog: \n" + UpdateInfo.SelectSingleNode("//changelog").InnerText);
            richTextBox1.Visible = true;
            richTextBox1.Text = "Changelog: " + UpdateInfo.SelectSingleNode("//changelog").InnerText;
            
            if (IsLatestVersion(Form1.AppVersion, NewVersion))
                MessageBox.Show("You're already using the latest version");
            else
            {
                DialogResult result = MessageBox.Show("Continuing will delete any unsaved changes. Please save before updating", "Save your changes", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    if (NewVersion != Form1.AppVersion)
                    {
                        using (WebClient wc = new WebClient())
                        {
                            wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                            wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                            UpdateLabelText(activityLabel, "Activity: Downloading update");
                            wc.DownloadFileAsync(new System.Uri(Url), Path.GetDirectoryName(Application.ExecutablePath) + "\\update.zip");
                        }
                    }
                }
            }
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string ExcutablePath = Path.GetDirectoryName(Application.ExecutablePath);
            string ZipUpdate = ExcutablePath + "\\update.zip";
            string DestinationPath = Path.Combine(ExcutablePath, NewVersion);

            UpdateLabelText(activityLabel, "Activity: Download completed. Extracting files");
            Directory.CreateDirectory(DestinationPath);
            if (File.Exists(ZipUpdate))
            {
                using (ZipArchive archive = ZipFile.OpenRead(ZipUpdate))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        entry.ExtractToFile(Path.Combine(DestinationPath, entry.FullName), true);
                        UpdateLabelText(activityLabel, "Activity: Download completed. Extracting file: " + entry.FullName);
                    }
                }

                UpdateLabelText(activityLabel, "Activity: Extraction complete. Cleaning up... ");
                BackupCurrentFiles(DestinationPath, ExcutablePath);
                MoveUpdateFiles(DestinationPath, ExcutablePath);
                CleanUpdateFiles(DestinationPath, ExcutablePath, ZipUpdate);
                UpdateLabelText(activityLabel, "Activity: Update done. Please restart to apply update ");

                DialogResult result = MessageBox.Show("You need to restart to apply update", "Update Finished", MessageBoxButtons.OK);
                if (result == DialogResult.OK)
                {
                    Application.Restart();
                }
            }
            else MessageBox.Show("Couldn't find update zip");
        }

        private void BackupCurrentFiles (string DestinationPath, string ExcutablePath)
        {
            List<String> CurrentFiles = Directory.GetFiles(DestinationPath).ToList();

            foreach (string file in CurrentFiles)
            {
                FileInfo mFile = new FileInfo(file);
                string path = Path.Combine(ExcutablePath, mFile.Name) + ".bak";
                mFile.MoveTo(path);
            }
        }

        private void MoveUpdateFiles(string DestinationPath, string ExcutablePath)
        {
            List<String> UpdateFiles = Directory.GetFiles(DestinationPath).ToList();

            foreach (string file in UpdateFiles)
            {
                FileInfo mFile = new FileInfo(file);
                mFile.MoveTo(Path.Combine(ExcutablePath, mFile.Name));
            }
        }

        private void CleanUpdateFiles(string DestinationPath, string ExcutablePath, string ZipUpdate)
        {
            List<String> BackupFiles = Directory.GetFiles(ExcutablePath).ToList();
            foreach (string file in BackupFiles)
            {
                FileInfo mFile = new FileInfo(file);
                if (mFile.Name.Contains(".bak"))
                    mFile.Delete();
            }
            File.Delete(ZipUpdate);
            Directory.Delete(DestinationPath);
        }

        private void UpdateLabelText(Label label, string LabelText)
        {
            label.Text = LabelText;
        }

        private bool IsLatestVersion(string CurrentVer, string NewVer)
        {
            var version1 = new Version(CurrentVer);
            var version2 = new Version(NewVer);

            if (version1.CompareTo(version2) >= 0)
                return true;
            else
                return false;
        }

        public string GetWebPage(string URL)
        {
            System.Net.HttpWebRequest Request = (HttpWebRequest)(WebRequest.Create(new Uri(URL)));
            Request.Method = "GET";
            Request.MaximumAutomaticRedirections = 4;
            Request.MaximumResponseHeadersLength = 4;
            Request.ContentLength = 0;

            StreamReader ReadStream = null;
            HttpWebResponse Response = null;
            string ResponseText = string.Empty;

            try
            {
                Response = (HttpWebResponse)(Request.GetResponse());
                Stream ReceiveStream = Response.GetResponseStream();
                ReadStream = new StreamReader(ReceiveStream, System.Text.Encoding.UTF8);
                ResponseText = ReadStream.ReadToEnd();
                Response.Close();
                ReadStream.Close();

            }
            catch (Exception ex)
            {
                ResponseText = string.Empty;
            }

            return ResponseText;
        }
    }
}
