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
	public partial class SzsEditor : Form, IFileProvider, IFormSaveToArchive
	{
		IFileProvider _parentArch;
		public IFileProvider ParentArchive { get => _parentArch; set { _parentArch = value; saveToSzsToolStripMenuItem.Visible = _parentArch != null; } }

		SARCExt.SarcData loadedSarc;
        Form1 MainForm;

        public SzsEditor(SARCExt.SarcData _sarc, Form1 _parentForm)
		{
			InitializeComponent();
			loadedSarc = _sarc;
			MainForm = _parentForm;
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

		Dictionary<Form, string> SaveToArchiveList = new Dictionary<Form, string>();
		public void SaveToArchive(byte[] Data, IFormSaveToArchive ChildForm)
		{
			var form = ChildForm as Form;
			if (!SaveToArchiveList.ContainsKey(form))
			{
				MessageBox.Show("Internal error: Wrong argument");
				return;
			}
			if (!loadedSarc.Files.ContainsKey(SaveToArchiveList[form]))
			{
				MessageBox.Show("The file has been removed from the archive !");
				return;
			}
			loadedSarc.Files[SaveToArchiveList[form]] = Data;
		}

		public void EditorClosed(IFormSaveToArchive ChildForm)
		{
			var form = ChildForm as Form;
			if (SaveToArchiveList.ContainsKey(form))
				SaveToArchiveList.Remove(form);
		}

		private void SzsEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (SaveToArchiveList.Count != 0)
			{
				if (MessageBox.Show("There are some files of this SZS opened, this will close all of them and all the unsaved edits will be lost, do you want to continue ?", this.Text, MessageBoxButtons.YesNo) == DialogResult.No)
					e.Cancel = true;
				else
				{
					var toClose = SaveToArchiveList.Keys.ToArray();
					foreach (var k in toClose)
						k.Close();
					if (SaveToArchiveList.Count != 0)
					{
						MessageBox.Show("Some child forms have not been closed");
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
				return SARC.PackN(loadedSarc).Item2;
			else
			{
				var s = SARC.PackN(loadedSarc);
				return ManagedYaz0.Compress(s.Item2, (int)numericUpDown1.Value, s.Item1);
			}
		}

        void SaveSzsAs()
        {
            var sav = new SaveFileDialog() { Filter = "szs file|*.szs|sarc file|*.sarc" };
            if (sav.ShowDialog() != DialogResult.OK)
                return;
            File.WriteAllBytes(sav.FileName, PackArchive());
        }

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
            SaveSzsAs();

        }

		private void listBox1_DoubleClick(object sender, EventArgs e)
		{
			if (listBox1.SelectedItem == null)
				return;
			var Fname = listBox1.SelectedItem.ToString();
			if (SaveToArchiveList.Values.Contains(Fname))
			{
				MessageBox.Show("This file is already opened in another editor");
				return;
			}
			var form = MainForm.OpenFile(loadedSarc.Files[Fname], Fname);
			if (form is IFormSaveToArchive)
			{
				((IFormSaveToArchive)form).ParentArchive = this;
				SaveToArchiveList.Add(form, Fname);
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
			_parentArch.SaveToArchive(PackArchive(), this);
		}

		void FormBringToFront()
		{
			this.Activate();
			this.BringToFront();
			this.Focus();
		}

		private void SzsEditor_Click(object sender, EventArgs e) => FormBringToFront();
		private void SzsEditor_LocationChanged(object sender, EventArgs e) => FormBringToFront();

		private void SzsEditor_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (ParentArchive != null)
				ParentArchive.EditorClosed(this);
		}

		private void thisFileIsTheOriginalSzsToolStripMenuItem_Click(object sender, EventArgs e)
			=> new LayoutDiffForm(loadedSarc, null).ShowDialog();

		private void thisFileIsTheEditedSzsToolStripMenuItem_Click(object sender, EventArgs e)
			=> new LayoutDiffForm(null, loadedSarc).ShowDialog();

        private void SzsEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Shift && e.KeyCode == Keys.S) SaveSzsAs();
            else if (e.Control && e.KeyCode == Keys.S && _parentArch != null) _parentArch.SaveToArchive(PackArchive(), this);
        }

        private void loadJSONPatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var opn = new OpenFileDialog();
            if (opn.ShowDialog() != DialogResult.OK) return;
			bool useAnimation = MessageBox.Show("Do you want to patch animations as well ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes;
			bool use8Fixes = MessageBox.Show("Enable 8.x default layout fixes ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes;

			SzsPatcher P = new SzsPatcher(loadedSarc, new List<PatchTemplate>());
			LayoutPatch JSONLayout = LayoutPatch.LoadTemplate(File.ReadAllText(opn.FileName));

			if (JSONLayout.IsCompatible(loadedSarc))
            {
                var layoutRes = P.PatchLayouts(JSONLayout,"", use8Fixes);
				var AnimRes = !useAnimation || P.PatchAnimations(JSONLayout.Anims);
				if (layoutRes && AnimRes)
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
	}
}
