namespace GTurtle
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
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnNewScript = new System.Windows.Forms.ToolStripButton();
            this.btnOpenScript = new System.Windows.Forms.ToolStripButton();
            this.btnSaveScript = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnPlay = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.btnStepInto = new System.Windows.Forms.ToolStripButton();
            this.btnStepOver = new System.Windows.Forms.ToolStripButton();
            this.btnStepOut = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmbSurfaceSize = new System.Windows.Forms.ToolStripComboBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.stLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnPause = new System.Windows.Forms.ToolStripButton();
            this.toolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dockPanel
            // 
            this.dockPanel.ActiveAutoHideContent = null;
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.Location = new System.Drawing.Point(0, 39);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Size = new System.Drawing.Size(1006, 422);
            this.dockPanel.TabIndex = 3;
            // 
            // toolStrip
            // 
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNewScript,
            this.btnOpenScript,
            this.btnSaveScript,
            this.toolStripSeparator1,
            this.btnPlay,
            this.btnStop,
            this.btnPause,
            this.btnStepInto,
            this.btnStepOver,
            this.btnStepOut,
            this.toolStripSeparator2,
            this.cmbSurfaceSize});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1006, 39);
            this.toolStrip.TabIndex = 6;
            this.toolStrip.Text = "toolStrip1";
            // 
            // btnNewScript
            // 
            this.btnNewScript.AutoToolTip = false;
            this.btnNewScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNewScript.Image = ((System.Drawing.Image)(resources.GetObject("btnNewScript.Image")));
            this.btnNewScript.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnNewScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNewScript.Name = "btnNewScript";
            this.btnNewScript.Size = new System.Drawing.Size(36, 36);
            this.btnNewScript.Text = "btnNewScript";
            this.btnNewScript.ToolTipText = "New Script";
            this.btnNewScript.Click += new System.EventHandler(this.btnNewScript_Click);
            // 
            // btnOpenScript
            // 
            this.btnOpenScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpenScript.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenScript.Image")));
            this.btnOpenScript.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnOpenScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpenScript.Name = "btnOpenScript";
            this.btnOpenScript.Size = new System.Drawing.Size(36, 36);
            this.btnOpenScript.Text = "btnOpenScript";
            this.btnOpenScript.ToolTipText = "Open Script";
            this.btnOpenScript.Click += new System.EventHandler(this.btnOpenScript_Click);
            // 
            // btnSaveScript
            // 
            this.btnSaveScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSaveScript.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveScript.Image")));
            this.btnSaveScript.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnSaveScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveScript.Name = "btnSaveScript";
            this.btnSaveScript.Size = new System.Drawing.Size(36, 36);
            this.btnSaveScript.Text = "btnSaveScript";
            this.btnSaveScript.ToolTipText = "Save Script";
            this.btnSaveScript.Click += new System.EventHandler(this.btnSaveScript_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // btnPlay
            // 
            this.btnPlay.Image = ((System.Drawing.Image)(resources.GetObject("btnPlay.Image")));
            this.btnPlay.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPlay.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(77, 36);
            this.btnPlay.Text = "PLAY";
            this.btnPlay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPlay.ToolTipText = "Play";
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnStop
            // 
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(67, 36);
            this.btnStop.Text = "STOP";
            this.btnStop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStepInto
            // 
            this.btnStepInto.Image = ((System.Drawing.Image)(resources.GetObject("btnStepInto.Image")));
            this.btnStepInto.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStepInto.Name = "btnStepInto";
            this.btnStepInto.Size = new System.Drawing.Size(102, 36);
            this.btnStepInto.Text = "STEP INTO";
            this.btnStepInto.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStepInto.Click += new System.EventHandler(this.btnStepInto_Click);
            // 
            // btnStepOver
            // 
            this.btnStepOver.Image = ((System.Drawing.Image)(resources.GetObject("btnStepOver.Image")));
            this.btnStepOver.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStepOver.Name = "btnStepOver";
            this.btnStepOver.Size = new System.Drawing.Size(106, 36);
            this.btnStepOver.Text = "STEP OVER";
            this.btnStepOver.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStepOver.Click += new System.EventHandler(this.btnStepOver_Click);
            // 
            // btnStepOut
            // 
            this.btnStepOut.Image = ((System.Drawing.Image)(resources.GetObject("btnStepOut.Image")));
            this.btnStepOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStepOut.Name = "btnStepOut";
            this.btnStepOut.Size = new System.Drawing.Size(98, 36);
            this.btnStepOut.Text = "STEP OUT";
            this.btnStepOut.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStepOut.Click += new System.EventHandler(this.btnStepOut_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // cmbSurfaceSize
            // 
            this.cmbSurfaceSize.AutoSize = false;
            this.cmbSurfaceSize.AutoToolTip = true;
            this.cmbSurfaceSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSurfaceSize.Name = "cmbSurfaceSize";
            this.cmbSurfaceSize.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.cmbSurfaceSize.Size = new System.Drawing.Size(121, 28);
            this.cmbSurfaceSize.ToolTipText = "Surface Size";
            this.cmbSurfaceSize.SelectedIndexChanged += new System.EventHandler(this.cmbSurfaceSize_SelectedIndexChanged);
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 461);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1006, 25);
            this.statusStrip.TabIndex = 7;
            this.statusStrip.Text = "statusStrip1";
            // 
            // stLabel
            // 
            this.stLabel.Name = "stLabel";
            this.stLabel.Size = new System.Drawing.Size(53, 20);
            this.stLabel.Text = "Label1";
            // 
            // btnPause
            // 
            this.btnPause.Image = ((System.Drawing.Image)(resources.GetObject("btnPause.Image")));
            this.btnPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(76, 36);
            this.btnPause.Text = "PAUSE";
            this.btnPause.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 486);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.IsMdiContainer = true;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GTurtle";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripButton btnOpenScript;
        private System.Windows.Forms.ToolStripButton btnNewScript;
        private System.Windows.Forms.ToolStripButton btnSaveScript;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnPlay;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripComboBox cmbSurfaceSize;
        private System.Windows.Forms.ToolStripButton btnStop;
        private System.Windows.Forms.ToolStripButton btnStepInto;
        private System.Windows.Forms.ToolStripButton btnStepOver;
        private System.Windows.Forms.ToolStripButton btnStepOut;
        private System.Windows.Forms.ToolStripStatusLabel stLabel;
        private System.Windows.Forms.ToolStripButton btnPause;
    }
}