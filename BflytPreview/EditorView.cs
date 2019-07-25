using SwitchThemes.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using BflytPreview.EditorForms;
using System.Threading.Tasks;
using SwitchThemes.Common.Bflyt;
using static SwitchThemes.Common.Bflyt.BflytFile;

namespace BflytPreview
{
	public partial class EditorView : Form, IFormSaveToArchive
	{
		BflytFile layout;
		IFileProvider _parentArch;
		public IFileProvider ParentArchive { get => _parentArch; set { _parentArch = value; saveToSZSToolStripMenuItem.Visible = _parentArch != null; } }

		double zoomFactor => zoomSlider.Value / 10f;

		float x = 640, y = -360;
		private Point firstPoint = new Point();

		bool canMoveView;

		OpenTK.GLControl glControl;

		public static int texture;

		public EditorView(BflytFile _layout)
		{
			TypeDescriptor.AddAttributes(typeof(SwitchThemes.Common.Vector3), new TypeConverterAttribute(typeof(Vector3Converter)));
			TypeDescriptor.AddAttributes(typeof(SwitchThemes.Common.Vector2), new TypeConverterAttribute(typeof(Vector2Converter)));

			KeyPreview = true;

			InitializeComponent();
			layout = _layout;

			treeView1.NodeMouseClick += (sender, args) => treeView1.SelectedNode = args.Node;

			glControl = new OpenTK.GLControl();
			glControl.Dock = DockStyle.Fill;
			panel1.Controls.Add(glControl);
			glControl.KeyDown += new KeyEventHandler(glControl_KeyDown);
			glControl.Resize += new EventHandler(glControl_Resize);
			glControl.Paint += GlControl_Paint;
			glControl.MouseDown += glControl_MouseDown;
			glControl.MouseMove += glControl_MouseMove;
			glControl.MouseUp += GlControl_MouseUp;
        }

        #region OnLoad

        private void EditorView_Load(object sender, System.EventArgs e)
		{
			bringToFront();
			glControl_Resize(glControl, EventArgs.Empty);

			UpdateView();
			Render();

			Task ignRes = setMoveView();

			/*Text =
				GL.GetString(StringName.Vendor) + " " +
				GL.GetString(StringName.Renderer) + " " +
				GL.GetString(StringName.Version);*/
		}

		private async Task setMoveView()
		{
			await Task.Delay(500);
			canMoveView = true;
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
			/*switch (e.KeyData)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }*/
		}

		#endregion

		#region GLControl.Paint event handler

		private void GlControl_Paint(object sender, PaintEventArgs e)
		{
			this.glControl.Context.MakeCurrent(this.glControl.WindowInfo);
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
			GL.ClearColor(160 / 255f, 160 / 255f, 160 / 255f, 1); //Control dark color

			if (texture != 0 && Settings.Default.ShowImage)
				DrawBgImage();

			RenderPanes();

			glControl.SwapBuffers();
		}

		#endregion

		#region Draw Background Image
		void DrawBgImage()
		{
			GL.Enable(EnableCap.Texture2D);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
			GL.PushMatrix();
			GL.Scale(1 * zoomFactor, -1 * zoomFactor, 1);
			GL.Translate(x, y, 0);

			GL.Color4(Color.White);

			GL.BindTexture(TextureTarget.Texture2D, texture);
			GL.Begin(PrimitiveType.Quads);

			GL.TexCoord2(-1, -1);
			GL.Vertex3(-640, -360, 0);

			GL.TexCoord2(0, -1);
			GL.Vertex3(640, -360, 0);

			GL.TexCoord2(0, 0);
			GL.Vertex3(640, 360, 0);

			GL.TexCoord2(-1, 0);
			GL.Vertex3(-640, 360, 0);
			GL.End();
			GL.BindTexture(TextureTarget.Texture2D, 0);
			GL.PopMatrix();
		}
		#endregion

		void RenderPanes()
		{
			float[] DrawOnTopTransform = new float[16];
			EditablePane DrawOnTop = null;

			void RecursiveRenderPane(EditablePane p)
			{
				if (!p.ParentVisibility)
					return;

				var color = Settings.Default.PaneColor;

				GL.PushMatrix();
				GL.Translate(p.Position.X, p.Position.Y, 0);
				GL.Rotate(p.Rotation.Z, p.Rotation.X, p.Rotation.Y, p.Rotation.Z);
				GL.Scale(p.Scale.X, p.Scale.Y, 1);

				if (p.ViewInEditor)
				{
					if (treeView1.SelectedNode != null && (p == treeView1.SelectedNode.Tag as EditablePane))
					{
						DrawOnTop = p;
						GL.GetFloat(GetPName.ModelviewMatrix, DrawOnTopTransform);
					}
					else
						DrawPane(p.transformedRect, color);
				}

				foreach (var c in p.Children.Where(x => x is EditablePane))
					RecursiveRenderPane((EditablePane)c);
				GL.PopMatrix();
			}

			GL.Scale(1 * zoomFactor, -1 * zoomFactor, 1);
			GL.Translate(x, y, 0);

			RecursiveRenderPane((EditablePane)layout.RootPane);
			DrawPane(new BflytFile.CusRectangle(-1280 / 2, -720 / 2, 1280, 720), Settings.Default.OutlineColor);

			if (DrawOnTop != null)
			{
				GL.LoadMatrix(DrawOnTopTransform);
                DrawPane(DrawOnTop.transformedRect, Settings.Default.SelectedColor);
                DrawPaneMiddlePoint(DrawOnTop.transformedRect, Settings.Default.SelectedColor);
            }
		}

