namespace modbus_self
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support.
        /// </summary>
        private void InitializeComponent()
        {
            lblTitle = new Label();
            lblIp = new Label();
            lblPort = new Label();
            txtIp = new TextBox();
            txtPort = new TextBox();
            lblReadTitle = new Label();
            txtCurrentData = new RichTextBox();
            btnStart = new Button();
            btnStop = new Button();
            btnRefresh = new Button();
            btnShowRegs = new Button();
            lblWriteTitle = new Label();
            dtpDate = new DateTimePicker();
            lblPosY = new Label();
            lblWidth = new Label();
            lblHeight = new Label();
            lblResult = new Label();
            lblPath = new Label();
            lblUser = new Label();
            cmbDirection = new ComboBox();
            cmbRecipe = new ComboBox();
            txtSerial = new TextBox();
            txtMeasureId = new TextBox();
            txtWorkOrder = new TextBox();
            txtUser = new TextBox();
            txtPath = new TextBox();
            txtHeight = new TextBox();
            txtWidth = new TextBox();
            txtPosY = new TextBox();
            lblDirection = new Label();
            lblRecipe = new Label();
            lblSerial = new Label();
            lblWorkOrder = new Label();
            lblMeasureId = new Label();
            lblDate = new Label();
            cmbResult = new ComboBox();
            btnApply = new Button();
            lblLogTitle = new Label();
            txtLog = new TextBox();
            statusStrip1 = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.Location = new Point(20, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(249, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "ModbusTCP (Ethernet)";
            // 
            // lblIp
            // 
            lblIp.AutoSize = true;
            lblIp.Location = new Point(20, 55);
            lblIp.Name = "lblIp";
            lblIp.Size = new Size(105, 15);
            lblIp.TabIndex = 1;
            lblIp.Text = "Server IP-Address";
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Location = new Point(140, 55);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(68, 15);
            lblPort.TabIndex = 2;
            lblPort.Text = "Server Port";
            // 
            // txtIp
            // 
            txtIp.Location = new Point(20, 75);
            txtIp.Name = "txtIp";
            txtIp.Size = new Size(100, 23);
            txtIp.TabIndex = 3;
            // 
            // txtPort
            // 
            txtPort.Location = new Point(140, 75);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(70, 23);
            txtPort.TabIndex = 4;
            // 
            // lblReadTitle
            // 
            lblReadTitle.AutoSize = true;
            lblReadTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblReadTitle.Location = new Point(20, 120);
            lblReadTitle.Name = "lblReadTitle";
            lblReadTitle.Size = new Size(195, 21);
            lblReadTitle.TabIndex = 5;
            lblReadTitle.Text = "Read values from Server";
            // 
            // txtCurrentData
            // 
            txtCurrentData.Location = new Point(20, 145);
            txtCurrentData.Name = "txtCurrentData";
            txtCurrentData.Size = new Size(350, 135);
            txtCurrentData.TabIndex = 6;
            txtCurrentData.Text = "";
            // 
            // btnStart
            // 
            btnStart.BackColor = Color.Lime;
            btnStart.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnStart.Location = new Point(250, 55);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(85, 45);
            btnStart.TabIndex = 7;
            btnStart.Text = "start";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.BackColor = Color.Red;
            btnStop.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnStop.Location = new Point(350, 55);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(85, 45);
            btnStop.TabIndex = 8;
            btnStop.Text = "stop";
            btnStop.UseVisualStyleBackColor = false;
            btnStop.Click += btnStop_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(20, 290);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(110, 28);
            btnRefresh.TabIndex = 9;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnShowRegs
            // 
            btnShowRegs.Location = new Point(145, 290);
            btnShowRegs.Name = "btnShowRegs";
            btnShowRegs.Size = new Size(110, 28);
            btnShowRegs.TabIndex = 10;
            btnShowRegs.Text = "Show";
            btnShowRegs.UseVisualStyleBackColor = true;
            btnShowRegs.Click += btnShowRegs_Click;
            // 
            // lblWriteTitle
            // 
            lblWriteTitle.AutoSize = true;
            lblWriteTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblWriteTitle.Location = new Point(66, 340);
            lblWriteTitle.Name = "lblWriteTitle";
            lblWriteTitle.Size = new Size(178, 21);
            lblWriteTitle.TabIndex = 11;
            lblWriteTitle.Text = "Write values to Server";
            // 
            // dtpDate
            // 
            dtpDate.Location = new Point(166, 372);
            dtpDate.Name = "dtpDate";
            dtpDate.Size = new Size(150, 23);
            dtpDate.TabIndex = 12;
            // 
            // lblPosY
            // 
            lblPosY.AutoSize = true;
            lblPosY.Location = new Point(336, 375);
            lblPosY.Name = "lblPosY";
            lblPosY.Size = new Size(34, 15);
            lblPosY.TabIndex = 13;
            lblPosY.Text = "PosY";
            // 
            // lblWidth
            // 
            lblWidth.AutoSize = true;
            lblWidth.Location = new Point(336, 405);
            lblWidth.Name = "lblWidth";
            lblWidth.Size = new Size(41, 15);
            lblWidth.TabIndex = 14;
            lblWidth.Text = "Width";
            // 
            // lblHeight
            // 
            lblHeight.AutoSize = true;
            lblHeight.Location = new Point(336, 435);
            lblHeight.Name = "lblHeight";
            lblHeight.Size = new Size(45, 15);
            lblHeight.TabIndex = 15;
            lblHeight.Text = "Height";
            // 
            // lblResult
            // 
            lblResult.AutoSize = true;
            lblResult.Location = new Point(336, 465);
            lblResult.Name = "lblResult";
            lblResult.Size = new Size(41, 15);
            lblResult.TabIndex = 16;
            lblResult.Text = "Result";
            // 
            // lblPath
            // 
            lblPath.AutoSize = true;
            lblPath.Location = new Point(336, 495);
            lblPath.Name = "lblPath";
            lblPath.Size = new Size(59, 15);
            lblPath.TabIndex = 17;
            lblPath.Text = "SavePath";
            // 
            // lblUser
            // 
            lblUser.AutoSize = true;
            lblUser.Location = new Point(336, 525);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(32, 15);
            lblUser.TabIndex = 18;
            lblUser.Text = "User";
            // 
            // cmbDirection
            // 
            cmbDirection.FormattingEnabled = true;
            cmbDirection.Location = new Point(166, 522);
            cmbDirection.Name = "cmbDirection";
            cmbDirection.Size = new Size(150, 23);
            cmbDirection.TabIndex = 19;
            // 
            // cmbRecipe
            // 
            cmbRecipe.FormattingEnabled = true;
            cmbRecipe.Location = new Point(166, 492);
            cmbRecipe.Name = "cmbRecipe";
            cmbRecipe.Size = new Size(150, 23);
            cmbRecipe.TabIndex = 20;
            // 
            // txtSerial
            // 
            txtSerial.Location = new Point(166, 462);
            txtSerial.Name = "txtSerial";
            txtSerial.Size = new Size(150, 23);
            txtSerial.TabIndex = 21;
            // 
            // txtMeasureId
            // 
            txtMeasureId.Location = new Point(166, 402);
            txtMeasureId.Name = "txtMeasureId";
            txtMeasureId.Size = new Size(150, 23);
            txtMeasureId.TabIndex = 22;
            // 
            // txtWorkOrder
            // 
            txtWorkOrder.Location = new Point(166, 432);
            txtWorkOrder.Name = "txtWorkOrder";
            txtWorkOrder.Size = new Size(150, 23);
            txtWorkOrder.TabIndex = 23;
            // 
            // txtUser
            // 
            txtUser.Location = new Point(406, 522);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(170, 23);
            txtUser.TabIndex = 24;
            // 
            // txtPath
            // 
            txtPath.Location = new Point(406, 492);
            txtPath.Name = "txtPath";
            txtPath.Size = new Size(170, 23);
            txtPath.TabIndex = 25;
            // 
            // txtHeight
            // 
            txtHeight.Location = new Point(406, 432);
            txtHeight.Name = "txtHeight";
            txtHeight.Size = new Size(170, 23);
            txtHeight.TabIndex = 26;
            // 
            // txtWidth
            // 
            txtWidth.Location = new Point(406, 402);
            txtWidth.Name = "txtWidth";
            txtWidth.Size = new Size(170, 23);
            txtWidth.TabIndex = 27;
            // 
            // txtPosY
            // 
            txtPosY.Location = new Point(406, 372);
            txtPosY.Name = "txtPosY";
            txtPosY.Size = new Size(170, 23);
            txtPosY.TabIndex = 28;
            // 
            // lblDirection
            // 
            lblDirection.AutoSize = true;
            lblDirection.Location = new Point(86, 525);
            lblDirection.Name = "lblDirection";
            lblDirection.Size = new Size(58, 15);
            lblDirection.TabIndex = 29;
            lblDirection.Text = "Direction";
            // 
            // lblRecipe
            // 
            lblRecipe.AutoSize = true;
            lblRecipe.Location = new Point(86, 495);
            lblRecipe.Name = "lblRecipe";
            lblRecipe.Size = new Size(46, 15);
            lblRecipe.TabIndex = 30;
            lblRecipe.Text = "Recipe";
            // 
            // lblSerial
            // 
            lblSerial.AutoSize = true;
            lblSerial.Location = new Point(86, 465);
            lblSerial.Name = "lblSerial";
            lblSerial.Size = new Size(38, 15);
            lblSerial.TabIndex = 31;
            lblSerial.Text = "Serial";
            // 
            // lblWorkOrder
            // 
            lblWorkOrder.AutoSize = true;
            lblWorkOrder.Location = new Point(86, 435);
            lblWorkOrder.Name = "lblWorkOrder";
            lblWorkOrder.Size = new Size(70, 15);
            lblWorkOrder.TabIndex = 32;
            lblWorkOrder.Text = "WorkOrder";
            // 
            // lblMeasureId
            // 
            lblMeasureId.AutoSize = true;
            lblMeasureId.Location = new Point(86, 405);
            lblMeasureId.Name = "lblMeasureId";
            lblMeasureId.Size = new Size(67, 15);
            lblMeasureId.TabIndex = 33;
            lblMeasureId.Text = "MeasureId";
            // 
            // lblDate
            // 
            lblDate.AutoSize = true;
            lblDate.Location = new Point(86, 375);
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(34, 15);
            lblDate.TabIndex = 34;
            lblDate.Text = "Date";
            // 
            // cmbResult
            // 
            cmbResult.FormattingEnabled = true;
            cmbResult.Location = new Point(406, 462);
            cmbResult.Name = "cmbResult";
            cmbResult.Size = new Size(170, 23);
            cmbResult.TabIndex = 35;
            // 
            // btnApply
            // 
            btnApply.BackColor = Color.Yellow;
            btnApply.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnApply.Location = new Point(606, 492);
            btnApply.Name = "btnApply";
            btnApply.Size = new Size(136, 53);
            btnApply.TabIndex = 36;
            btnApply.Text = "Apply / Update";
            btnApply.UseVisualStyleBackColor = false;
            btnApply.Click += btnApply_Click;
            // 
            // lblLogTitle
            // 
            lblLogTitle.AutoSize = true;
            lblLogTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblLogTitle.Location = new Point(400, 120);
            lblLogTitle.Name = "lblLogTitle";
            lblLogTitle.Size = new Size(165, 21);
            lblLogTitle.TabIndex = 37;
            lblLogTitle.Text = "Communication Log";
            // 
            // txtLog
            // 
            txtLog.Location = new Point(400, 145);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(388, 216);
            txtLog.TabIndex = 38;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatus });
            statusStrip1.Location = new Point(0, 579);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(812, 22);
            statusStrip1.TabIndex = 39;
            statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(84, 17);
            lblStatus.Text = "Disconnected";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(812, 601);
            Controls.Add(statusStrip1);
            Controls.Add(txtLog);
            Controls.Add(lblLogTitle);
            Controls.Add(btnApply);
            Controls.Add(cmbResult);
            Controls.Add(lblDate);
            Controls.Add(lblMeasureId);
            Controls.Add(lblWorkOrder);
            Controls.Add(lblSerial);
            Controls.Add(lblRecipe);
            Controls.Add(lblDirection);
            Controls.Add(txtPosY);
            Controls.Add(txtWidth);
            Controls.Add(txtHeight);
            Controls.Add(txtPath);
            Controls.Add(txtUser);
            Controls.Add(txtWorkOrder);
            Controls.Add(txtMeasureId);
            Controls.Add(txtSerial);
            Controls.Add(cmbRecipe);
            Controls.Add(cmbDirection);
            Controls.Add(lblUser);
            Controls.Add(lblPath);
            Controls.Add(lblResult);
            Controls.Add(lblHeight);
            Controls.Add(lblWidth);
            Controls.Add(lblPosY);
            Controls.Add(dtpDate);
            Controls.Add(lblWriteTitle);
            Controls.Add(btnShowRegs);
            Controls.Add(btnRefresh);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(txtCurrentData);
            Controls.Add(lblReadTitle);
            Controls.Add(txtPort);
            Controls.Add(txtIp);
            Controls.Add(lblPort);
            Controls.Add(lblIp);
            Controls.Add(lblTitle);
            Name = "MainForm";
            Text = "Supervisor Modbus TCP Server";
            FormClosing += MainForm_FormClosing;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private Label lblIp;
        private Label lblPort;
        private TextBox txtIp;
        private TextBox txtPort;
        private Label lblReadTitle;
        private RichTextBox txtCurrentData;
        private Button btnStart;
        private Button btnStop;
        private Button btnRefresh;
        private Button btnShowRegs;
        private Label lblWriteTitle;
        private DateTimePicker dtpDate;
        private Label lblPosY;
        private Label lblWidth;
        private Label lblHeight;
        private Label lblResult;
        private Label lblPath;
        private Label lblUser;
        private ComboBox cmbDirection;
        private ComboBox cmbRecipe;
        private TextBox txtSerial;
        private TextBox txtMeasureId;
        private TextBox txtWorkOrder;
        private TextBox txtUser;
        private TextBox txtPath;
        private TextBox txtHeight;
        private TextBox txtWidth;
        private TextBox txtPosY;
        private Label lblDirection;
        private Label lblRecipe;
        private Label lblSerial;
        private Label lblWorkOrder;
        private Label lblMeasureId;
        private Label lblDate;
        private ComboBox cmbResult;
        private Button btnApply;
        private Label lblLogTitle;
        private TextBox txtLog;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblStatus;
    }
}