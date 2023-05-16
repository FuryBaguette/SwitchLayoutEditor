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
using SwitchThemes.Common.Bflan;
using System.Diagnostics;
using BflytPreview.Automation;

namespace BflytPreview
{
	public partial class BflanEditor : Form
	{
		readonly LoadedTemplate[] BflanTemplates;
		
		BflanFile file = null;

		IFileWriter _saveTo;
		public IFileWriter SaveTo 
		{
			get => _saveTo; 
			set
			{
				_saveTo?.EditorClosed();
				_saveTo = value; 
				saveToolStripMenuItem.Visible = _saveTo != null;
				this.Text = value?.ToString() ?? "";
			}
		}

		class LoadedTemplate 
		{
			public BflanTemplate Manifest;
			public string FullPathToTemplate;

			public static LoadedTemplate FromJsonManifest(string path) 
			{
				path = Path.GetFullPath(path);
				var res = new LoadedTemplate
				{
					Manifest = JsonConvert.DeserializeObject<Automation.BflanTemplate>(File.ReadAllText(path))
                };

				if (res.Manifest == null)
					throw new Exception("Couldn't load the json for " + path);

				if (Path.IsPathRooted(res.Manifest.FileName))
				{
					res.FullPathToTemplate = res.Manifest.FileName;
				}
				else 
				{
                    var root = Path.GetDirectoryName(path);
					res.FullPathToTemplate = Path.Combine(root, res.Manifest.FileName);
                }

				if (!File.Exists(res.FullPathToTemplate))
					throw new Exception($"Couldn't find the template file for {path}, tried searchingi n {res.FullPathToTemplate}");
				
				return res;
			}
        }

		public BflanEditor(BflanFile _file, IFileWriter saveTo)
		{
			InitializeComponent();
			file = _file;
			SaveTo = saveTo;

			try
			{
				if (Directory.Exists("BflanTemplates"))
					BflanTemplates = Directory.EnumerateFiles("BflanTemplates", "*.json")
						.Select(x => LoadedTemplate.FromJsonManifest(x))
						.ToArray();
				else
					BflanTemplates = new LoadedTemplate[0];
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error while loading BFLAn template: " + ex.Message + "\r\n\r\nFix the issue then close and reopen this editor to reload the files");
                BflanTemplates = new LoadedTemplate[0];
            }
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
				BflanObjectTonode(section, treeView1.Nodes);
		}

		void BflanObjectTonode(object obj, TreeNodeCollection parent)
		{
            var subnode = parent.Add(obj.ToString());
			subnode.Tag = obj;
			BflanObjectChildrenToNode(obj, subnode);
		}

		void BflanObjectChildrenToNode(object obj, TreeNode node)
		{
            if (obj is Pai1Section paisection)
            {
                foreach (var entry in paisection.Entries)
                    BflanObjectTonode(entry, node.Nodes);
            }
            else if (obj is Pai1Section.PaiEntry paientry)
            {
                foreach (var tag in paientry.Tags)
                    BflanObjectTonode(tag, node.Nodes);
            }
            else if (obj is Pai1Section.PaiTag paitag)
            {
                foreach (var entry in paitag.Entries)
                    BflanObjectTonode(entry, node.Nodes);
            }
        }

		private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e) =>
			propertyGrid1.SelectedObject = e.Node.Tag;

