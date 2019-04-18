namespace BflytPreview
{
	partial class BflanEditor
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
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToArchiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.exportToJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.importFromJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGrid1.Location = new System.Drawing.Point(179, 24);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(360, 387);
			this.propertyGrid1.TabIndex = 0;
			this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PropertyGrid1_PropertyValueChanged);
			// 
			// treeView1
			// 
			this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
			this.treeView1.Location = new System.Drawing.Point(0, 24);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(173, 387);
			this.treeView1.TabIndex = 1;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView1_AfterSelect);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(540, 24);
			this.menuStrip1.TabIndex = 2;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.saveToArchiveToolStripMenuItem,
            this.toolStripSeparator1,
            this.exportToJSONToolStripMenuItem,
            this.importFromJSONToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
			this.saveToolStripMenuItem.Text = "Save...";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
			// 
			// saveToArchiveToolStripMenuItem
			// 
			this.saveToArchiveToolStripMenuItem.Name = "saveToArchiveToolStripMenuItem";
			this.saveToArchiveToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
			this.saveToArchiveToolStripMenuItem.Text = "Save to SZS";
			this.saveToArchiveToolStripMenuItem.Visible = false;
			this.saveToArchiveToolStripMenuItem.Click += new System.EventHandler(this.SaveToArchiveToolStripMenuItem_Click);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addEntryToolStripMenuItem,
            this.deleteToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(127, 48);
			// 
			// addEntryToolStripMenuItem
			// 
			this.addEntryToolStripMenuItem.Name = "addEntryToolStripMenuItem";
			this.addEntryToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.addEntryToolStripMenuItem.Text = "Add entry";
			this.addEntryToolStripMenuItem.Click += new System.EventHandler(this.AddEntryToolStripMenuItem_Click);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.deleteToolStripMenuItem.Text = "Delete";
			this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
			// 
			// exportToJSONToolStripMenuItem
			// 
			this.exportToJSONToolStripMenuItem.Name = "exportToJSONToolStripMenuItem";
			this.exportToJSONToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.exportToJSONToolStripMenuItem.Text = "Export to JSON";
			this.exportToJSONToolStripMenuItem.Click += new System.EventHandler(this.ExportToJSONToolStripMenuItem_Click);
			// 
			// importFromJSONToolStripMenuItem
			// 
			this.importFromJSONToolStripMenuItem.Name = "importFromJSONToolStripMenuItem";
			this.importFromJSONToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.importFromJSONToolStripMenuItem.Text = "Import from JSON";
			this.importFromJSONToolStripMenuItem.Click += new System.EventHandler(this.ImportFromJSONToolStripMenuItem_Click);
			// 
			// BflanEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(540, 415);
			this.Controls.Add(this.treeView1);
			this.Controls.Add(this.propertyGrid1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "BflanEditor";
			this.Text = "BflanEditor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BflanEditor_FormClosing);
			this.Load += new System.EventHandler(this.BflanEditor_Load);
			this.LocationChanged += new System.EventHandler(this.BflanEditor_LocationChanged);
			this.Click += new System.EventHandler(this.BflanEditor_Click);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToArchiveToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem addEntryToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem exportToJSONToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importFromJSONToolStripMenuItem;
	}
}