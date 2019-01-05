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
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;

namespace BflytPreview
{
    public partial class EditorView : Form
    {
		BFLYT layout;

		double zoomFactor => zoomSlider.Value / 10f;

        OpenTK.GLControl glControl;

        public EditorView(BFLYT _layout)
        {
            TypeDescriptor.AddAttributes(typeof(SwitchThemes.Common.Vector3), new TypeConverterAttribute(typeof(Vector3Converter)));
            TypeDescriptor.AddAttributes(typeof(SwitchThemes.Common.Vector2), new TypeConverterAttribute(typeof(Vector2Converter)));

            InitializeComponent();
			layout = _layout;

			glControl = new OpenTK.GLControl();
            glControl.Dock = DockStyle.Fill;
			panel1.Controls.Add(glControl);
			glControl.KeyDown += new KeyEventHandler(glControl_KeyDown);
			glControl.KeyUp += new KeyEventHandler(glControl_KeyUp);
			glControl.Resize += new EventHandler(glControl_Resize);
			glControl.Paint += GlControl_Paint;
		}

		#region OnLoad

		private void EditorView_Load(object sender, System.EventArgs e)
		{
			BringToFront();
			glControl_Resize(glControl, EventArgs.Empty);

			UpdateView();
			Render();

			Text =
				GL.GetString(StringName.Vendor) + " " +
				GL.GetString(StringName.Renderer) + " " +
				GL.GetString(StringName.Version);
		}

		void glControl_KeyUp(object sender, KeyEventArgs e)
        {
            /*if (e.KeyCode == Keys.F12)
            {
                
            }*/
        }

        #endregion

        #region GLControl.Resize event handler

        void glControl_Resize(object sender, EventArgs e)
        {
            OpenTK.GLControl c = sender as OpenTK.GLControl;

            if (c.ClientSize.Height == 0)
                c.ClientSize = new System.Drawing.Size(c.ClientSize.Width, 1);

            GL.Viewport(0, 0, c.ClientSize.Width, c.ClientSize.Height);
			glControl.Invalidate();
			/*float aspect_ratio = panel1.Width / (float)panel1.Height;
            Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);*/
		}

        #endregion

        #region GLControl.KeyDown event handler

        void glControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

		#endregion

		#region GLControl.Paint event handler

		private void GlControl_Paint(object sender, PaintEventArgs e)
		{
			Render();
		}

		#endregion

		#region private void Render()

		private void Render()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, glControl.Width, glControl.Height, 0, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

			GL.Clear(ClearBufferMask.ColorBufferBit);
			GL.ClearColor(160/255f, 160 / 255f, 160 / 255f, 1); //Control dark color

			RenderPanes();
            
            glControl.SwapBuffers();
        }

        #endregion

        void RenderPanes()
        {
			float[] DrawOnTopTransform = new float[16];
			BFLYT.EditablePane DrawOnTop = null;

            void RecursiveRenderPane(BFLYT.EditablePane p)
            {
                if (!p.ParentVisibility)
                    return;
				
                var color = Color.Black;

				GL.PushMatrix();
                GL.Translate(p.Position.X, p.Position.Y, 0);
                GL.Rotate(p.Rotation.Z, p.Rotation.X, p.Rotation.Y, p.Rotation.Z);
                GL.Scale(p.Scale.X, p.Scale.Y, 1);

                if (p.ViewInEditor)
                {
					if (treeView1.SelectedNode != null && (p == treeView1.SelectedNode.Tag as BFLYT.EditablePane))
					{
						DrawOnTop = p;
						GL.GetFloat(GetPName.ModelviewMatrix, DrawOnTopTransform);
					}
					else
						DrawPane(p.transformedRect, color);
				}

                foreach (var c in p.Children.Where(x => x is BFLYT.EditablePane))
                    RecursiveRenderPane((BFLYT.EditablePane)c);
				GL.PopMatrix();
            }

            GL.Scale(1 * zoomFactor, -1 * zoomFactor, 1);
            GL.Translate(640, -360, 0);
            RecursiveRenderPane((BFLYT.EditablePane)layout.RootPane);

			if (DrawOnTop != null)
			{
				GL.LoadMatrix(DrawOnTopTransform);
				DrawPane(DrawOnTop.transformedRect, Color.Red);
			}
		}

