using SwitchThemes.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SwitchThemes.Common.Custom;

namespace BflytPreview
{
	public partial class Form1 : Form
	{
		BFLYT layout;

		public Form1()
		{
			TypeDescriptor.AddAttributes(typeof(Vector3),new TypeConverterAttribute(typeof(Vector3Converter)));
			TypeDescriptor.AddAttributes(typeof(Vector2), new TypeConverterAttribute(typeof(Vector2Converter)));

			InitializeComponent();
		}

		private void openBFLYTToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog opn = new OpenFileDialog() { Filter = "Binary layout file (*.bflyt)|*.bflyt"};
			if (opn.ShowDialog() != DialogResult.OK) return;
			layout = new BFLYT(File.ReadAllBytes(opn.FileName));
			UpdateView();
		}

		void UpdateView()
		{
			treeView1.Nodes.Clear();
			RecursiveAddNode(layout.RootPane, treeView1.Nodes);
			RenderImg();
		}

		Bitmap b = new Bitmap(2000, 1000);

		void RenderImg()
		{
			Console.WriteLine("StartRender");
			//first rendering test, misplaced objects
			//using (Graphics gfx = Graphics.FromImage(b))
			//{
			//	gfx.Clear(Color.LightGray);

			//	var pen = new Pen(Brushes.Black, 2);

			//	gfx.DrawRectangle(new Pen(Brushes.Red, 2), new Rectangle(0, 0, 1280, 720));

			//	foreach (var p in layout.Panes.Where(x => x is BFLYT.EditablePane))
			//	{
			//		var pane = (BFLYT.EditablePane)p;

			//		if (!pane.ViewInEditor || !pane.ParentVisibility)
			//			continue;
			//		var box = (pane).BoundingBox;
			//		box.Y = 720 - box.Y;
			//		gfx.DrawRectangle(pen, box);
			//		Console.WriteLine(p.ToString() + "  " + box.ToString());
			//	}
			//}

			using (Graphics gfx = Graphics.FromImage(b))
			{
				gfx.Clear(Color.LightGray);

				var pen = new Pen(Brushes.Black, 2);

				gfx.DrawRectangle(new Pen(Brushes.Red, 2), new Rectangle(0, 0, 1280, 720));

				Stack<Matrix> CurMatrix = new Stack<Matrix>();
				void RecursiveRenderPane(BFLYT.EditablePane p)
				{
					if (!p.ParentVisibility)
						return;

					CurMatrix.Push(gfx.Transform.Clone());

					gfx.TranslateTransform(p.Position.X, p.Position.Y);
					gfx.RotateTransform(p.Rotation.Z);
					gfx.ScaleTransform(p.Scale.X, p.Scale.Y);

					if (p.ViewInEditor)
						gfx.DrawRectangle(pen, p.transformedRect);

					foreach (var c in p.Children.Where(x => x is BFLYT.EditablePane))
						RecursiveRenderPane((BFLYT.EditablePane)c);
					gfx.Transform = CurMatrix.Pop();
				}

				gfx.ScaleTransform(1, -1);
				gfx.TranslateTransform(640, -360);
				RecursiveRenderPane((BFLYT.EditablePane)layout.RootPane);

			}
			pictureBox1.Image = b;
		}

		void RecursiveAddNode(BFLYT.BasePane p, TreeNodeCollection node)
		{
			var TargetNode = node.Add(p.ToString());
			TargetNode.Tag = p;
			foreach (var c in p.Children)
				RecursiveAddNode(c, TargetNode.Nodes);
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			RenderImg();
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			propertyGrid1.SelectedObject = treeView1.SelectedNode.Tag as BFLYT.EditablePane;
		}

		private void treeView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.H)
			{
				var target = treeView1.SelectedNode.Tag as BFLYT.EditablePane;
				if (target == null) return;
				target.ViewInEditor = !target.ViewInEditor;
				RenderImg();
			}
		}

		private void saveBFLYTToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog sav = new SaveFileDialog() { Filter = "Binary cafe layout (*.bflyt)|*.bflyt" };
			if (sav.ShowDialog() != DialogResult.OK) return;

			foreach (var p in layout.Panes.Where(x => x is BFLYT.EditablePane))
				((BFLYT.EditablePane)p).ApplyChanges();

			File.WriteAllBytes(sav.FileName ,layout.SaveFile());
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
