using SwitchThemes.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using SwitchThemes.Common.Custom;
using Syroot.BinaryData;

namespace BflytPreview
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			TypeDescriptor.AddAttributes(typeof(Vector3), new TypeConverterAttribute(typeof(Vector3Converter)));
			TypeDescriptor.AddAttributes(typeof(Vector2), new TypeConverterAttribute(typeof(Vector2Converter)));

			InitializeComponent();
		}

		private void openBFLYTToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog opn = new OpenFileDialog() { Filter = "Supported files (bflyt,szs)|*.bflyt;*.szs|All files|*.*" };
			if (opn.ShowDialog() != DialogResult.OK) return;
			OpenFile(File.ReadAllBytes(opn.FileName), opn.FileName);
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
#if DEBUG
			string AutoLaunch = @"RdtBase.bflyt";
			if (!File.Exists(AutoLaunch)) return;
			OpenFile(File.ReadAllBytes(AutoLaunch), AutoLaunch);
#endif
		}

		public void OpenForm(Form f)
		{
			this.IsMdiContainer = true;
			f.TopLevel = false;
			f.Parent = pnlSubSystem;
			f.Show();
		}

		public Form OpenFile(byte[] File, string name)
		{
			BinaryDataReader bin = new BinaryDataReader(new MemoryStream(File));
			string Magic = bin.ReadString(4);
			if (Magic == "Yaz0")
			{
				return OpenFile(ManagedYaz0.Decompress(File), name);
			}
			else if (Magic == "SARC")
			{
				var f = new EditorForms.SzsEditor(SARCExt.SARC.UnpackRamN(File), this);
				f.Text = name;
				OpenForm(f);
				return f;
			}
			else if (Magic == "FLYT")
			{
				EditorView editorView = new EditorView(new BFLYT(File));
				editorView.Text = name;
				OpenForm(editorView);
				return editorView;
			}
			return null;
		}

		private void pnlSubSystem_DragEnter(object sender, DragEventArgs e) 
		{if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;}	

		private void pnlSubSystem_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (string file in files)
			{
				OpenFile(File.ReadAllBytes(file), file);
			}
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
