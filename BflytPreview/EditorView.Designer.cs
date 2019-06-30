namespace BflytPreview
{
    partial class EditorView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorView));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveBFLYTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToSZSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.PaneMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.clonePaneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.nullPaneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pic1PaneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.txtPaneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.zoomSlider = new System.Windows.Forms.TrackBar();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.panel1 = new System.Windows.Forms.Panel();
			this.GroupMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
			this.MaterialMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.cloneMaterialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.TextureMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.newTextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.PaneMenuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.zoomSlider)).BeginInit();
			this.GroupMenuStrip.SuspendLayout();
			this.MaterialMenuStrip.SuspendLayout();
			this.TextureMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.settingsToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(800, 24);
			this.menuStrip1.TabIndex = 5;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveBFLYTToolStripMenuItem,
            this.saveToSZSToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// saveBFLYTToolStripMenuItem
			// 
			this.saveBFLYTToolStripMenuItem.Name = "saveBFLYTToolStripMenuItem";
			this.saveBFLYTToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
			this.saveBFLYTToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.saveBFLYTToolStripMenuItem.Text = "Save as...";
			this.saveBFLYTToolStripMenuItem.Click += new System.EventHandler(this.saveBFLYTToolStripMenuItem_Click);
			// 
			// saveToSZSToolStripMenuItem
			// 
			this.saveToSZSToolStripMenuItem.Name = "saveToSZSToolStripMenuItem";
			this.saveToSZSToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToSZSToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.saveToSZSToolStripMenuItem.Text = "Save to SZS";
			this.saveToSZSToolStripMenuItem.Visible = false;
			this.saveToSZSToolStripMenuItem.Click += new System.EventHandler(this.saveToSZSToolStripMenuItem_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandAllToolStripMenuItem,
            this.collapseAllToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "View";
			// 
			// expandAllToolStripMenuItem
			// 
			this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
			this.expandAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
			this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.expandAllToolStripMenuItem.Text = "Expand All";
			this.expandAllToolStripMenuItem.ToolTipText = "Expand all nodes in the tree view";
			this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
			// 
			// collapseAllToolStripMenuItem
			// 
			this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
			this.collapseAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
			this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.collapseAllToolStripMenuItem.Text = "Collapse All";
			this.collapseAllToolStripMenuItem.ToolTipText = "Collapse all nodes in the tree view";
			this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.collapseAllToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "Help";
			this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
			this.settingsToolStripMenuItem.Text = "Settings";
			this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.panel1);
			this.splitContainer1.Size = new System.Drawing.Size(800, 450);
			this.splitContainer1.SplitterDistance = 266;
			this.splitContainer1.TabIndex = 6;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.treeView1);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer2.Panel2.Controls.Add(this.zoomSlider);
			this.splitContainer2.Panel2.Controls.Add(this.propertyGrid1);
			this.splitContainer2.Size = new System.Drawing.Size(266, 450);
			this.splitContainer2.SplitterDistance = 225;
			this.splitContainer2.TabIndex = 4;
			// 
			// treeView1
			// 
			this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.treeView1.ContextMenuStrip = this.PaneMenuStrip;
			this.treeView1.Location = new System.Drawing.Point(0, 26);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(266, 201);
			this.treeView1.TabIndex = 3;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView1_NodeMouseClick);
			this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
			// 
			// PaneMenuStrip
			// 
			this.PaneMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clonePaneToolStripMenuItem,
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.toolStripSeparator1,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem});
			this.PaneMenuStrip.Name = "contextMenuStrip1";
			this.PaneMenuStrip.Size = new System.Drawing.Size(181, 142);
			// 
			// clonePaneToolStripMenuItem
			// 
			this.clonePaneToolStripMenuItem.Name = "clonePaneToolStripMenuItem";
			this.clonePaneToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.clonePaneToolStripMenuItem.Text = "Clone Pane";
			this.clonePaneToolStripMenuItem.Click += new System.EventHandler(this.clonePaneToolStripMenuItem_Click);
			// 
			// addToolStripMenuItem
			// 
			this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nullPaneToolStripMenuItem,
            this.pic1PaneToolStripMenuItem,
            this.txtPaneToolStripMenuItem});
			this.addToolStripMenuItem.Name = "addToolStripMenuItem";
			this.addToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.addToolStripMenuItem.Text = "Add";
			// 
			// nullPaneToolStripMenuItem
			// 
			this.nullPaneToolStripMenuItem.Name = "nullPaneToolStripMenuItem";
			this.nullPaneToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
			this.nullPaneToolStripMenuItem.Text = "Null Pane";
			this.nullPaneToolStripMenuItem.Click += new System.EventHandler(this.nullPaneToolStripMenuItem_Click);
			// 
			// pic1PaneToolStripMenuItem
			// 
			this.pic1PaneToolStripMenuItem.Name = "pic1PaneToolStripMenuItem";
			this.pic1PaneToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
			this.pic1PaneToolStripMenuItem.Text = "Pic1 Pane";
			this.pic1PaneToolStripMenuItem.Click += new System.EventHandler(this.pic1PaneToolStripMenuItem_Click);
			// 
			// txtPaneToolStripMenuItem
			// 
			this.txtPaneToolStripMenuItem.Name = "txtPaneToolStripMenuItem";
			this.txtPaneToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
			this.txtPaneToolStripMenuItem.Text = "Txt Pane";
			this.txtPaneToolStripMenuItem.Click += new System.EventHandler(this.txtPaneToolStripMenuItem_Click);
			// 
			// removeToolStripMenuItem
			// 
			this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
			this.removeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.removeToolStripMenuItem.Text = "Remove";
			this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
			// 
			// zoomSlider
			// 
			this.zoomSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.zoomSlider.LargeChange = 1;
			this.zoomSlider.Location = new System.Drawing.Point(3, 167);
			this.zoomSlider.Maximum = 20;
			this.zoomSlider.Minimum = 1;
			this.zoomSlider.Name = "zoomSlider";
			this.zoomSlider.Size = new System.Drawing.Size(260, 45);
			this.zoomSlider.TabIndex = 1;
			this.zoomSlider.Value = 10;
			this.zoomSlider.Scroll += new System.EventHandler(this.zoomSlider_Scroll);
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGrid1.Location = new System.Drawing.Point(3, 3);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(263, 216);
			this.propertyGrid1.TabIndex = 0;
			this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.AutoScroll = true;
			this.panel1.Location = new System.Drawing.Point(0, 27);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(527, 420);
			this.panel1.TabIndex = 0;
			// 
			// GroupMenuStrip
			// 
			this.GroupMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem6,
            this.toolStripMenuItem7});
			this.GroupMenuStrip.Name = "contextMenuStrip1";
			this.GroupMenuStrip.Size = new System.Drawing.Size(132, 48);
			// 
			// toolStripMenuItem6
			// 
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			this.toolStripMenuItem6.Size = new System.Drawing.Size(131, 22);
			this.toolStripMenuItem6.Text = "Add group";
			this.toolStripMenuItem6.Click += new System.EventHandler(this.AddGroupToolStripMenuItem_Click);
			// 
			// toolStripMenuItem7
			// 
			this.toolStripMenuItem7.Name = "toolStripMenuItem7";
			this.toolStripMenuItem7.Size = new System.Drawing.Size(131, 22);
			this.toolStripMenuItem7.Text = "Remove";
			this.toolStripMenuItem7.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
			// 
			// MaterialMenuStrip
			// 
			this.MaterialMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cloneMaterialToolStripMenuItem,
            this.removeToolStripMenuItem1});
			this.MaterialMenuStrip.Name = "MaterialMenuStrip";
			this.MaterialMenuStrip.Size = new System.Drawing.Size(152, 48);
			// 
			// cloneMaterialToolStripMenuItem
			// 
			this.cloneMaterialToolStripMenuItem.Name = "cloneMaterialToolStripMenuItem";
			this.cloneMaterialToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
			this.cloneMaterialToolStripMenuItem.Text = "Clone material";
			this.cloneMaterialToolStripMenuItem.Click += new System.EventHandler(this.CloneMaterialToolStripMenuItem_Click);
			// 
			// removeToolStripMenuItem1
			// 
			this.removeToolStripMenuItem1.Name = "removeToolStripMenuItem1";
			this.removeToolStripMenuItem1.Size = new System.Drawing.Size(151, 22);
			this.removeToolStripMenuItem1.Text = "Remove";
			this.removeToolStripMenuItem1.Click += new System.EventHandler(this.RemoveMaterial_Click);
			// 
			// TextureMenuStrip
			// 
			this.TextureMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newTextureToolStripMenuItem,
            this.toolStripMenuItem2});
			this.TextureMenuStrip.Name = "MaterialMenuStrip";
			this.TextureMenuStrip.Size = new System.Drawing.Size(138, 48);
			// 
			// newTextureToolStripMenuItem
			// 
			this.newTextureToolStripMenuItem.Name = "newTextureToolStripMenuItem";
			this.newTextureToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
			this.newTextureToolStripMenuItem.Text = "New texture";
			this.newTextureToolStripMenuItem.Click += new System.EventHandler(this.NewTextureToolStripMenuItem_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(137, 22);
			this.toolStripMenuItem2.Text = "Remove";
			this.toolStripMenuItem2.Click += new System.EventHandler(this.RemoveTexture_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
			// 
			// moveUpToolStripMenuItem
			// 
			this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
			this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.moveUpToolStripMenuItem.Text = "Move up";
			this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.MoveUpToolStripMenuItem_Click);
			// 
			// moveDownToolStripMenuItem
			// 
			this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
			this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.moveDownToolStripMenuItem.Text = "Move down";
			this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.MoveDownToolStripMenuItem_Click);
			// 
			// EditorView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.splitContainer1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "EditorView";
			this.Text = "EditorView";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EditorView_FormClosed);
			this.Load += new System.EventHandler(this.EditorView_Load);
			this.LocationChanged += new System.EventHandler(this.EditorView_LocationChanged);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditorView_KeyDown);
			this.Resize += new System.EventHandler(this.EditorView_Resize);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.PaneMenuStrip.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.zoomSlider)).EndInit();
			this.GroupMenuStrip.ResumeLayout(false);
			this.MaterialMenuStrip.ResumeLayout(false);
			this.TextureMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveBFLYTToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TrackBar zoomSlider;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToSZSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseAllToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip PaneMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nullPaneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem clonePaneToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pic1PaneToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem txtPaneToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip GroupMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
		private System.Windows.Forms.ContextMenuStrip MaterialMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem cloneMaterialToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip TextureMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem newTextureToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
	}
}