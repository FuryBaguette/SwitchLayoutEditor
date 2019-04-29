using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BflytPreview
{
	public partial class UpdateForm : Form
	{
		Octokit.Release rel;
		public UpdateForm(Octokit.Release release)
		{
			InitializeComponent();
			rel = release;
		}

		void UpdateStatus(string s)
		{
			Status.Text = s;
			this.Refresh();
		}

		private void UpdateForm_Load(object sender, EventArgs e)
		{
			UpdateStatus("Downloading the update..");
			using (WebClient wc = new WebClient())
			{
				wc.DownloadProgressChanged += wc_DownloadProgressChanged;
				wc.DownloadFileCompleted += wc_DownloadFileCompleted;
				wc.DownloadFileAsync(new System.Uri(rel.Assets[0].BrowserDownloadUrl), Path.GetDirectoryName(Application.ExecutablePath) + "\\update.zip");
			}
		}

		private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			string ExcutablePath = Path.GetDirectoryName(Application.ExecutablePath);
			string ZipUpdate = ExcutablePath + "\\update.zip";
			string DestinationPath = Path.Combine(ExcutablePath, "updateData");
			try
			{
				UpdateStatus("Download completed. Extracting files...");
				Directory.CreateDirectory(DestinationPath);
				if (File.Exists(ZipUpdate))
				{
					using (ZipArchive archive = ZipFile.OpenRead(ZipUpdate))
					{
						foreach (ZipArchiveEntry entry in archive.Entries)
						{
							entry.ExtractToFile(Path.Combine(DestinationPath, entry.FullName), true);
							UpdateStatus("Extracting file: " + entry.FullName);
						}
					}

					UpdateStatus("Extraction complete. Cleaning up... ");
					BackupCurrentFiles(DestinationPath, ExcutablePath);
					MoveUpdateFiles(DestinationPath, ExcutablePath);
					UpdateStatus("Completed. Please restart to apply update ");

					DialogResult result = MessageBox.Show("You need to restart to apply update", "Update Finished", MessageBoxButtons.OK);
					if (result == DialogResult.OK)
					{
						Application.Restart();
					}
				}
				else MessageBox.Show("Couldn't find update zip");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Update failed: " + ex.ToString());
			}
			CleanUpdateFiles(DestinationPath, ExcutablePath, ZipUpdate);
		}

		private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			UpdateStatus("Downloading: " + e.ProgressPercentage.ToString() + "%");
		}

		private void BackupCurrentFiles(string DestinationPath, string ExcutablePath)
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

	}
}
