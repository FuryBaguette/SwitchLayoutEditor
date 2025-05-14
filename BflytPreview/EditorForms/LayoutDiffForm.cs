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
using SARCExt;
using SwitchThemes.Common;

namespace BflytPreview.EditorForms
{
	public partial class LayoutDiffForm : Form
	{
		public LayoutDiffForm(SarcData _Original = null, SarcData _Edited = null)
		{
			InitializeComponent();
			if (_Original != null)
			{
				textBox1.Text = "<From file>";
				button1.Enabled = false;
				textBox1.Enabled = false;
				textBox1.AllowDrop = false;
				Original = _Original;
			}
			if (_Edited != null)
			{
				textBox2.Text = "<From file>";
				button2.Enabled = false;
				textBox2.Enabled = false;
				textBox2.AllowDrop = false;
				Edited = _Edited;
			}

			EnableResidentMenuCheckbox(_Original ?? _Edited);
		}

		void EnableResidentMenuCheckbox(SarcData sarc)
		{
			if (cb11Compat.Tag != null) return;
			if (sarc == null) return;		
			cb11Compat.Visible = (DefaultTemplates.GetFor(sarc)?.NXThemeName ?? "home") == "home";
			cb11Compat.Tag = "checked";
		}

		SarcData Original = null;
		SarcData Edited = null;

		private void button1_Click(object sender, EventArgs e) =>
			Original = SelectFile(ref textBox1) ?? Original;

		private void button2_Click(object sender, EventArgs e) =>
			Edited = SelectFile(ref textBox2) ?? Edited;

		private void textBox1_DragDrop(object sender, DragEventArgs e) =>
			Original = SelectFile(ref textBox1, e) ?? Original;

		private void textBox2_DragDrop(object sender, DragEventArgs e) =>
			Edited = SelectFile(ref textBox2, e) ?? Edited;

		private void tbDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		SarcData SelectFile(ref TextBox target, string name = null)
		{
			if (name is null)
			{
				OpenFileDialog opn = new OpenFileDialog() { Filter = "szs files|*.szs" };
				if (opn.ShowDialog() != DialogResult.OK) return null;
				name = opn.FileName;	
			}
			target.Text = name;
			var sarc = SARC.Unpack(ManagedYaz0.Decompress(File.ReadAllBytes(target.Text)));
			EnableResidentMenuCheckbox(sarc);
			return sarc;
		}

		SarcData SelectFile(ref TextBox target, DragEventArgs e)
		{
			var f = (string[])e.Data.GetData(DataFormats.FileDrop);
			if (f.Any())
				return SelectFile(ref target, f.First());
			return null;
		}

		private void LayoutDiffForm_Load(object sender, EventArgs e)
		{

		}

		private void button3_Click(object sender, EventArgs e)
		{
			try
			{
				bool HideOnlineButton = false;
				if (cb11Compat.Visible)
					HideOnlineButton = cb11Compat.Checked;

				var diff = new LayoutDiff(Original, Edited, new LayoutDiff.DiffOptions { 
					HideOnlineButton = HideOnlineButton,
                    IgnoreMissingPanes = cbIgnoreMissingPanes.Checked,
                    // Also ignore these since we can't shouldn't import animations if panes are missing
                    IgnoreAnimations = cbIgnoreMissingPanes.Checked,
                    IgnoreGroups = cbIgnoreMissingPanes.Checked,
                    IgnoreMaterials = cbIgnoreMissingPanes.Checked,
                });
                
				var res = diff.ComputeDiff();
				var log = diff.OutputLog;

                if (!string.IsNullOrWhiteSpace(log))
					MessageBox.Show(log);

				if (res != null)
				{
					SaveFileDialog sav = new SaveFileDialog() { Filter = "json file|*.json" };
					if (sav.ShowDialog() != DialogResult.OK) return;
					File.WriteAllText(sav.FileName, res.AsJson());
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}	
	}
}
