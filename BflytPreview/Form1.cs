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
            EditorView editorView = new EditorView();
            this.IsMdiContainer = true;
            editorView.TopLevel = false;
            editorView.Parent = pnlSubSystem;
            editorView.layout = new BFLYT(File.ReadAllBytes(opn.FileName));
            editorView.Text = opn.FileName;
            editorView.Show();
            //editorView.UpdateView();
		}

        private void Form1_Load(object sender, System.EventArgs e)
        {
#if DEBUG
			string AutoLaunch = @"RdtBase.bflyt";
			if (!File.Exists(AutoLaunch)) return;
			EditorView editorView = new EditorView();
			this.IsMdiContainer = true;
			editorView.TopLevel = false;
			editorView.Parent = pnlSubSystem;
			editorView.layout = new BFLYT(File.ReadAllBytes(AutoLaunch));
			editorView.Text = AutoLaunch;
			editorView.Show();
#endif
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
