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

		private void SzsEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (SaveToArchiveList.Count != 0)
			{
				if (MessageBox.Show("There are some editors opened from files of this SZS, this will close all of them and all the unsaved edits will be lost, do you want to continue ?", "", MessageBoxButtons.YesNo) == DialogResult.No)
					e.Cancel = true;
				else
				{
					foreach (var k in SaveToArchiveList.Keys)
						k.Close();
					SaveToArchiveList.Clear();
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
				File.WriteAllBytes(Path.Combine(dlg.SelectedPath, f), loadedSarc.Files[f]);
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

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var sav = new SaveFileDialog() { Filter = "szs file|*.szs|sarc file|*.sarc" };
			if (sav.ShowDialog() != DialogResult.OK)
				return;
			File.WriteAllBytes(sav.FileName, PackArchive());
		}

		private void listBox1_DoubleClick(object sender, EventArgs e)
		{
			if (listBox1.SelectedItem == null)
				return;
			var Fname = listBox1.SelectedItem.ToString();
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
	}
}