        void DrawPaneMiddlePoint(BflytFile.CusRectangle rect, Color color)
        {
            GL.Color3(color);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(rect.x + (rect.width / 2) - 1, rect.y + (rect.height / 2) - 1);
            GL.Vertex2(rect.x + (rect.width / 2) - 1, rect.y + (rect.height / 2) + 1);
            GL.Vertex2(rect.x + (rect.width / 2) - 1, rect.y + (rect.height / 2) + 1);
            GL.Vertex2(rect.x + (rect.width / 2) + 1, rect.y + (rect.height / 2) + 1);
            GL.Vertex2(rect.x + (rect.width / 2) + 1, rect.y + (rect.height / 2) + 1);
            GL.Vertex2(rect.x + (rect.width / 2) + 1, rect.y + (rect.height / 2) - 1);
            GL.Vertex2(rect.x + (rect.width / 2) + 1, rect.y + (rect.height / 2) - 1);
            GL.Vertex2(rect.x + (rect.width / 2) - 1, rect.y + (rect.height / 2) - 1);
            GL.End();
        }

		void DrawPane(BflytFile.CusRectangle rect, Color color)
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

		public void UpdateView(object focus = null)
		{
			treeView1.Nodes.Clear();
			{
				string target = focus as string;
				var texNode = treeView1.Nodes.Add("Textures");
				texNode.Tag = new TextureTag();
				int index = 0;
				foreach (var t in layout.GetTex.Textures)
				{
					var n = texNode.Nodes.Add($"{index++} : {t}");
					n.Tag = new TextureTag(t);
					if (target != null && t == target) n.Expand();
				}
			}
			{
				var target = focus as BflytMaterial;
				var matNode = treeView1.Nodes.Add("Materials");
				int index = 0;
				foreach (var t in layout.GetMat.Materials)
				{
					var n = matNode.Nodes.Add($"{index++} : {t}");
					n.Tag = t;
					if (target != null & t == target) n.Expand();
				}
			}
			RecursiveAddNode(layout.RootPane, treeView1.Nodes, focus as BasePane);
			RecursiveAddNode(layout.RootGroup, treeView1.Nodes, focus as BasePane);
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
                void RecursiveRenderPane(BflytFile.EditablePane p)
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
                        if (treeView1.SelectedNode != null && p == treeView1.SelectedNode.Tag as BflytFile.EditablePane)
                            pen = HighlightedPen;
                        gfx.DrawRectangle(pen, transformedRect);
                    }

                    foreach (var c in p.Children.Where(x => x is BflytFile.EditablePane))
                        RecursiveRenderPane((BflytFile.EditablePane)c);
                    gfx.Transform = CurMatrix.Pop();
                }