        void DrawPane(BFLYT.CusRectangle rect, Color color)
        {
            GL.Color3(color);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(rect.x, rect.y);
            GL.Vertex2(rect.x, rect.y + rect.height);
            GL.Vertex2(rect.x, rect.y + rect.height);
            GL.Vertex2(rect.x + rect.width, rect.y + rect.height);
            GL.Vertex2(rect.x + rect.width, rect.y + rect.height);
            GL.Vertex2(rect.x + rect.width, rect.y);
            GL.Vertex2(rect.x + rect.width, rect.y);
            GL.Vertex2(rect.x, rect.y);
            GL.End();
        }

        public void UpdateView()
        {
            treeView1.Nodes.Clear();
            RecursiveAddNode(layout.RootPane, treeView1.Nodes);
			glControl.Invalidate();
        }
		
        /*void RenderImg()
        {
            using (Graphics gfx = Graphics.FromImage(b)) Do not remove this as it can be used to render the layout as an image
            {
                gfx.Clear(Color.LightGray);

                gfx.DrawRectangle(new Pen(Brushes.Red, 2), new Rectangle(0, 0, 1280, 720));

                Stack<Matrix> CurMatrix = new Stack<Matrix>();
                Random r = new Random();
                void RecursiveRenderPane(BFLYT.EditablePane p)
                {
                    if (!p.ParentVisibility)
                        return;
                    CurMatrix.Push(gfx.Transform.Clone());
                    gfx.TranslateTransform(p.Position.X, p.Position.Y);
                    gfx.RotateTransform(p.Rotation.Z);
                    gfx.ScaleTransform(p.Scale.X, p.Scale.Y);

                    Rectangle transformedRect = new Rectangle(p.transformedRect.x, p.transformedRect.y, p.transformedRect.width, p.transformedRect.height);

                    var pen = new Pen(Brushes.Black, 2);
                    var HighlightedPen = new Pen(Brushes.Red, 4);

                    if (p.ViewInEditor)
                    {
                        if (treeView1.SelectedNode != null && p == treeView1.SelectedNode.Tag as BFLYT.EditablePane)
                            pen = HighlightedPen;
                        gfx.DrawRectangle(pen, transformedRect);
                    }

                    foreach (var c in p.Children.Where(x => x is BFLYT.EditablePane))
                        RecursiveRenderPane((BFLYT.EditablePane)c);
                    gfx.Transform = CurMatrix.Pop();
                }

                gfx.ScaleTransform(1, -1);
                gfx.TranslateTransform(640, -360);
                RecursiveRenderPane((BFLYT.EditablePane)layout.RootPane);

            }
            pictureBox1.Image = b;
        }*/

        void RecursiveAddNode(BFLYT.BasePane p, TreeNodeCollection node)
        {
            var TargetNode = node.Add(p.ToString().Split(' ').Last());
            TargetNode.Tag = p;
            foreach (var c in p.Children)
                RecursiveAddNode(c, TargetNode.Nodes);
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
			glControl.Invalidate();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyGrid1.SelectedObject = treeView1.SelectedNode.Tag as BFLYT.EditablePane;
			glControl.Invalidate();
		}

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.H)
            {
                var target = treeView1.SelectedNode.Tag as BFLYT.EditablePane;
                if (target == null) return;
                target.ViewInEditor = !target.ViewInEditor;
				glControl.Invalidate();
			}
        }

        private void saveBFLYTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sav = new SaveFileDialog() { Filter = "Binary cafe layout (*.bflyt)|*.bflyt" };
            if (sav.ShowDialog() != DialogResult.OK) return;

            foreach (var p in layout.Panes.Where(x => x is BFLYT.EditablePane))
                ((BFLYT.EditablePane)p).ApplyChanges(layout.FileByteOrder);

            File.WriteAllBytes(sav.FileName, layout.SaveFile());
        }

        private void bringToFront()
        {
            this.Activate();
            this.BringToFront();
            this.Focus();
        }

        private void EditorView_Click(object sender, System.EventArgs e)
        {
            bringToFront();
        }

        private void EditorView_Resize(object sender, System.EventArgs e)
        {
            bringToFront();
        }

		private void zoomSlider_Scroll(object sender, EventArgs e)
		{
			glControl.Invalidate();
		}
	}
}
