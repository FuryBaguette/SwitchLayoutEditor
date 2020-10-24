using SwitchThemes.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Syroot.BinaryData;
using System.Threading.Tasks;
using SwitchThemes.Common.Bflyt;
using SwitchThemes.Common.Bflan;
using System.Text;

namespace BflytPreview
{
	public partial class Form1 : Form
	{
		//Increase this by one for each release on github
		// 5 = v 1.0 beta 5
		// 6 = v 1.0 beta 6
		// 7 = v 1.0 beta 7
		// 8 = v 1.0 beta 8
		// 9 = v 1.0 beta 9
		// 10 = v 1.0 beta 10
		// 11 = v 1.0 beta 11
		public const int AppRelease = 11;

		public static void CheckForUpdates(bool showErrors)
		{
			try
			{
				var githubClient = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("SwitchLayoutEditor"));
				var ver = githubClient.Repository.Release.GetAll("FuryBaguette", "SwitchLayoutEditor").GetAwaiter().GetResult();
				if (ver.Count > AppRelease)
				{
					if (MessageBox.Show($"A new version has been found: {ver[0].Name}\r\n{ver[0].Body}\r\nOpen the github page ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
						System.Diagnostics.Process.Start("https://github.com/FuryBaguette/SwitchLayoutEditor/releases/latest");
				}
				else if (showErrors)
					MessageBox.Show("You're running the latest version :)");
			}
			catch (Exception ex)
			{
				if (showErrors)
					MessageBox.Show("Error while searching for updates:\r\n" + ex.ToString());
			}
		}

		public Form1(string[] args = null)
		{
			InitializeComponent();

			foreach (string s in args)
				if (File.Exists(s))
					OpenFileFromDisk(s);
		}

        private void openBFLYTToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog opn = new OpenFileDialog() { Filter = "Supported files (bflyt,szs,bflan)|*.bflyt;*.szs;*.bflan|All files|*.*" };
			if (opn.ShowDialog() != DialogResult.OK) return;
			OpenFileFromDisk(opn.FileName);
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
#if DEBUG
			this.Text += " - This is a debug build, auto updates are disabled.";
#else
			Task.Run(() => CheckForUpdates(false));
#endif
			//#if DEBUG
			//			string AutoLaunch = @"RdtBase.bflyt";
			//			if (!File.Exists(AutoLaunch)) return;
			//			OpenFile(File.ReadAllBytes(AutoLaunch), AutoLaunch);
			//#endif
		}

		public void OpenForm(Form f)
		{
			this.IsMdiContainer = true;
			f.TopLevel = false;
			f.Parent = pnlSubSystem;
			f.Show();
		}

		public Form OpenFileFromDisk(string path) =>
			OpenFile(File.ReadAllBytes(path), new DiskFileProvider(path));

		public Form OpenFile(byte[] File, IFileWriter saveTo)
		{
			string Magic = Encoding.ASCII.GetString(File, 0, 4);
			Form result = null;
			if (Magic == "Yaz0")
				return OpenFile(ManagedYaz0.Decompress(File), saveTo);
			else if (Magic == "SARC")
				result = new EditorForms.SzsEditor(SARCExt.SARC.Unpack(File), saveTo, this);
			else if (Magic == "FLYT")
				result = new EditorView(new BflytFile(File), saveTo);
			else if (Magic == "FLAN")
				result = new BflanEditor(new BflanFile(File), saveTo);
			
			if (result != null)
				OpenForm(result);

			return result;
		}

		private void pnlSubSystem_DragEnter(object sender, DragEventArgs e) 
		{if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;}	

		private void pnlSubSystem_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (string file in files)
				OpenFileFromDisk(file);
		}

		private void layoutDiffToolStripMenuItem_Click(object sender, EventArgs e)
			=> new EditorForms.LayoutDiffForm().Show();

        private void checkUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
			CheckForUpdates(true);
        }

		private void BFLANToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				var f = BflanEditor.OpenFromJson();
				if (f != null) OpenForm(f);
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					"Error while opening the provided file: " + ex.Message + "\r\n" +
					"This often happens when trying to open a json file that doesn't contain animations. " +
					"If you want to open a json layout open the target szs first and then load it from the window that appears.\r\n\r\n" +
					"More details: " + ex.ToString());
			}
		}
	}

	public class Vector3Converter : System.ComponentModel.TypeConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) => true;

		public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			string[] tokens = ((string)value).Split(';');
			return new Vector3(
				float.Parse(tokens[0]),
				float.Parse(tokens[1]),
				float.Parse(tokens[2]));
		}

		public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			var v = (Vector3)value;
			return $"{v.X};{v.Y};{v.Z}";
		}
	}

	public class Vector2Converter : System.ComponentModel.TypeConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) => true;

		public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			string[] tokens = ((string)value).Split(';');
			return new Vector2(
				float.Parse(tokens[0]),
				float.Parse(tokens[1]));
		}

		public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			var v = (Vector2)value;
			return $"{v.X};{v.Y}";
		}
	}
}
