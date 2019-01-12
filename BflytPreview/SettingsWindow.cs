using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BflytPreview
{
    public partial class SettingsWindow : Form
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SettingsWindow_Load(object sender, EventArgs e)
        {
            pickColor.BackColor = Settings.Default.PaneColor;
            selectedColor.BackColor = Settings.Default.SelectedColor;
            outlineColor.BackColor = Settings.Default.OutlineColor;
            Console.WriteLine(Settings.Default.BgFileName);
            if (!string.IsNullOrEmpty(Settings.Default.BgFileName))
            {
                pictureBox1.BackgroundImage = Image.FromFile(Settings.Default.BgFileName);
                EditorView.texture = EditorView.LoadBgImage(Settings.Default.BgFileName, false, true);
            }
            if (Settings.Default.ShowImage)
                showImg.Text = "Hide Image";
        }

        private void SettingsWindow_FormClosing(object sender, FormClosingEventArgs e)
        {

            Settings.Default.PaneColor = pickColor.BackColor;
            Settings.Default.SelectedColor = selectedColor.BackColor;
            Settings.Default.OutlineColor = outlineColor.BackColor;
            Settings.Default.OutlineColor = outlineColor.BackColor;
            Settings.Default.Save();
        }

        private void pickColor_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.ShowHelp = true;
            MyDialog.Color = Settings.Default.PaneColor;
            
            if (MyDialog.ShowDialog() == DialogResult.OK)
                Settings.Default.PaneColor = pickColor.BackColor = MyDialog.Color;
        }

        private void selectedColor_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.ShowHelp = true;
            MyDialog.Color = Settings.Default.SelectedColor;

            if (MyDialog.ShowDialog() == DialogResult.OK)
                Settings.Default.SelectedColor = selectedColor.BackColor = MyDialog.Color;
        }

        private void outlineColor_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.ShowHelp = true;
            MyDialog.Color = Settings.Default.OutlineColor;

            if (MyDialog.ShowDialog() == DialogResult.OK)
                Settings.Default.OutlineColor = outlineColor.BackColor = MyDialog.Color;
        }

        private void loadBackgroundImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog opn = new OpenFileDialog() { Filter = "Supported files (jpg,jpeg,png)|*.jpg;*.jpeg;*.png|All files|*.*" };
            if (opn.ShowDialog() != DialogResult.OK) return;
            
            pictureBox1.BackgroundImage = Image.FromFile(opn.FileName);
            Settings.Default.BgFileName = opn.FileName;
            EditorView.texture = EditorView.LoadBgImage(opn.FileName, false, true);
        }

        private void showImg_Click(object sender, EventArgs e)
        {
            if (!Settings.Default.ShowImage)
            {
                Settings.Default.ShowImage = true;
                showImg.Text = "Hide Image";
            }
            else
            {
                Settings.Default.ShowImage = false;
                showImg.Text = "Show Image";
            }
        }
    }
}
