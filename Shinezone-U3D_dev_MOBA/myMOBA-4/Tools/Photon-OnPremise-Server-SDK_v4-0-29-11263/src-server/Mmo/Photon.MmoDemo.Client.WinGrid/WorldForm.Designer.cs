namespace Photon.MmoDemo.Client.WinGrid
{
    partial class WorldForm
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorldForm));
            this.tabControlTabs = new System.Windows.Forms.TabControl();
            this.tabPageLog = new System.Windows.Forms.TabPage();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelSettings = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxPlayerText = new System.Windows.Forms.TextBox();
            this.labelSendMovementInterval = new System.Windows.Forms.Label();
            this.textBoxSendMovementInterval = new System.Windows.Forms.TextBox();
            this.labelText = new System.Windows.Forms.Label();
            this.labelColor = new System.Windows.Forms.Label();
            this.buttonPlayerColor = new System.Windows.Forms.Button();
            this.labelSendReliable = new System.Windows.Forms.Label();
            this.checkBoxSendReliable = new System.Windows.Forms.CheckBox();
            this.textBoxAutoMoveInterval = new System.Windows.Forms.TextBox();
            this.checkBoxAutoMoveEnabled = new System.Windows.Forms.CheckBox();
            this.textBoxAutoMoveVelocity = new System.Windows.Forms.TextBox();
            this.labelAutoMoveVelocity = new System.Windows.Forms.Label();
            this.labelAutoMoveInterval = new System.Windows.Forms.Label();
            this.labelAutoMove = new System.Windows.Forms.Label();
            this.textBoxInfo = new System.Windows.Forms.TextBox();
            this.tabPageCounter = new System.Windows.Forms.TabPage();
            this.counterGraph = new ZedGraph.ZedGraphControl();
            this.tabPageRadar = new Photon.MmoDemo.Client.WinGrid.RadarTabPage();
            this.tabControlTabs.SuspendLayout();
            this.tabPageLog.SuspendLayout();
            this.tabPageSettings.SuspendLayout();
            this.tableLayoutPanelSettings.SuspendLayout();
            this.tabPageCounter.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlTabs
            // 
            this.tabControlTabs.Controls.Add(this.tabPageLog);
            this.tabControlTabs.Controls.Add(this.tabPageSettings);
            this.tabControlTabs.Controls.Add(this.tabPageCounter);
            this.tabControlTabs.Controls.Add(this.tabPageRadar);
            this.tabControlTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlTabs.Location = new System.Drawing.Point(0, 0);
            this.tabControlTabs.Name = "tabControlTabs";
            this.tabControlTabs.Padding = new System.Drawing.Point(2, 2);
            this.tabControlTabs.SelectedIndex = 0;
            this.tabControlTabs.Size = new System.Drawing.Size(344, 368);
            this.tabControlTabs.TabIndex = 0;
            this.tabControlTabs.KeyPress += new System.Windows.Forms.KeyPressEventHandler(OnKeyPress);
            this.tabControlTabs.SelectedIndexChanged += new System.EventHandler(this.OnTabControlTabsSelectedIndexChanged);
            this.tabControlTabs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            // 
            // tabPageLog
            // 
            this.tabPageLog.Controls.Add(this.textBoxLog);
            this.tabPageLog.Location = new System.Drawing.Point(4, 20);
            this.tabPageLog.Name = "tabPageLog";
            this.tabPageLog.Size = new System.Drawing.Size(336, 344);
            this.tabPageLog.TabIndex = 2;
            this.tabPageLog.Text = "Log";
            this.tabPageLog.UseVisualStyleBackColor = true;
            // 
            // textBoxLog
            // 
            this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxLog.Location = new System.Drawing.Point(0, 0);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxLog.Size = new System.Drawing.Size(336, 344);
            this.textBoxLog.TabIndex = 0;
            // 
            // tabPageSettings
            // 
            this.tabPageSettings.Controls.Add(this.tableLayoutPanelSettings);
            this.tabPageSettings.Controls.Add(this.textBoxInfo);
            this.tabPageSettings.Location = new System.Drawing.Point(4, 20);
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSettings.Size = new System.Drawing.Size(336, 344);
            this.tabPageSettings.TabIndex = 0;
            this.tabPageSettings.Text = "Settings";
            this.tabPageSettings.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelSettings
            // 
            this.tableLayoutPanelSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelSettings.ColumnCount = 2;
            this.tableLayoutPanelSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelSettings.Controls.Add(this.textBoxPlayerText, 1, 0);
            this.tableLayoutPanelSettings.Controls.Add(this.labelSendMovementInterval, 0, 2);
            this.tableLayoutPanelSettings.Controls.Add(this.textBoxSendMovementInterval, 1, 2);
            this.tableLayoutPanelSettings.Controls.Add(this.labelText, 0, 0);
            this.tableLayoutPanelSettings.Controls.Add(this.labelColor, 0, 1);
            this.tableLayoutPanelSettings.Controls.Add(this.buttonPlayerColor, 1, 1);
            this.tableLayoutPanelSettings.Controls.Add(this.labelSendReliable, 0, 3);
            this.tableLayoutPanelSettings.Controls.Add(this.checkBoxSendReliable, 1, 3);
            this.tableLayoutPanelSettings.Controls.Add(this.textBoxAutoMoveInterval, 1, 4);
            this.tableLayoutPanelSettings.Controls.Add(this.checkBoxAutoMoveEnabled, 1, 6);
            this.tableLayoutPanelSettings.Controls.Add(this.textBoxAutoMoveVelocity, 1, 5);
            this.tableLayoutPanelSettings.Controls.Add(this.labelAutoMoveVelocity, 0, 5);
            this.tableLayoutPanelSettings.Controls.Add(this.labelAutoMoveInterval, 0, 4);
            this.tableLayoutPanelSettings.Controls.Add(this.labelAutoMove, 0, 6);
            this.tableLayoutPanelSettings.Location = new System.Drawing.Point(3, 72);
            this.tableLayoutPanelSettings.Name = "tableLayoutPanelSettings";
            this.tableLayoutPanelSettings.RowCount = 7;
            this.tableLayoutPanelSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelSettings.Size = new System.Drawing.Size(330, 264);
            this.tableLayoutPanelSettings.TabIndex = 5;
            // 
            // textBoxPlayerText
            // 
            this.textBoxPlayerText.Location = new System.Drawing.Point(154, 3);
            this.textBoxPlayerText.Name = "textBoxPlayerText";
            this.textBoxPlayerText.Size = new System.Drawing.Size(100, 20);
            this.textBoxPlayerText.TabIndex = 1;
            this.textBoxPlayerText.TextChanged += new System.EventHandler(this.OnTextBoxNameTextChanged);
            // 
            // labelSendMovementInterval
            // 
            this.labelSendMovementInterval.AutoSize = true;
            this.labelSendMovementInterval.Location = new System.Drawing.Point(3, 52);
            this.labelSendMovementInterval.Name = "labelSendMovementInterval";
            this.labelSendMovementInterval.Size = new System.Drawing.Size(145, 13);
            this.labelSendMovementInterval.TabIndex = 0;
            this.labelSendMovementInterval.Text = "Send Movement Interval (ms)";
            // 
            // textBoxSendMovementInterval
            // 
            this.textBoxSendMovementInterval.Location = new System.Drawing.Point(154, 55);
            this.textBoxSendMovementInterval.Name = "textBoxSendMovementInterval";
            this.textBoxSendMovementInterval.Size = new System.Drawing.Size(100, 20);
            this.textBoxSendMovementInterval.TabIndex = 4;
            this.textBoxSendMovementInterval.Text = "100";
            this.textBoxSendMovementInterval.Leave += new System.EventHandler(this.OnTextBoxSendMovementLeave);
            this.textBoxSendMovementInterval.Validating += new System.ComponentModel.CancelEventHandler(OnNumericTextBoxValidating);
            // 
            // labelText
            // 
            this.labelText.AutoSize = true;
            this.labelText.Location = new System.Drawing.Point(3, 0);
            this.labelText.Name = "labelText";
            this.labelText.Size = new System.Drawing.Size(60, 13);
            this.labelText.TabIndex = 0;
            this.labelText.Text = "Player Text";
            // 
            // labelColor
            // 
            this.labelColor.AutoSize = true;
            this.labelColor.Location = new System.Drawing.Point(3, 26);
            this.labelColor.Name = "labelColor";
            this.labelColor.Size = new System.Drawing.Size(63, 13);
            this.labelColor.TabIndex = 0;
            this.labelColor.Text = "Player Color";
            // 
            // buttonPlayerColor
            // 
            this.buttonPlayerColor.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonPlayerColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPlayerColor.Location = new System.Drawing.Point(154, 29);
            this.buttonPlayerColor.Name = "buttonPlayerColor";
            this.buttonPlayerColor.Size = new System.Drawing.Size(20, 20);
            this.buttonPlayerColor.TabIndex = 2;
            this.buttonPlayerColor.UseVisualStyleBackColor = true;
            this.buttonPlayerColor.Click += new System.EventHandler(this.OnButtonColorClick);
            // 
            // labelSendReliable
            // 
            this.labelSendReliable.AutoSize = true;
            this.labelSendReliable.Location = new System.Drawing.Point(3, 78);
            this.labelSendReliable.Name = "labelSendReliable";
            this.labelSendReliable.Size = new System.Drawing.Size(68, 13);
            this.labelSendReliable.TabIndex = 0;
            this.labelSendReliable.Text = "Send reliable";
            // 
            // checkBoxSendReliable
            // 
            this.checkBoxSendReliable.AutoSize = true;
            this.checkBoxSendReliable.Location = new System.Drawing.Point(154, 81);
            this.checkBoxSendReliable.Name = "checkBoxSendReliable";
            this.checkBoxSendReliable.Size = new System.Drawing.Size(64, 17);
            this.checkBoxSendReliable.TabIndex = 6;
            this.checkBoxSendReliable.Text = "enabled";
            this.checkBoxSendReliable.UseVisualStyleBackColor = true;
            this.checkBoxSendReliable.CheckedChanged += new System.EventHandler(this.OnCheckBoxSendReliableCheckedChanged);
            // 
            // textBoxAutoMoveInterval
            // 
            this.textBoxAutoMoveInterval.Location = new System.Drawing.Point(154, 104);
            this.textBoxAutoMoveInterval.Name = "textBoxAutoMoveInterval";
            this.textBoxAutoMoveInterval.Size = new System.Drawing.Size(100, 20);
            this.textBoxAutoMoveInterval.TabIndex = 8;
            this.textBoxAutoMoveInterval.Text = "500";
            this.textBoxAutoMoveInterval.Leave += new System.EventHandler(this.OnTextBoxAutoMoveIntervalLeave);
            this.textBoxAutoMoveInterval.Validating += new System.ComponentModel.CancelEventHandler(OnNumericTextBoxValidating);
            // 
            // checkBoxAutoMoveEnabled
            // 
            this.checkBoxAutoMoveEnabled.AutoSize = true;
            this.checkBoxAutoMoveEnabled.Location = new System.Drawing.Point(154, 156);
            this.checkBoxAutoMoveEnabled.Name = "checkBoxAutoMoveEnabled";
            this.checkBoxAutoMoveEnabled.Size = new System.Drawing.Size(64, 17);
            this.checkBoxAutoMoveEnabled.TabIndex = 10;
            this.checkBoxAutoMoveEnabled.Text = "enabled";
            this.checkBoxAutoMoveEnabled.UseVisualStyleBackColor = true;
            this.checkBoxAutoMoveEnabled.CheckedChanged += new System.EventHandler(this.OnCheckBoxAutoMoveCheckedChanged);
            // 
            // textBoxAutoMoveVelocity
            // 
            this.textBoxAutoMoveVelocity.Location = new System.Drawing.Point(154, 130);
            this.textBoxAutoMoveVelocity.Name = "textBoxAutoMoveVelocity";
            this.textBoxAutoMoveVelocity.Size = new System.Drawing.Size(100, 20);
            this.textBoxAutoMoveVelocity.TabIndex = 9;
            this.textBoxAutoMoveVelocity.Text = "1";
            this.textBoxAutoMoveVelocity.Leave += new System.EventHandler(this.OnTextBoxAutoMoveVelocityLeave);
            this.textBoxAutoMoveVelocity.Validating += new System.ComponentModel.CancelEventHandler(OnNumericTextBoxValidating);
            // 
            // labelAutoMoveVelocity
            // 
            this.labelAutoMoveVelocity.AutoSize = true;
            this.labelAutoMoveVelocity.Location = new System.Drawing.Point(3, 127);
            this.labelAutoMoveVelocity.Name = "labelAutoMoveVelocity";
            this.labelAutoMoveVelocity.Size = new System.Drawing.Size(99, 13);
            this.labelAutoMoveVelocity.TabIndex = 9;
            this.labelAutoMoveVelocity.Text = "Auto Move Velocity";
            // 
            // labelAutoMoveInterval
            // 
            this.labelAutoMoveInterval.AutoSize = true;
            this.labelAutoMoveInterval.Location = new System.Drawing.Point(3, 101);
            this.labelAutoMoveInterval.Name = "labelAutoMoveInterval";
            this.labelAutoMoveInterval.Size = new System.Drawing.Size(119, 13);
            this.labelAutoMoveInterval.TabIndex = 14;
            this.labelAutoMoveInterval.Text = "Auto Move Interval (ms)";
            // 
            // labelAutoMove
            // 
            this.labelAutoMove.AutoSize = true;
            this.labelAutoMove.Location = new System.Drawing.Point(3, 153);
            this.labelAutoMove.Name = "labelAutoMove";
            this.labelAutoMove.Size = new System.Drawing.Size(59, 13);
            this.labelAutoMove.TabIndex = 11;
            this.labelAutoMove.Text = "Auto Move";
            // 
            // textBoxInfo
            // 
            this.textBoxInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxInfo.Enabled = false;
            this.textBoxInfo.Location = new System.Drawing.Point(3, 3);
            this.textBoxInfo.Multiline = true;
            this.textBoxInfo.Name = "textBoxInfo";
            this.textBoxInfo.ReadOnly = true;
            this.textBoxInfo.Size = new System.Drawing.Size(330, 63);
            this.textBoxInfo.TabIndex = 0;
            this.textBoxInfo.Text = "w a s d or Num Pad (1, 2, 3, 4, 5, 6, 7, 8, 9) to move your player.\r\n+ - to " +
                "add or remove players.\r\n[ ] to change view area.\r\nSpace to switch to settings and back to player view.";
            // 
            // tabPageCounter
            // 
            this.tabPageCounter.Controls.Add(this.counterGraph);
            this.tabPageCounter.Location = new System.Drawing.Point(4, 20);
            this.tabPageCounter.Name = "tabPageCounter";
            this.tabPageCounter.Size = new System.Drawing.Size(336, 344);
            this.tabPageCounter.TabIndex = 3;
            this.tabPageCounter.Text = "Counter";
            this.tabPageCounter.UseVisualStyleBackColor = true;
            // 
            // counterGraph
            // 
            this.counterGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.counterGraph.Location = new System.Drawing.Point(0, 0);
            this.counterGraph.Name = "counterGraph";
            this.counterGraph.ScrollGrace = 0;
            this.counterGraph.ScrollMaxX = 0;
            this.counterGraph.ScrollMaxY = 0;
            this.counterGraph.ScrollMaxY2 = 0;
            this.counterGraph.ScrollMinX = 0;
            this.counterGraph.ScrollMinY = 0;
            this.counterGraph.ScrollMinY2 = 0;
            this.counterGraph.Size = new System.Drawing.Size(336, 344);
            this.counterGraph.TabIndex = 0;
            // 
            // tabPageRadar
            // 
            this.tabPageRadar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPageRadar.BackgroundImage")));
            this.tabPageRadar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tabPageRadar.Location = new System.Drawing.Point(4, 20);
            this.tabPageRadar.Name = "tabPageRadar";
            this.tabPageRadar.Size = new System.Drawing.Size(336, 344);
            this.tabPageRadar.TabIndex = 4;
            this.tabPageRadar.Text = "Radar";
            this.tabPageRadar.UseVisualStyleBackColor = true;
            // 
            // WorldForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 368);
            this.Controls.Add(this.tabControlTabs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(360, 406);
            this.Name = "WorldForm";
            this.Text = "Photon Mmo Demo v2";
            this.Load += new System.EventHandler(this.WorldForm_OnLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.tabControlTabs.ResumeLayout(false);
            this.tabPageLog.ResumeLayout(false);
            this.tabPageLog.PerformLayout();
            this.tabPageSettings.ResumeLayout(false);
            this.tabPageSettings.PerformLayout();
            this.tableLayoutPanelSettings.ResumeLayout(false);
            this.tableLayoutPanelSettings.PerformLayout();
            this.tabPageCounter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlTabs;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.TextBox textBoxInfo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSettings;
        private System.Windows.Forms.Label labelText;
        private System.Windows.Forms.TextBox textBoxPlayerText;
        private System.Windows.Forms.Label labelSendReliable;
        private System.Windows.Forms.Label labelSendMovementInterval;
        private System.Windows.Forms.Button buttonPlayerColor;
        private System.Windows.Forms.Label labelColor;
        private System.Windows.Forms.CheckBox checkBoxSendReliable;
        private System.Windows.Forms.TextBox textBoxSendMovementInterval;
        private System.Windows.Forms.TabPage tabPageLog;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.TextBox textBoxAutoMoveVelocity;
        private System.Windows.Forms.Label labelAutoMoveVelocity;
        private System.Windows.Forms.CheckBox checkBoxAutoMoveEnabled;
        private System.Windows.Forms.Label labelAutoMove;
        private System.Windows.Forms.Label labelAutoMoveInterval;
        private System.Windows.Forms.TextBox textBoxAutoMoveInterval;
        private System.Windows.Forms.TabPage tabPageCounter;
        private ZedGraph.ZedGraphControl counterGraph;
        private RadarTabPage tabPageRadar;
    }
}