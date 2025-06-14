using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BflytPreview.EditorForms
{
    public partial class TextView : Form
    {
        public TextView(string title, string content)
        {
            InitializeComponent();
            this.Text = title;
            textBox1.Text = content;
        }
    }
}