		private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog sav = new SaveFileDialog() { Filter = "Bflan file|*.bflan" };
			if (sav.ShowDialog() != DialogResult.OK) return;
			SaveTo = new DiskFileProvider(sav.FileName);
			SaveTo.Save(file.WriteFile());
		}

		private void SaveAsToArchiveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_saveTo != null)
				_saveTo.Save(file.WriteFile());
			else saveAsToolStripMenuItem.PerformClick();
		}

		private void BflanEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			SaveTo?.EditorClosed();
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

		private void InsertEntry(object target, object entry, TreeNode viewNode)
		{
            if (target is Pai1Section sect && entry is Pai1Section.PaiEntry paiEntry)
                sect.Entries.Add(paiEntry);
            else if (target is Pai1Section.PaiEntry targetPai && entry is Pai1Section.PaiTag paiTag)
                targetPai.Tags.Add(paiTag);
            else if (target is Pai1Section.PaiTag targetPaiTag && entry is Pai1Section.PaiTagEntry paiTagEntry)
                targetPaiTag.Entries.Add(paiTagEntry);
            else
			{
                MessageBox.Show("Can't insert the parent object here");
                return;
            }

			BflanObjectTonode(entry, viewNode.Nodes);
        }

		private void AddEntryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var parent = treeView1.SelectedNode.Tag;
			object entry = null;

			if (parent is Pai1Section)
				entry = new Pai1Section.PaiEntry() { Name = "NEW-" };
			else if (parent is Pai1Section.PaiEntry)
				entry = new Pai1Section.PaiTag() { TagType = "NEW-" };
			else if (parent is Pai1Section.PaiTag paiTag)
				entry = new Pai1Section.PaiTagEntry() { FLEUEntryName = paiTag.IsFLEU ? "New FLEU entry" : null };
			else
			{
				MessageBox.Show("Can't add an entry to the parent object");
				return;
			}

			if (entry != null)
				InsertEntry(parent, entry, treeView1.SelectedNode);
		}

        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Parent == null) return;
			
			var clone = ((ICloneable)treeView1.SelectedNode.Tag).Clone();

			// Can names be duplicate ?
			var parent = treeView1.SelectedNode.Parent;
			InsertEntry(parent.Tag, clone, parent);
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var selected = treeView1.SelectedNode;
			var parent = treeView1.SelectedNode?.Parent;
			if (selected.Tag is Pai1Section.PaiEntry entry)
			{
				var par = (Pai1Section)parent.Tag;
				par.Entries.Remove(entry);
			}
			else if (selected.Tag is Pai1Section.PaiTag tag)
			{
				var par = (Pai1Section.PaiEntry)parent.Tag;
				par.Tags.Remove(tag);
			}
			else if (selected.Tag is Pai1Section.PaiTagEntry tagEntry)
			{
				var par = (Pai1Section.PaiTag)parent.Tag;
				par.Entries.Remove(tagEntry);
			}
			else
			{
				MessageBox.Show("Can't remove this element");
				return;
			}

			parent.Nodes.Remove(selected);
		}

		private void ExportToJSONToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog sav = new SaveFileDialog() { Filter = "JSON file|*.json" };
			if (sav.ShowDialog() != DialogResult.OK) return;
			File.WriteAllText(sav.FileName, JsonConvert.SerializeObject(BflanSerializer.Serialize(file), Formatting.Indented));
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
			var b = new BflanEditor(null, null);
			b.ImportFromJSONToolStripMenuItem_Click(null,null);
			if (b.file == null) return null;
			return b;
		}

		private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			treeView1.SelectedNode.Text = treeView1.SelectedNode.Tag.ToString();
			treeView1.Invalidate();
		}

		private void SwitchByteOrderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			file.byteOrder = file.byteOrder == Syroot.BinaryData.ByteOrder.BigEndian ?
				Syroot.BinaryData.ByteOrder.LittleEndian : Syroot.BinaryData.ByteOrder.BigEndian;
			UpdateByteOrder();
		}

		private void BflanEditor_KeyDown(object sender, KeyEventArgs e)
		{
			e.SuppressKeyPress = true;
			if (e.Shift && e.Control && e.KeyCode == Keys.S)
				saveAsToolStripMenuItem.PerformClick();
			else if (e.Control && e.KeyCode == Keys.S)
				saveToolStripMenuItem.PerformClick();
			else if (e.Control && e.KeyCode == Keys.L)
				treeView1.ExpandAll();
			else if (e.Control && e.KeyCode == Keys.K)
				treeView1.CollapseAll();
			else e.SuppressKeyPress = false;
		}

		private void expandAllToolStripMenuItem_Click(object sender, EventArgs e) =>
			treeView1.ExpandAll();

        private void templatesToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
			templatesToolStripMenuItem.DropDownItems.Clear();

			var currentItem = treeView1.SelectedNode?.Tag;
			LoadedTemplate[] templates = null;

            if (currentItem != null) 
			{
				if (!(currentItem is IBflanGenericCollection))
					return;

				if (currentItem is Pai1Section) // Contains PaiEntry[]
                    templates = BflanTemplates.Where(x => x.Manifest.Target == Automation.BflanTemplateKind.Pai1).ToArray();
				else if (currentItem is Pai1Section.PaiEntry) // contains PaiTag[]
					templates = BflanTemplates.Where(x => x.Manifest.Target == Automation.BflanTemplateKind.PaiEntry).ToArray();
                else if (currentItem is Pai1Section.PaiTag) // contains PaiTagEntry[]
                    templates = BflanTemplates.Where(x => x.Manifest.Target == Automation.BflanTemplateKind.PaiTag).ToArray();
				else if (currentItem is Pai1Section.PaiTagEntry) // contains Keyframe[]
                    templates = BflanTemplates.Where(x => x.Manifest.Target == Automation.BflanTemplateKind.PaiTagEntry).ToArray();
            }

			if (templates != null && templates.Length != 0)
			{
				foreach (var t in templates)
				{
					var item = templatesToolStripMenuItem.DropDownItems.Add(t.Manifest.Name);
					item.Tag = t;
                    item.Click += templateDynamicItem_Click;
                }
            }
			else 
			{
				templatesToolStripMenuItem.DropDownItems.Add("No templates available for this item");
            }
        }

        private void templateDynamicItem_Click(object sender, EventArgs e)
        {
			if (!(treeView1.SelectedNode?.Tag is IBflanGenericCollection bflanItem))
				return;

            var template = ((ToolStripItem)sender).Tag as LoadedTemplate;
			if (template == null) return;

			if (!File.Exists(template.FullPathToTemplate))
			{
				MessageBox.Show($"The file {template.FullPathToTemplate} associated with this template was not found");
				return;
			}

			var content = File.ReadAllText(template.FullPathToTemplate);
			using var importer = new ValueImportForm(template.Manifest, content);
			importer.ShowDialog();

			if (importer.Result == null)
				return;

			object[] deserialized = template.Manifest.DeserializeContent(importer.Result);

			foreach (var obj in deserialized)
                bflanItem.InsertElement(obj);

			treeView1.SelectedNode.Nodes.Clear();
			BflanObjectChildrenToNode(bflanItem, treeView1.SelectedNode);
        }
    }
}
