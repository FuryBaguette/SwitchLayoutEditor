using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using SwitchThemesCommon;
using BflytPreview.EditorForms;

namespace BflytPreview
{
	public partial class BflanEditor : Form, IFormSaveToArchive
	{
		Bflan file = null;
		IFileProvider _parentArch;
		public IFileProvider ParentArchive { get => _parentArch; set { _parentArch = value; saveToArchiveToolStripMenuItem.Visible = _parentArch != null; } }

		public BflanEditor(Bflan _file)
		{
			InitializeComponent();
			file = _file;
		}

		private void BflanEditor_Load(object sender, EventArgs e)
		{
			UpdateTreeview();
			FormBringToFront();
		}

		void UpdateTreeview()
		{
			treeView1.Nodes.Clear();
			foreach (var section in file.Sections)
			{
				var node = treeView1.Nodes.Add(section.ToString());
				node.Tag = section;
				if (section is Pai1Section)
					PaiSectionToNode((Pai1Section)section, node);
			}
		}

		void PatSectionToNode(Pat1Section sect, TreeNode node)
		{
			
		}

		void PaiSectionToNode(Pai1Section sect, TreeNode node)
		{
			foreach (var entry in sect.Entries)
			{
				var subnode = node.Nodes.Add(entry.Name);
				subnode.Tag = entry;
				foreach (var tag in entry.Tags)
				{
					var _subnode = subnode.Nodes.Add(tag.TagType);
					_subnode.Tag = tag;
					foreach (var TagEntry in tag.Entries)
					{
						var __subnode = _subnode.Nodes.Add("[TagEntry]");
						__subnode.Tag = TagEntry;
					}
				}
			}
		}

		private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			propertyGrid1.SelectedObject = e.Node.Tag;
		}

		private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog sav = new SaveFileDialog() { Filter = "Bflan file|*.bflan" };
			if (sav.ShowDialog() != DialogResult.OK) return;
			File.WriteAllBytes(sav.FileName,file.WriteFile());
		}

		private void SaveToArchiveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_parentArch.SaveToArchive(file.WriteFile(), this);
		}

		private void BflanEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (ParentArchive != null)
				ParentArchive.EditorClosed(this);
			Settings.Default.ShowImage = false;
		}

		void FormBringToFront()
		{
			this.Activate();
			this.BringToFront();
			this.Focus();
		}

		private void BflanEditor_Click(object sender, EventArgs e) => FormBringToFront();
		private void BflanEditor_LocationChanged(object sender, EventArgs e) => FormBringToFront();
	}
}
