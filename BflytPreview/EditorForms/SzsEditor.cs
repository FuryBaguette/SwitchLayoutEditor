using SARCExt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SwitchThemes.Common;
using SwitchThemes.Common.Serializers;
using SwitchThemes.Common.Bntxx;
using Syroot.BinaryData;
using SwitchThemes.Common.Bflyt;

namespace BflytPreview.EditorForms
{
	public partial class SzsEditor : Form
	{
		public class SzsFileProvider : IFileWriter
		{
			private SzsEditor Parent;
			internal Form EditorForm;
			public string Path { get; internal set; }

			public SzsFileProvider(SzsEditor parent, string path) =>
				(Parent, Path) = (parent, path);

			public void EditorClosed() =>
				Parent.CloseFileProvider(this);

			public void Save(byte[] Data) =>
				Parent.SaveFromProvider(this, Data);

			public override string ToString() => $"Szs file : {Path}";
		}

		internal List<SzsFileProvider> FileProviders = new List<SzsFileProvider>();

		internal void CloseFileProvider(SzsFileProvider file) =>
			FileProviders.Remove(file);

		internal void SaveFromProvider(SzsFileProvider file, byte[] Data)
		{
			if (!FileProviders.Contains(file)) throw new Exception("file is not a registered IFileWriter");
			loadedSarc.Files[file.Path] = Data;
		}

		IFileWriter _saveTo;
		public IFileWriter SaveTo 
		{
			get => _saveTo; 
			set 
			{
				_saveTo?.EditorClosed();
				_saveTo = value;
				saveToSzsToolStripMenuItem.Visible = _saveTo != null;
				this.Text = value?.ToString() ?? "";
			}
		}

		SarcData loadedSarc;
        Form1 MainForm;

		public SzsEditor(SARCExt.SarcData _sarc, IFileWriter saveTo, Form1 _parentForm)
		{
			InitializeComponent();
			loadedSarc = _sarc;
			MainForm = _parentForm;
			SaveTo = saveTo;
		}

		private void SzsEditor_Load(object sender, EventArgs e)
		{
			if (loadedSarc == null)
			{
				MessageBox.Show("No sarc has been loaded");
				this.Close();
			}
			else
			{
				listBox1.Items.AddRange(loadedSarc.Files.Keys.ToArray());
				FormBringToFront();
			}
		}

		private void SzsEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (FileProviders.Count != 0)
			{
				if (MessageBox.Show("There are some files of this SZS opened, this will close all of them and all the unsaved edits will be lost, do you want to continue ?", this.Text, MessageBoxButtons.YesNo) == DialogResult.No)
					e.Cancel = true;
				else
				{
					foreach (var k in FileProviders.ToArray())
						k.EditorForm?.Close();
					if (FileProviders.Count != 0) 
					{
						MessageBox.Show($"Failed to close {FileProviders.Count} editors");
						e.Cancel = true;
					}
				}
			}
		}

