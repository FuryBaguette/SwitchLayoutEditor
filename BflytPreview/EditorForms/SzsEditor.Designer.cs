namespace BflytPreview.EditorForms
{
	partial class SzsEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SzsEditor));
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.replaceToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.saveSzsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToSzsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.extractAllFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.layoutDiffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.thisFileIsTheOriginalSzsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.thisFileIsTheEditedSzsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadJSONPatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tb_search = new System.Windows.Forms.TextBox();
			this.contextMenuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBox1.ContextMenuStrip = this.contextMenuStrip1;
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new System.Drawing.Point(0, 27);
			this.listBox1.Name = "listBox1";
			this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBox1.Size = new System.Drawing.Size(303, 212);
			this.listBox1.Sorted = true;
			this.listBox1.TabIndex = 4;
			this.listBox1.DoubleClick += new System.EventHandler(this.listBox1_DoubleClick);
			this.listBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox1_KeyDown);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyNameToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.extractToolStripMenuItem,
            this.replaceToolStripMenuItem1,
            this.toolStripSeparator1,
            this.deleteToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(136, 120);
			// 
			// copyNameToolStripMenuItem
			// 
			this.copyNameToolStripMenuItem.Name = "copyNameToolStripMenuItem";
			this.copyNameToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
			this.copyNameToolStripMenuItem.Text = "Copy name";
			this.copyNameToolStripMenuItem.Click += new System.EventHandler(this.copyNameToolStripMenuItem_Click);
			// 
			// renameToolStripMenuItem
			// 
			this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
			this.renameToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
			this.renameToolStripMenuItem.Text = "Rename";
			this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
			// 
			// extractToolStripMenuItem
			// 
			this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
			this.extractToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
			this.extractToolStripMenuItem.Text = "Extract";
			this.extractToolStripMenuItem.Click += new System.EventHandler(this.extractToolStripMenuItem_Click);
			// 
			// replaceToolStripMenuItem1
			// 
			this.replaceToolStripMenuItem1.Name = "replaceToolStripMenuItem1";
			this.replaceToolStripMenuItem1.Size = new System.Drawing.Size(135, 22);
			this.replaceToolStripMenuItem1.Text = "Replace";
			this.replaceToolStripMenuItem1.Click += new System.EventHandler(this.replaceToolStripMenuItem1_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(132, 6);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
			this.deleteToolStripMenuItem.Text = "Delete";
			this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.numericUpDown1.Location = new System.Drawing.Point(130, 253);
			this.numericUpDown1.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(55, 20);
			this.numericUpDown1.TabIndex = 7;
			this.numericUpDown1.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 256);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(122, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Compression level [0-9] :";
			// 
			// saveSzsToolStripMenuItem
			// 
			this.saveSzsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAsToolStripMenuItem,
            this.saveToSzsToolStripMenuItem});
			this.saveSzsToolStripMenuItem.Name = "saveSzsToolStripMenuItem";
			this.saveSzsToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
			this.saveSzsToolStripMenuItem.Text = "Save";
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			this.saveAsToolStripMenuItem.Text = "Save as..";
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
			// 
			// saveToSzsToolStripMenuItem
			// 
			this.saveToSzsToolStripMenuItem.Name = "saveToSzsToolStripMenuItem";
			this.saveToSzsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToSzsToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			this.saveToSzsToolStripMenuItem.Text = "Save to Szs";
			this.saveToSzsToolStripMenuItem.Visible = false;
			this.saveToSzsToolStripMenuItem.Click += new System.EventHandler(this.saveToSzsToolStripMenuItem_Click);
			// 
			// addFilesToolStripMenuItem
			// 
			this.addFilesToolStripMenuItem.Name = "addFilesToolStripMenuItem";
			this.addFilesToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
			this.addFilesToolStripMenuItem.Text = "Add files";
			this.addFilesToolStripMenuItem.Click += new System.EventHandler(this.addFilesToolStripMenuItem_Click);
			// 
			// extractAllFilesToolStripMenuItem
			// 
			this.extractAllFilesToolStripMenuItem.Name = "extractAllFilesToolStripMenuItem";
			this.extractAllFilesToolStripMenuItem.Size = new System.Drawing.Size(94, 20);
			this.extractAllFilesToolStripMenuItem.Text = "Extract all files";
			this.extractAllFilesToolStripMenuItem.Click += new System.EventHandler(this.extractAllFilesToolStripMenuItem_Click);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveSzsToolStripMenuItem,
            this.addFilesToolStripMenuItem,
            this.extractAllFilesToolStripMenuItem,
            this.toolsToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(303, 24);
			this.menuStrip1.TabIndex = 5;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.layoutDiffToolStripMenuItem,
            this.loadJSONPatchToolStripMenuItem});
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
			this.toolsToolStripMenuItem.Text = "Tools";
			// 
			// layoutDiffToolStripMenuItem
			// 
			this.layoutDiffToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.thisFileIsTheOriginalSzsToolStripMenuItem,
            this.thisFileIsTheEditedSzsToolStripMenuItem});
			this.layoutDiffToolStripMenuItem.Name = "layoutDiffToolStripMenuItem";
			this.layoutDiffToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.layoutDiffToolStripMenuItem.Text = "Layout diff";
			// 
			// thisFileIsTheOriginalSzsToolStripMenuItem
			// 
			this.thisFileIsTheOriginalSzsToolStripMenuItem.Name = "thisFileIsTheOriginalSzsToolStripMenuItem";
			this.thisFileIsTheOriginalSzsToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
			this.thisFileIsTheOriginalSzsToolStripMenuItem.Text = "This file is the original szs";
			this.thisFileIsTheOriginalSzsToolStripMenuItem.Click += new System.EventHandler(this.thisFileIsTheOriginalSzsToolStripMenuItem_Click);
			// 
			// thisFileIsTheEditedSzsToolStripMenuItem
			// 
			this.thisFileIsTheEditedSzsToolStripMenuItem.Name = "thisFileIsTheEditedSzsToolStripMenuItem";
			this.thisFileIsTheEditedSzsToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
			this.thisFileIsTheEditedSzsToolStripMenuItem.Text = "This file is the edited szs";
			this.thisFileIsTheEditedSzsToolStripMenuItem.Click += new System.EventHandler(this.thisFileIsTheEditedSzsToolStripMenuItem_Click);
			// 
			// loadJSONPatchToolStripMenuItem
			// 
			this.loadJSONPatchToolStripMenuItem.Name = "loadJSONPatchToolStripMenuItem";
			this.loadJSONPatchToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.loadJSONPatchToolStripMenuItem.Text = "Load JSON patch";
			this.loadJSONPatchToolStripMenuItem.Click += new System.EventHandler(this.loadJSONPatchToolStripMenuItem_Click);
			// 
			// tb_search
			// 
			this.tb_search.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tb_search.Location = new System.Drawing.Point(195, 253);
			this.tb_search.Name = "tb_search";
			this.tb_search.Size = new System.Drawing.Size(100, 20);
			this.tb_search.TabIndex = 8;
			this.tb_search.Text = "Search";
			this.tb_search.TextChanged += new System.EventHandler(this.tb_search_TextChanged);
			// 
			// SzsEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(303, 279);
			this.Controls.Add(this.tb_search);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "SzsEditor";
			this.Text = "Szs editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SzsEditor_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SzsEditor_FormClosed);
			this.Load += new System.EventHandler(this.SzsEditor_Load);
			this.LocationChanged += new System.EventHandler(this.SzsEditor_LocationChanged);
			this.Click += new System.EventHandler(this.SzsEditor_Click);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SzsEditor_KeyDown);
			this.contextMenuStrip1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem copyNameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem extractToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ToolStripMenuItem saveSzsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToSzsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addFilesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem extractAllFilesToolStripMenuItem;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem layoutDiffToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem thisFileIsTheOriginalSzsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem thisFileIsTheEditedSzsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadJSONPatchToolStripMenuItem;
		private System.Windows.Forms.TextBox tb_search;
	}
}