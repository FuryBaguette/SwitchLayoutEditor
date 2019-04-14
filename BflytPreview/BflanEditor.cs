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

namespace BflytPreview
{
	public partial class BflanEditor : Form
	{
		Bflan file = null;
		public BflanEditor(Bflan _file)
		{
			InitializeComponent();
			file = _file;
		}

		private void BflanEditor_Load(object sender, EventArgs e)
		{
			UpdateTreeview();
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
	}
}
