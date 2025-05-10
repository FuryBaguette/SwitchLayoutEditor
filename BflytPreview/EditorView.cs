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
	public partial class EditorView : Form
	{
		BflytFile layout;
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

		double zoomFactor => zoomSlider.Value / 10f;

		float x = 640, y = -360;
		private Point firstPoint = new Point();

		bool canMoveView;

		OpenTK.GLControl glControl;

		public static int texture;

		// This represents the whole file while the other treeview roots represent the logical hierarchy
		TreeNode AllPanesRoot;
		TreeNode Pan1Root;
		TreeNode Grp1Root;
		TreeNode TexturesRoot;
		TreeNode MaterialsRoot;

		public EditorView(BflytFile _layout, IFileWriter saveTo)
		{
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

			SaveTo = saveTo;
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
			Pan1Pane DrawOnTop = null;

			void RecursiveRenderPane(Pan1Pane p)
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
					if (treeView1.SelectedNode != null && (p == treeView1.SelectedNode.Tag as Pan1Pane))
					{
						DrawOnTop = p;
						GL.GetFloat(GetPName.ModelviewMatrix, DrawOnTopTransform);
					}
					else
						DrawPane(p.transformedRect, color);
				}

				foreach (var c in p.Children.Where(x => x is Pan1Pane))
					RecursiveRenderPane((Pan1Pane)c);
				GL.PopMatrix();
			}

			GL.Scale(1 * zoomFactor, -1 * zoomFactor, 1);
			GL.Translate(x, y, 0);

			RecursiveRenderPane(layout.ElementsRoot);
			DrawPane(new CusRectangle(-1280 / 2, -720 / 2, 1280, 720), Settings.Default.OutlineColor);

			if (DrawOnTop != null)
			{
				GL.LoadMatrix(DrawOnTopTransform);
                DrawPane(DrawOnTop.transformedRect, Settings.Default.SelectedColor);
                DrawPaneMiddlePoint(DrawOnTop.transformedRect, Settings.Default.SelectedColor);
            }
		}

        void DrawPaneMiddlePoint(CusRectangle rect, Color color)
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

		void DrawPane(CusRectangle rect, Color color)
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

		object TryGetSelectedFocusTag() 
		{
			if (treeView1.SelectedNode == null) return null;
			if (treeView1.SelectedNode.Tag is BasePane) return treeView1.SelectedNode.Tag;
			if (treeView1.SelectedNode.Tag is TextureTag) return ((TextureTag)treeView1.SelectedNode.Tag).TexName;
			if (treeView1.SelectedNode.Tag is BflytMaterial) return treeView1.SelectedNode.Tag;
			return null;
		}

		TreeNode FindRoot(TreeNode item)
		{	
			if (item == null) return null;
			while (item.Parent != null)
				item = item.Parent;
			return item;
		}

		public void UpdateView(object focus = null)
		{
			TreeNode focusElement = null;
			
			if (focus == null)
			{
				// if there is no explicit focus change, try to keep the current one
				focus = TryGetSelectedFocusTag();
			}

			treeView1.SuspendLayout();
			treeView1.Nodes.Clear();

			{
				string target = focus as string;
				TexturesRoot = treeView1.Nodes.Add("Textures");
				TexturesRoot.Tag = new TextureTag();
				int index = 0;
				if (layout.Tex1 != null)
					foreach (var t in layout.Tex1.Textures)
					{
						var n = TexturesRoot.Nodes.Add($"{index++} : {t}");
						n.Tag = new TextureTag(t);
						if (target != null && t == target) 
							focusElement = n;
					}
			}

			{
				var target = focus as BflytMaterial;
				MaterialsRoot = treeView1.Nodes.Add("Materials");
				int index = 0;
				if (layout.Mat1 != null)
					foreach (var t in layout.Mat1.Materials)
					{
						var n = MaterialsRoot.Nodes.Add($"{index++} : {t}");
						n.Tag = t;
						if (target != null & t == target)
							focusElement = n;
					}
			}

			Pan1Root = null;
			RecursiveAddNode(layout.ElementsRoot, treeView1.Nodes, focus as BasePane, ref focusElement, ref Pan1Root);

			Grp1Root = null;
			RecursiveAddNode(layout.RootGroup, treeView1.Nodes, focus as BasePane, ref focusElement, ref Grp1Root);

			AllPanesRoot = treeView1.Nodes.Add("Full hierarchy");
			foreach (var r in layout.RootPanes)
			{
				// We don't care about ref AllPanesRoot here, since it is not null it won't be changed
				RecursiveAddNode(r, AllPanesRoot.Nodes, focus as BasePane, ref focusElement, ref AllPanesRoot);
			}

			treeView1.ResumeLayout();
			glControl.Invalidate();

			if (focusElement != null)
			{
				treeView1.SelectedNode = focusElement;
				focusElement.Expand();
			}
		}

		/*void RenderImg()
        {
            using (Graphics gfx = Graphics.FromImage(b)) Do not remove this as it can be used to render the layout as an image
            {
                gfx.Clear(Color.LightGray);

                gfx.DrawRectangle(new Pen(Brushes.Red, 2), new Rectangle(0, 0, 1280, 720));

                Stack<Matrix> CurMatrix = new Stack<Matrix>();
                Random r = new Random();
                void RecursiveRenderPane(BflytFile.Pan1Pane p)
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
                        if (treeView1.SelectedNode != null && p == treeView1.SelectedNode.Tag as BflytFile.Pan1Pane)
                            pen = HighlightedPen;
                        gfx.DrawRectangle(pen, transformedRect);
                    }

                    foreach (var c in p.Children.Where(x => x is BflytFile.Pan1Pane))
                        RecursiveRenderPane((BflytFile.Pan1Pane)c);
                    gfx.Transform = CurMatrix.Pop();
                }

                gfx.ScaleTransform(1, -1);
                gfx.TranslateTransform(640, -360);
                RecursiveRenderPane(ElementsRoot);

            }
            pictureBox1.Image = b;
        }*/

		public static void RecursiveAddNode(BflytFile.BasePane p, TreeNodeCollection node, BasePane focus, ref TreeNode focusElement, ref TreeNode outRoot)
		{
			var TargetNode = node.Add(p.ToString());
			if (outRoot == null)
				outRoot = TargetNode;

			TargetNode.Tag = p;

			if (focus == p && focusElement == null)
				focusElement = TargetNode;

			foreach (var c in p.Children)
				RecursiveAddNode(c, TargetNode.Nodes, focus, ref focusElement, ref outRoot);
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
				var target = treeView1.SelectedNode.Tag as Pan1Pane;
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

        void SaveAs()
		{
			SaveFileDialog sav = new SaveFileDialog() { Filter = "Binary cafe layout (*.bflyt)|*.bflyt" };
			if (sav.ShowDialog() != DialogResult.OK) return;
			SaveTo = new DiskFileProvider(sav.FileName);
			SaveTo.Save(layout.SaveFile());
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) =>
			SaveAs();

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

		private void SetupObjectXYZ(Pan1Pane p, Point res)
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
			if (_saveTo != null)
				_saveTo.Save(layout.SaveFile());
			else SaveAs();
		}

		private void EditorView_FormClosed(object sender, FormClosedEventArgs e)
		{
			SaveTo?.EditorClosed();
			Settings.Default.ShowImage = false;
		}

		bool DraggedObject = false;
		private void glControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left || !canMoveView)
				return;
			Pan1Pane target = null;
			if (treeView1.SelectedNode != null)
				target = treeView1.SelectedNode.Tag as Pan1Pane;

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
				var parent = ((BasePane)treeView1.SelectedNode.Tag).Parent;
				if (parent == null)
				{
					MessageBox.Show("You can't remove a root pane");
					return;
				}
				
				layout.RemovePane((BasePane)treeView1.SelectedNode.Tag);
				
				UpdateView(parent);
			}
		}

		private void nullPaneToolStripMenuItem_Click(object sender, EventArgs e) =>
			AddPane(new Pan1Pane("pan1", layout.FileByteOrder));

		private void pic1PaneToolStripMenuItem_Click(object sender, EventArgs e) =>
			AddPane(new Pic1Pane(layout.FileByteOrder));

		private void txtPaneToolStripMenuItem_Click(object sender, EventArgs e) =>
			AddPane(new Txt1Pane(layout.FileByteOrder));

		private void clonePaneToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Pan1Pane pane = treeView1.SelectedNode.Tag as Pan1Pane;
			layout.AddPane(-1, pane.Parent, pane.Clone());
			UpdateView(pane);
		}

		void AddPane(BasePane p)
		{
			if (treeView1.SelectedNode.Tag as Pan1Pane == null) return;
			layout.AddPane(-1, treeView1.SelectedNode.Tag as Pan1Pane, p);
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
			else if (e.Node.Tag is Pan1Pane)
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
			layout.GetTexturesSection().Textures.Add(name);
			UpdateView(name);
		}

		private void RemoveTexture_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Parent == null) return; //the texture must be in the root textures node
			layout.Tex1.Textures.Remove(((TextureTag)treeView1.SelectedNode.Tag).TexName);
			UpdateView();
		}

		private void RemoveMaterial_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Parent == null) return;
			layout.Mat1.Materials.Remove((BflytMaterial)treeView1.SelectedNode.Tag);
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
			layout.GetMaterialsSection().Materials.Add(next);
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

		#region Pane drag and drop
		// Reference https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.treeview.itemdrag?view=windowsdesktop-8.0
		private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
		{
			// Only allow dragging pan1 panes
			if (e.Item is TreeNode node && node.Tag is Pan1Pane pane)
			{
				// Only when they are not a root
				if (pane.Parent == null) return;
				// Only from the logical hierarchy, in theory this check doesn't really matter
				if (FindRoot(node) != Pan1Root) return;

				DoDragDrop(e.Item, DragDropEffects.Move);
			}
		}

		private void treeView1_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.AllowedEffect;
		}

		private void treeView1_DragOver(object sender, DragEventArgs e)
		{
			// Retrieve the client coordinates of the mouse position.
			Point targetPoint = treeView1.PointToClient(new Point(e.X, e.Y));

			// Select the node at the mouse position.
			treeView1.SelectedNode = treeView1.GetNodeAt(targetPoint);
		}

		private void treeView1_DragDrop(object sender, DragEventArgs e)
		{
			// Retrieve the client coordinates of the drop location.
			Point targetPoint = treeView1.PointToClient(new Point(e.X, e.Y));

			// Retrieve the node at the drop location.
			BasePane target = treeView1.GetNodeAt(targetPoint)?.Tag as BasePane;

			// Retrieve the node that was dragged.
			BasePane dragged = ((TreeNode)e.Data.GetData(typeof(TreeNode))).Tag as BasePane;

			if (target == null || dragged == null) return;

			// Confirm that the node at the drop location is not 
			// the dragged node or a descendant of the dragged node.
			if (target == dragged || dragged.ContainsChild(target)) return;
			
			layout.RemovePane(dragged);
			layout.AddPane(-1, target, dragged);
			UpdateView(dragged);
		}
		#endregion

		private void EditorView_KeyDown(object sender, KeyEventArgs e)
        {
			e.SuppressKeyPress = true;
			if (e.Shift && e.Control && e.KeyCode == Keys.S) saveBFLYTToolStripMenuItem.PerformClick();
			else if (e.Control && e.KeyCode == Keys.S) saveToolStripMenuItem.PerformClick();
			else if (e.Control && e.KeyCode == Keys.L) treeView1.ExpandAll();
			else if (e.Control && e.KeyCode == Keys.K) treeView1.CollapseAll();
			else e.SuppressKeyPress = false;
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