		private void extractAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExtractMultipleFiles(loadedSarc.Files.Keys.ToArray());
		}

		void ExtractMultipleFiles(IEnumerable<string> files)
		{
			var dlg = new FolderBrowserDialog();
			if (dlg.ShowDialog() != DialogResult.OK)
				return;
			foreach (string f in files)
			{
				string fOut = Path.Combine(dlg.SelectedPath, f);
				DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(fOut));
				if (!dir.Exists)
					dir.Create();
				File.WriteAllBytes(fOut, loadedSarc.Files[f]);
			}
		}

		private void extractToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedItems.Count > 1)
				ExtractMultipleFiles(listBox1.SelectedItems.Cast<string>());
			else
			{
				var sav = new SaveFileDialog() { FileName = listBox1.SelectedItem.ToString() };
				if (sav.ShowDialog() != DialogResult.OK)
					return;
				File.WriteAllBytes(sav.FileName, loadedSarc.Files[listBox1.SelectedItem.ToString()]);
			}
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (loadedSarc.HashOnly)
			{
				MessageBox.Show("Can't remove files from a hash only sarc");
				return;
			}
			string[] Targets = listBox1.SelectedItems.Cast<string>().ToArray();
			foreach (var item in Targets)
			{
				loadedSarc.Files.Remove(item);
				listBox1.Items.Remove(item);
			}
		}

		private void addFilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (loadedSarc.HashOnly)
			{
				MessageBox.Show("Can't add files to a hash only sarc");
				return;
			}
			var opn = new OpenFileDialog() { Multiselect = true };
			if (opn.ShowDialog() != DialogResult.OK)
				return;
			foreach (var f in opn.FileNames)
			{
				string name = Path.GetFileName(f);
				if (InputDialog.Show("File name", "Write the name for this file, use / to place it in a folder", ref name) != DialogResult.OK)
					return;

				if (loadedSarc.Files.ContainsKey(name))
				{
					MessageBox.Show($"File {name} already in szs");
					continue;
				}
				loadedSarc.Files.Add(name, File.ReadAllBytes(f));
				listBox1.Items.Add(name);
			}
		}

		byte[] PackArchive()
		{
			if (numericUpDown1.Value == 0)
				return SARC.Pack(loadedSarc).Item2;
			else
			{
				var s = SARC.Pack(loadedSarc);
				return ManagedYaz0.Compress(s.Item2, (int)numericUpDown1.Value, s.Item1);
			}
		}

        void SaveSzsAs()
        {
            var sav = new SaveFileDialog() { Filter = "szs file|*.szs|sarc file|*.sarc" };
            if (sav.ShowDialog() != DialogResult.OK)
                return;
			SaveTo = new DiskFileProvider(sav.FileName);
			SaveTo.Save(PackArchive());
        }

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) =>
            SaveSzsAs();

		private void listBox1_DoubleClick(object sender, EventArgs e)
		{
			if (listBox1.SelectedItem == null)
				return;
			var Fname = listBox1.SelectedItem.ToString();

			var alreadyOpened = FileProviders.Where(x => x.Path == Fname);
			if (alreadyOpened.FirstOrDefault() != null)
			{
				alreadyOpened.FirstOrDefault().EditorForm.Focus();
				return;
			}

			var provider = new SzsFileProvider(this, Fname);
			var form = MainForm.OpenFile(loadedSarc.Files[Fname], provider);
			if (form != null)
			{
				provider.EditorForm = form;
				FileProviders.Add(provider);
			}
		}

		private void replaceToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedItem == null) return;
			var opn = new OpenFileDialog();
			if (opn.ShowDialog() != DialogResult.OK) return;
			loadedSarc.Files[listBox1.SelectedItem.ToString()] = File.ReadAllBytes(opn.FileName);
		}

		private void copyNameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedItem == null) return;
			Clipboard.SetText(listBox1.SelectedItem.ToString());
		}

		private void renameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedItem == null) return;
			string originalName = listBox1.SelectedItem.ToString();
			string name = Path.GetFileName(originalName);
			if (InputDialog.Show("File name", "Write the name for this file, use / to place it in a folder", ref name) != DialogResult.OK)
				return;

			if (loadedSarc.Files.ContainsKey(name))
			{
				MessageBox.Show($"File {name} already in szs");
				return;
			}
			loadedSarc.Files.Add(name, loadedSarc.Files[originalName]);
			loadedSarc.Files.Remove(originalName);
			listBox1.Items.Add(name);
			listBox1.Items.Remove(originalName);
		}

		private void saveToSzsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (SaveTo == null)
				SaveSzsAs();
			else SaveTo.Save(PackArchive());
		}

		void FormBringToFront()
		{
			this.Activate();
			this.BringToFront();
			this.Focus();
		}

		private void SzsEditor_Click(object sender, EventArgs e) => FormBringToFront();
		private void SzsEditor_LocationChanged(object sender, EventArgs e) => FormBringToFront();

		private void SzsEditor_FormClosed(object sender, FormClosedEventArgs e) =>
			SaveTo?.EditorClosed();

		private void thisFileIsTheOriginalSzsToolStripMenuItem_Click(object sender, EventArgs e)
			=> new LayoutDiffForm(loadedSarc, null).ShowDialog();

		private void thisFileIsTheEditedSzsToolStripMenuItem_Click(object sender, EventArgs e)
			=> new LayoutDiffForm(null, loadedSarc).ShowDialog();

        private void SzsEditor_KeyDown(object sender, KeyEventArgs e)
        {
			e.SuppressKeyPress = true;
			if (e.Shift && e.Control && e.KeyCode == Keys.S)
				saveAsToolStripMenuItem.PerformClick();
			else if (e.Control && e.KeyCode == Keys.S)
				saveToSzsToolStripMenuItem.PerformClick();
			else e.SuppressKeyPress = false;
		}

        private void loadJSONPatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var opn = new OpenFileDialog();
            if (opn.ShowDialog() != DialogResult.OK) return;
			
			SzsPatcher P = new SzsPatcher(loadedSarc);
			LayoutPatch JSONLayout = LayoutPatch.Load(File.ReadAllText(opn.FileName));

			if (JSONLayout.IsCompatible(loadedSarc))
			{
				var layoutRes = P.PatchLayouts(JSONLayout, null);
				if (layoutRes)
				{
					loadedSarc = P.GetFinalSarc();
					MessageBox.Show("Loaded JSON patch");
				}
				else
					MessageBox.Show("Something is wrong with your layout patch.");
            }

        }

		private void listBox1_KeyDown(object sender, KeyEventArgs e)
		{
			if (listBox1.SelectedItem == null) return;
			if (e.KeyCode == Keys.Q)
				HexEditorForm.Show(loadedSarc.Files[listBox1.SelectedItem as string]);
			else if (e.KeyCode == Keys.Return)
				listBox1_DoubleClick(sender, null);
		}

		private void tb_search_TextChanged(object sender, EventArgs e)
		{
			listBox1.Items.Clear();
			if (tb_search.Text.Trim() == "")
				listBox1.Items.AddRange(loadedSarc.Files.Keys.ToArray());
			else
				foreach (var k in loadedSarc.Files.Keys)
					if (k.IndexOf(tb_search.Text, StringComparison.InvariantCultureIgnoreCase) != -1)
						listBox1.Items.Add(k);
		}

		//private void openAllBflanToolStripMenuItem_Click(object sender, EventArgs e)
		//{
		//	foreach (var k in loadedSarc.Files.Keys)
		//		if (k.EndsWith("bflan"))
		//			MainForm.OpenFile(loadedSarc.Files[k], k);
		//}
	}
}