                gfx.ScaleTransform(1, -1);
                gfx.TranslateTransform(640, -360);
                RecursiveRenderPane((BflytFile.EditablePane)layout.RootPane);

            }
            pictureBox1.Image = b;
        }*/

		public static void RecursiveAddNode(BflytFile.BasePane p, TreeNodeCollection node, BasePane focus)
		{
			var TargetNode = node.Add(p.ToString());
			TargetNode.Tag = p;
			if (focus == p) TargetNode.Expand();
			foreach (var c in p.Children)
				RecursiveAddNode(c, TargetNode.Nodes, focus);
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem.Label == "PaneName")
				UpdateView();
			glControl.Invalidate();
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			propertyGrid1.SelectedObject = treeView1.SelectedNode.Tag;
			glControl.Invalidate();
		}

		private void treeView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.H)
			{
				var target = treeView1.SelectedNode.Tag as EditablePane;
				if (target == null) return;
				target.ViewInEditor = !target.ViewInEditor;
				glControl.Invalidate();
			}
			else if (e.KeyCode == Keys.Q)
			{
				var target = treeView1.SelectedNode.Tag as IInspectable;
				if (target == null) return;
				HexEditorForm.Show(target.GetData());
			}
		}

        void SaveBflyt()
		{
			SaveFileDialog sav = new SaveFileDialog() { Filter = "Binary cafe layout (*.bflyt)|*.bflyt" };
			if (sav.ShowDialog() != DialogResult.OK) return;

			File.WriteAllBytes(sav.FileName, layout.SaveFile());
		}

		private void saveBflytFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveBflyt();
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

		private void EditorView_LocationChanged(object sender, System.EventArgs e)
		{
			bringToFront();
		}

		private void zoomSlider_Scroll(object sender, EventArgs e)
		{
			glControl.Invalidate();
		}

		private void SetupCursorXYZ(Point res)
		{
			x -= res.X;
			y += res.Y;
		}

		private void SetupObjectXYZ(EditablePane p, Point res)
		{
			p.Position = new SwitchThemes.Common.Vector3(p.Position.X - res.X, p.Position.Y + res.Y, 0);
		}

		private void glControl_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			firstPoint = Control.MousePosition;
		}

		private void helpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show(
                "Quick guide:\n\n" +
				"- Moving the view: to move the view just click anywhere in the canvas and drag the canvas\n\n" +
                "- Zoom: To increase or reduce the zoom level use the trackbar on the bottom left\n\n" +
                "- Dragging objects: First select an object in the tree view, it will be highlighted in the preview, then keeping CTRL pressed drag it with the cursor in the canvas\n\n" +
				"- The green box: The green box represents the screen bounds, it's always at (0,0) and has the screen size.");
		}

		private void saveToSZSToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_parentArch.SaveToArchive(layout.SaveFile(), this);
		}

		private void EditorView_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (ParentArchive != null)
				ParentArchive.EditorClosed(this);
			Settings.Default.ShowImage = false;
		}

		bool DraggedObject = false;
		private void glControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left || !canMoveView)
				return;
			EditablePane target = null;
			if (treeView1.SelectedNode != null)
				target = treeView1.SelectedNode.Tag as EditablePane;

			if (ModifierKeys.HasFlag(Keys.Control) && target != null)
			{
				Point temp = Control.MousePosition;
				Point res = new Point(firstPoint.X - temp.X, firstPoint.Y - temp.Y);

				SetupObjectXYZ(target, res);

				firstPoint = temp;
				DraggedObject = true;
				glControl.Invalidate();
			}
			else if (!ModifierKeys.HasFlag(Keys.Control))
			{
				Point temp = Control.MousePosition;
				Point res = new Point(firstPoint.X - temp.X, firstPoint.Y - temp.Y);

				SetupCursorXYZ(res);

				firstPoint = temp;
				glControl.Invalidate();
			}
		}

		public static int LoadBgImage(string path, bool flip_x = false, bool flip_y = false)
		{
			if (!File.Exists(path))
				throw new FileNotFoundException("File not found at '" + path + "'");

			Bitmap bitmap = new Bitmap(path);

			if (flip_y)
				bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
			if (flip_x)
				bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);

			int tex;
			GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

			GL.GenTextures(1, out tex);
			GL.BindTexture(TextureTarget.Texture2D, tex);

			BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
				ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
			bitmap.UnlockBits(data);


			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

			return tex;
		}

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SettingsWindow set = new SettingsWindow();
			set.ShowDialog(this);
			set.Dispose();
		}

		private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			treeView1.ExpandAll();
		}

		private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			treeView1.CollapseAll();
		}

		private void GlControl_MouseUp(object sender, MouseEventArgs e)
		{
			if (DraggedObject)
			{
				DraggedObject = false;
				propertyGrid1.Refresh();
			}
		}

		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode != null)
			{
				if (((BasePane)treeView1.SelectedNode.Tag).Parent == null)
				{
					MessageBox.Show("You can't remove a root pane");
					return;
				}
				layout.RemovePane((BasePane)treeView1.SelectedNode.Tag);
				UpdateView();
			}
		}

		private void nullPaneToolStripMenuItem_Click(object sender, EventArgs e) =>
			AddPane(new EditablePane("pan1", layout.FileByteOrder));

		private void pic1PaneToolStripMenuItem_Click(object sender, EventArgs e) =>
			AddPane(new Pic1Pane(layout.FileByteOrder));

		private void txtPaneToolStripMenuItem_Click(object sender, EventArgs e) =>
			AddPane(new Txt1Pane(layout.FileByteOrder));

		private void clonePaneToolStripMenuItem_Click(object sender, EventArgs e)
		{
			EditablePane pane = treeView1.SelectedNode.Tag as EditablePane;
			layout.AddPane(-1, pane.Parent, pane.Clone());
			UpdateView(pane);
		}

		void AddPane(BasePane p)
		{
			if (treeView1.SelectedNode.Tag as EditablePane == null) return;
			layout.AddPane(-1, treeView1.SelectedNode.Tag as EditablePane, p);
			UpdateView(p);
		}

		private void AddGroupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Grp1Pane pane = new Grp1Pane(layout.Version);
			pane.GroupName = "New group";
			layout.AddPane(-1, treeView1.SelectedNode.Tag as Grp1Pane, pane);
			UpdateView(pane);
		}

		private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Node.Tag is Grp1Pane)
				treeView1.ContextMenuStrip = GroupMenuStrip;
			else if (e.Node.Tag is EditablePane)
				treeView1.ContextMenuStrip = PaneMenuStrip;
			else if (e.Node.Tag is TextureTag)
				treeView1.ContextMenuStrip = TextureMenuStrip;
			else if (e.Node.Tag is BflytMaterial)
				treeView1.ContextMenuStrip = MaterialMenuStrip;
			else
				treeView1.ContextMenuStrip = null;
		}

		private void NewTextureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string name = "New_texture";
			if (InputDialog.Show("Add new texture", "Enter a name for the new texture.", ref name) != DialogResult.OK) return;
			layout.GetTex.Textures.Add(name);
			UpdateView(name);
		}

		private void RemoveTexture_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Parent == null) return; //the texture must be in the root textures node
			layout.GetTex.Textures.Remove(((TextureTag)treeView1.SelectedNode.Tag).TexName);
			UpdateView();
		}

		private void RemoveMaterial_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Parent == null) return;
			layout.GetMat.Materials.Remove((BflytMaterial)treeView1.SelectedNode.Tag);
			UpdateView();
		}

		private void CloneMaterialToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Parent == null) return;
			var selected = (BflytMaterial)treeView1.SelectedNode.Tag;
			var next = new BflytMaterial(selected.Write(layout.Version, layout.FileByteOrder), layout.FileByteOrder, layout.Version);
			if (next.Name.Length < 27)
				next.Name += "_";
			else
				next.Name = next.Name.Substring(0,26) + "_";
			layout.GetMat.Materials.Add(next);
			UpdateView(next);
		}

		private void MoveUpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var p = treeView1.SelectedNode.Tag as BasePane;
			if (p == null || p.Parent == null) return;
			layout.MovePane(p, p.Parent, p.Parent.Children.IndexOf(p) - 1);
			UpdateView(p);
		}

		private void MoveDownToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var p = treeView1.SelectedNode.Tag as BasePane;
			if (p == null || p.Parent == null) return;
			layout.MovePane(p, p.Parent, p.Parent.Children.IndexOf(p) + 1);
			UpdateView(p);
		}

		//TODO
		//private void TreeView1_ItemDrag(object sender, ItemDragEventArgs e)
		//{
		//	if (((TreeNode)e.Item).Tag is EditablePane && ((TreeNode)e.Item).Tag != layout.RootPane)
		//	{
		//		DoDragDrop(e.Item, DragDropEffects.Move);
		//	}
		//}

		//private void TreeView1_DragEnter(object sender, DragEventArgs e)
		//{
		//	e.Effect = DragDropEffects.Move;
		//}

		//private void TreeView1_DragDrop(object sender, DragEventArgs e)
		//{
		//	if (!e.Data.GetDataPresent(typeof(TreeNode))) return;

		//	TreeNode sourceNode = e.Data.GetData(typeof(TreeView)) as TreeNode;

		//	var item = new TreeNode(sourceNode.Text);


		//	System.Drawing.Point pt = ((TreeView)sender).PointToClient(new System.Drawing.Point(e.X, e.Y));
		//	TreeNode DestinationNode = ((TreeView)sender).GetNodeAt(pt);

		//	DestinationNode.Nodes.Add(item);
		//	DestinationNode.Expand();

		//}

		private void EditorView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Shift && e.KeyCode == Keys.S) SaveBflyt();
            else if (e.Control && e.KeyCode == Keys.S && _parentArch != null) _parentArch.SaveToArchive(layout.SaveFile(), this);
            else if (e.Control && e.KeyCode == Keys.L) treeView1.ExpandAll();
            else if (e.Control && e.KeyCode == Keys.K) treeView1.CollapseAll();
        }
    }

	//used for tagging and root node
	internal class TextureTag
	{
		internal string TexName;
		public TextureTag(string n = null) { TexName = n; }
		public override bool Equals(object obj)
		{
			if (obj is string)
				return TexName == (string)obj;
			else if (obj is TextureTag)
				return ((TextureTag)obj).TexName == TexName;
			return base.Equals(obj);
		}

		public static bool operator ==(TextureTag a, TextureTag b) => a.Equals(b);
		public static bool operator !=(TextureTag a, TextureTag b) => !a.Equals(b);
	}
}
