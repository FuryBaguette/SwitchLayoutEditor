using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BflytPreview.EditorForms
{
	public class HexEditorForm
	{
		public static void Show(byte[] data)
		{
			Form f = new Form();
			f.Text = "Hex editor";
			ByteViewer bv = new ByteViewer();
			bv.Dock = DockStyle.Fill;
			f.Controls.Add(bv);
			bv.SetBytes(data);
			f.TopMost = true;
			f.Show();
		}
	}
}
