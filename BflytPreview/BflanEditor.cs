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
using SwitchThemes.Common;
using BflytPreview.EditorForms;
using Newtonsoft.Json;
using SwitchThemes.Common.Serializers;

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
			if (file == null) this.Close();
			UpdateTreeview();
			FormBringToFront();
		}

		void UpdateByteOrder()
		{
			ByteOrderToolStripMenuItem.Text =
				file.byteOrder == Syroot.BinaryData.ByteOrder.BigEndian ?
				"Big endian (Wii u)" : "Little endian (Switch/3DS)";
			SwitchByteOrderToolStripMenuItem.Text =
				file.byteOrder == Syroot.BinaryData.ByteOrder.BigEndian ?
				"Change to little endian" : "Change to big endian";
		}

		void UpdateTreeview()
		{
			UpdateByteOrder();
			treeView1.Nodes.Clear();
			foreach (var section in file.Sections)
			{
				var node = treeView1.Nodes.Add(section.ToString());
				node.Tag = section;
				if (section is Pai1Section)
					PaiSectionToNode((Pai1Section)section, node);
			}
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

		private void AddEntryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var selected = treeView1.SelectedNode.Tag;
			if (selected is Pai1Section)
			{
				var sect = (Pai1Section)selected;
				sect.Entries.Add(new Pai1Section.PaiEntry() { Name = "NEW-" });
			}
			else if (selected is Pai1Section.PaiEntry)
			{
				var entry = (Pai1Section.PaiEntry)selected;
				entry.Tags.Add(new Pai1Section.PaiTag() { TagType = "NEW-" });
			}
			else if (selected is Pai1Section.PaiTag)
			{
				var tag = (Pai1Section.PaiTag)selected;
				tag.Entries.Add(new Pai1Section.PaiTagEntry());
			}
			else
			{
				MessageBox.Show("Can't add an entry to the selected object");
				return;
			}
			UpdateTreeview();
		}

		private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var selected = treeView1.SelectedNode?.Tag;
			var parent = treeView1.SelectedNode?.Parent?.Tag;
			if (selected is Pai1Section.PaiEntry)
			{
				var par = (Pai1Section)parent;
				par.Entries.Remove((Pai1Section.PaiEntry)selected);
			}
			else if (selected is Pai1Section.PaiTag)
			{
				var par = (Pai1Section.PaiEntry)parent;
				par.Tags.Remove((Pai1Section.PaiTag)selected);
			}
			else if (selected is Pai1Section.PaiTagEntry)
			{
				var par = (Pai1Section.PaiTag)parent;
				par.Entries.Remove((Pai1Section.PaiTagEntry)selected);
			}
			else
			{
				MessageBox.Show("Can't remove this element");
				return;
			}
			UpdateTreeview();
		}

		private void ExportToJSONToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog sav = new SaveFileDialog() { Filter = "JSON file|*.json" };
			if (sav.ShowDialog() != DialogResult.OK) return;
			File.WriteAllText(sav.FileName, JsonConvert.SerializeObject(BflanSerializer.Serialize(file)));
		}

		private void ImportFromJSONToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog opn = new OpenFileDialog() { Filter = "JSON file|*.json" };
			if (opn.ShowDialog() != DialogResult.OK) return;
			var SerializedFile = JsonConvert.DeserializeObject<BflanSerializer>(File.ReadAllText(opn.FileName));
			var newFile = SerializedFile.Deserialize();
			if (file != null && newFile.Version != file.Version)
			{
				if (MessageBox.Show("Do you want to keep the format version of the original file ?\n(You should for custom themes)", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
					newFile.Version = file.Version;
			}
			file = newFile;
			UpdateTreeview();
		}

		public static Form OpenFromJson()
		{
			var b = new BflanEditor(null);
			b.ImportFromJSONToolStripMenuItem_Click(null,null);
			if (b.file == null) return null;
			return b;
		}

		private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			treeView1.Invalidate();
		}

		private void SwitchByteOrderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			file.byteOrder = file.byteOrder == Syroot.BinaryData.ByteOrder.BigEndian ?
				Syroot.BinaryData.ByteOrder.LittleEndian : Syroot.BinaryData.ByteOrder.BigEndian;
			UpdateByteOrder();
		}
	}
}
