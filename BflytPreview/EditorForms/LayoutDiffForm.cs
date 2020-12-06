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
				Original = _Original;
			}
			if (_Edited != null)
			{
				textBox2.Text = "<From file>";
				button2.Enabled = false;
				textBox2.Enabled = false;
				Edited = _Edited;
			}

			EnableResidentMenuCheckbox(_Original ?? _Edited);
		}

		void EnableResidentMenuCheckbox(SarcData sarc)
		{
			if (cb11Compat.Tag != null) return;
			if (sarc == null) return;		
			cb11Compat.Visible = (DefaultTemplates.GetFor(sarc)?.NXThemeName ?? "home") == "home";
			cb11Compat.Checked = true;
			cb11Compat.Tag = "checked";
		}

		SarcData Original = null;
		SarcData Edited = null;

		private void button1_Click(object sender, EventArgs e)
			=> Original = SelectFile(ref textBox1) ?? Original;

		private void button2_Click(object sender, EventArgs e)
			=> Edited = SelectFile(ref textBox2) ?? Edited;

		SarcData SelectFile(ref TextBox target)
		{
			OpenFileDialog opn = new OpenFileDialog() { Filter = "szs files|*.szs" };
			if (opn.ShowDialog() != DialogResult.OK) return null;
			target.Text = opn.FileName;
			var sarc = SARC.Unpack(ManagedYaz0.Decompress(File.ReadAllBytes(target.Text)));
			EnableResidentMenuCheckbox(sarc);
			return sarc;
		}

		private void LayoutDiffForm_Load(object sender, EventArgs e)
		{

		}

		private void button3_Click(object sender, EventArgs e)
		{
			try
			{
				bool? HideOnlineButton = null;
				if (cb11Compat.Visible)
					HideOnlineButton = cb11Compat.Checked;

				var (res,msg) = LayoutDiff.Diff(Original, Edited, new LayoutDiff.DiffOptions { HideOnlineButton = HideOnlineButton });
				if (msg != null)
					MessageBox.Show(msg);
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
