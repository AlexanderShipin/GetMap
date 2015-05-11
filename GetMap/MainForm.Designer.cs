namespace GetMap
{
	partial class MainForm
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
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.buildMapButton = new System.Windows.Forms.Button();
			this.buildingStatusLabel = new System.Windows.Forms.Label();
			this.saveMapDialog = new System.Windows.Forms.SaveFileDialog();
			this.leftTopLatTextBox = new System.Windows.Forms.TextBox();
			this.leftTopLatLabel = new System.Windows.Forms.Label();
			this.leftTopLonLabel = new System.Windows.Forms.Label();
			this.leftTopLonTextBox = new System.Windows.Forms.TextBox();
			this.rightBottomLonLabel = new System.Windows.Forms.Label();
			this.rightBottomLonTextBox = new System.Windows.Forms.TextBox();
			this.rightBottomLatLabel = new System.Windows.Forms.Label();
			this.rightBottomLatTextBox = new System.Windows.Forms.TextBox();
			this.zoomLabel = new System.Windows.Forms.Label();
			this.zoomTextBox = new System.Windows.Forms.TextBox();
			this.zoomTrackBar = new System.Windows.Forms.TrackBar();
			this.sourceLabel = new System.Windows.Forms.Label();
			this.sourceComboBox = new System.Windows.Forms.ComboBox();
			this.pathLabel = new System.Windows.Forms.Label();
			this.pathTextBox = new System.Windows.Forms.TextBox();
			this.pathButton = new System.Windows.Forms.Button();
			this.zoomExamplePictureBox = new System.Windows.Forms.PictureBox();
			this.cancelButton = new System.Windows.Forms.Button();
			this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
			this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			((System.ComponentModel.ISupportInitialize)(this.zoomTrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.zoomExamplePictureBox)).BeginInit();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// buildMapButton
			// 
			this.buildMapButton.Location = new System.Drawing.Point(313, 170);
			this.buildMapButton.Name = "buildMapButton";
			this.buildMapButton.Size = new System.Drawing.Size(75, 23);
			this.buildMapButton.TabIndex = 8;
			this.buildMapButton.Text = "Build map";
			this.buildMapButton.UseVisualStyleBackColor = true;
			this.buildMapButton.Click += new System.EventHandler(this.BuildMapButton_Click);
			// 
			// buildingStatusLabel
			// 
			this.buildingStatusLabel.AutoSize = true;
			this.buildingStatusLabel.Location = new System.Drawing.Point(24, 175);
			this.buildingStatusLabel.Name = "buildingStatusLabel";
			this.buildingStatusLabel.Size = new System.Drawing.Size(0, 13);
			this.buildingStatusLabel.TabIndex = 0;
			// 
			// saveMapDialog
			// 
			this.saveMapDialog.DefaultExt = "png";
			this.saveMapDialog.Filter = ".png|*.png";
			this.saveMapDialog.InitialDirectory = "C:\\";
			// 
			// leftTopLatTextBox
			// 
			this.leftTopLatTextBox.Location = new System.Drawing.Point(114, 41);
			this.leftTopLatTextBox.Name = "leftTopLatTextBox";
			this.leftTopLatTextBox.Size = new System.Drawing.Size(71, 20);
			this.leftTopLatTextBox.TabIndex = 2;
			this.leftTopLatTextBox.Text = "57.645670";
			this.leftTopLatTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.leftTopLatTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
			this.leftTopLatTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
			// 
			// leftTopLatLabel
			// 
			this.leftTopLatLabel.AutoSize = true;
			this.leftTopLatLabel.Location = new System.Drawing.Point(12, 44);
			this.leftTopLatLabel.Name = "leftTopLatLabel";
			this.leftTopLatLabel.Size = new System.Drawing.Size(83, 13);
			this.leftTopLatLabel.TabIndex = 0;
			this.leftTopLatLabel.Text = "Left top latitude:";
			// 
			// leftTopLonLabel
			// 
			this.leftTopLonLabel.AutoSize = true;
			this.leftTopLonLabel.Location = new System.Drawing.Point(12, 70);
			this.leftTopLonLabel.Name = "leftTopLonLabel";
			this.leftTopLonLabel.Size = new System.Drawing.Size(92, 13);
			this.leftTopLonLabel.TabIndex = 0;
			this.leftTopLonLabel.Text = "Left top longitude:";
			// 
			// leftTopLonTextBox
			// 
			this.leftTopLonTextBox.Location = new System.Drawing.Point(114, 67);
			this.leftTopLonTextBox.Name = "leftTopLonTextBox";
			this.leftTopLonTextBox.Size = new System.Drawing.Size(71, 20);
			this.leftTopLonTextBox.TabIndex = 3;
			this.leftTopLonTextBox.Text = "39.805716";
			this.leftTopLonTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.leftTopLonTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
			this.leftTopLonTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
			// 
			// rightBottomLonLabel
			// 
			this.rightBottomLonLabel.AutoSize = true;
			this.rightBottomLonLabel.Location = new System.Drawing.Point(191, 70);
			this.rightBottomLonLabel.Name = "rightBottomLonLabel";
			this.rightBottomLonLabel.Size = new System.Drawing.Size(116, 13);
			this.rightBottomLonLabel.TabIndex = 0;
			this.rightBottomLonLabel.Text = "Right bottom longitude:";
			// 
			// rightBottomLonTextBox
			// 
			this.rightBottomLonTextBox.Location = new System.Drawing.Point(317, 67);
			this.rightBottomLonTextBox.Name = "rightBottomLonTextBox";
			this.rightBottomLonTextBox.Size = new System.Drawing.Size(71, 20);
			this.rightBottomLonTextBox.TabIndex = 5;
			this.rightBottomLonTextBox.Text = "39.910257";
			this.rightBottomLonTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.rightBottomLonTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
			this.rightBottomLonTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
			// 
			// rightBottomLatLabel
			// 
			this.rightBottomLatLabel.AutoSize = true;
			this.rightBottomLatLabel.Location = new System.Drawing.Point(191, 44);
			this.rightBottomLatLabel.Name = "rightBottomLatLabel";
			this.rightBottomLatLabel.Size = new System.Drawing.Size(107, 13);
			this.rightBottomLatLabel.TabIndex = 0;
			this.rightBottomLatLabel.Text = "Right bottom latitude:";
			// 
			// rightBottomLatTextBox
			// 
			this.rightBottomLatTextBox.Location = new System.Drawing.Point(317, 41);
			this.rightBottomLatTextBox.Name = "rightBottomLatTextBox";
			this.rightBottomLatTextBox.Size = new System.Drawing.Size(71, 20);
			this.rightBottomLatTextBox.TabIndex = 4;
			this.rightBottomLatTextBox.Text = "57.613543";
			this.rightBottomLatTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.rightBottomLatTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
			this.rightBottomLatTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
			// 
			// zoomLabel
			// 
			this.zoomLabel.AutoSize = true;
			this.zoomLabel.Location = new System.Drawing.Point(12, 124);
			this.zoomLabel.Name = "zoomLabel";
			this.zoomLabel.Size = new System.Drawing.Size(37, 13);
			this.zoomLabel.TabIndex = 0;
			this.zoomLabel.Text = "Zoom:";
			// 
			// zoomTextBox
			// 
			this.zoomTextBox.Location = new System.Drawing.Point(114, 121);
			this.zoomTextBox.Name = "zoomTextBox";
			this.zoomTextBox.ReadOnly = true;
			this.zoomTextBox.Size = new System.Drawing.Size(22, 20);
			this.zoomTextBox.TabIndex = 0;
			this.zoomTextBox.TabStop = false;
			this.zoomTextBox.Text = "16";
			this.zoomTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// zoomTrackBar
			// 
			this.zoomTrackBar.LargeChange = 1;
			this.zoomTrackBar.Location = new System.Drawing.Point(142, 119);
			this.zoomTrackBar.Maximum = 19;
			this.zoomTrackBar.Name = "zoomTrackBar";
			this.zoomTrackBar.Size = new System.Drawing.Size(246, 45);
			this.zoomTrackBar.TabIndex = 7;
			this.zoomTrackBar.Value = 16;
			this.zoomTrackBar.ValueChanged += new System.EventHandler(this.zoomTrackBar_ValueChanged);
			// 
			// sourceLabel
			// 
			this.sourceLabel.AutoSize = true;
			this.sourceLabel.Location = new System.Drawing.Point(12, 15);
			this.sourceLabel.Name = "sourceLabel";
			this.sourceLabel.Size = new System.Drawing.Size(44, 13);
			this.sourceLabel.TabIndex = 0;
			this.sourceLabel.Text = "Source:";
			// 
			// sourceComboBox
			// 
			this.sourceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.sourceComboBox.FormattingEnabled = true;
			this.sourceComboBox.Location = new System.Drawing.Point(114, 12);
			this.sourceComboBox.Name = "sourceComboBox";
			this.sourceComboBox.Size = new System.Drawing.Size(274, 21);
			this.sourceComboBox.TabIndex = 1;
			this.sourceComboBox.SelectedIndexChanged += new System.EventHandler(this.sourceComboBox_SelectedIndexChanged);
			// 
			// pathLabel
			// 
			this.pathLabel.AutoSize = true;
			this.pathLabel.Location = new System.Drawing.Point(12, 97);
			this.pathLabel.Name = "pathLabel";
			this.pathLabel.Size = new System.Drawing.Size(32, 13);
			this.pathLabel.TabIndex = 0;
			this.pathLabel.Text = "Path:";
			// 
			// pathTextBox
			// 
			this.pathTextBox.Location = new System.Drawing.Point(114, 93);
			this.pathTextBox.Name = "pathTextBox";
			this.pathTextBox.ReadOnly = true;
			this.pathTextBox.Size = new System.Drawing.Size(247, 20);
			this.pathTextBox.TabIndex = 0;
			this.pathTextBox.TabStop = false;
			this.pathTextBox.Text = "C:\\GetMapTmp\\";
			// 
			// pathButton
			// 
			this.pathButton.Location = new System.Drawing.Point(367, 92);
			this.pathButton.Name = "pathButton";
			this.pathButton.Size = new System.Drawing.Size(21, 23);
			this.pathButton.TabIndex = 6;
			this.pathButton.Text = "..";
			this.pathButton.UseVisualStyleBackColor = true;
			this.pathButton.Click += new System.EventHandler(this.PathButton_Click);
			// 
			// zoomExamplePictureBox
			// 
			this.zoomExamplePictureBox.Location = new System.Drawing.Point(80, 199);
			this.zoomExamplePictureBox.Name = "zoomExamplePictureBox";
			this.zoomExamplePictureBox.Size = new System.Drawing.Size(256, 256);
			this.zoomExamplePictureBox.TabIndex = 9;
			this.zoomExamplePictureBox.TabStop = false;
			// 
			// cancelButton
			// 
			this.cancelButton.Enabled = false;
			this.cancelButton.Location = new System.Drawing.Point(232, 170);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 9;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// backgroundWorker
			// 
			this.backgroundWorker.WorkerReportsProgress = true;
			this.backgroundWorker.WorkerSupportsCancellation = true;
			this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
			this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
			this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.toolStripProgressBar,
				this.toolStripStatusLabel});
			this.statusStrip.Location = new System.Drawing.Point(0, 470);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(400, 22);
			this.statusStrip.SizingGrip = false;
			this.statusStrip.TabIndex = 0;
			this.statusStrip.Text = "statusStrip";
			// 
			// toolStripProgressBar
			// 
			this.toolStripProgressBar.Name = "toolStripProgressBar";
			this.toolStripProgressBar.Size = new System.Drawing.Size(100, 16);
			// 
			// toolStripStatusLabel
			// 
			this.toolStripStatusLabel.Name = "toolStripStatusLabel";
			this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
			// 
			// MainForm
			// 
			this.AcceptButton = this.buildMapButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(400, 492);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.zoomExamplePictureBox);
			this.Controls.Add(this.zoomTrackBar);
			this.Controls.Add(this.pathButton);
			this.Controls.Add(this.pathTextBox);
			this.Controls.Add(this.pathLabel);
			this.Controls.Add(this.sourceComboBox);
			this.Controls.Add(this.sourceLabel);
			this.Controls.Add(this.zoomLabel);
			this.Controls.Add(this.zoomTextBox);
			this.Controls.Add(this.rightBottomLonLabel);
			this.Controls.Add(this.rightBottomLonTextBox);
			this.Controls.Add(this.rightBottomLatLabel);
			this.Controls.Add(this.rightBottomLatTextBox);
			this.Controls.Add(this.leftTopLonLabel);
			this.Controls.Add(this.leftTopLonTextBox);
			this.Controls.Add(this.leftTopLatLabel);
			this.Controls.Add(this.leftTopLatTextBox);
			this.Controls.Add(this.buildingStatusLabel);
			this.Controls.Add(this.buildMapButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "MainForm";
			this.Text = "GetMap";
			((System.ComponentModel.ISupportInitialize)(this.zoomTrackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.zoomExamplePictureBox)).EndInit();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buildMapButton;
		private System.Windows.Forms.Label buildingStatusLabel;
		private System.Windows.Forms.SaveFileDialog saveMapDialog;
		private System.Windows.Forms.TextBox leftTopLatTextBox;
		private System.Windows.Forms.Label leftTopLatLabel;
		private System.Windows.Forms.Label leftTopLonLabel;
		private System.Windows.Forms.TextBox leftTopLonTextBox;
		private System.Windows.Forms.Label rightBottomLonLabel;
		private System.Windows.Forms.TextBox rightBottomLonTextBox;
		private System.Windows.Forms.Label rightBottomLatLabel;
		private System.Windows.Forms.TextBox rightBottomLatTextBox;
		private System.Windows.Forms.Label zoomLabel;
		private System.Windows.Forms.TextBox zoomTextBox;
		private System.Windows.Forms.Label sourceLabel;
		private System.Windows.Forms.ComboBox sourceComboBox;
		private System.Windows.Forms.Label pathLabel;
		private System.Windows.Forms.TextBox pathTextBox;
		private System.Windows.Forms.Button pathButton;
		private System.Windows.Forms.TrackBar zoomTrackBar;
		private System.Windows.Forms.PictureBox zoomExamplePictureBox;
		private System.Windows.Forms.Button cancelButton;
		private System.ComponentModel.BackgroundWorker backgroundWorker;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
	}
}

