using SupervisorModbusWinForms;
using System;
using System.Text;
using System.Windows.Forms;

namespace modbus_self
{
    public partial class MainForm : Form
    {
        private readonly SupervisorModbusServer _server = new SupervisorModbusServer();

        public MainForm()
        {
            InitializeComponent();
            InitUi();
            BindServerEvents();
        }

        private void InitUi()
        {
            txtIp.Text = "127.0.0.1";
            txtPort.Text = "1502";

            cmbRecipe.Items.Clear();
            cmbRecipe.Items.AddRange(new object[] { "標A", "標B", "標C" });
            cmbRecipe.SelectedIndex = 0;

            cmbDirection.Items.Clear();
            cmbDirection.Items.AddRange(new object[] { "L", "R" });
            cmbDirection.SelectedIndex = 0;

            cmbResult.Items.Clear();
            cmbResult.Items.AddRange(new object[] { "OK", "NG" });
            cmbResult.SelectedIndex = 0;

            lblStatus.Text = "Disconnected";
            lblStatus.BackColor = Color.Cyan;
        }

        private void BindServerEvents()
        {
            _server.LogGenerated += msg =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => AppendLog(msg)));
                    return;
                }
                AppendLog(msg);
            };

            _server.StatusChanged += msg =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => lblStatus.Text = msg));
                    return;
                }
                lblStatus.Text = msg;
            };
        }

        private void AppendLog(string msg)
        {
            txtLog.AppendText(msg + Environment.NewLine);
        }

        private void LoadSnapshotToUi(SupervisorModbusServer.LatestData d)
        {
            dtpDate.Value = d.Date;
            txtMeasureId.Text = d.MeasureId;
            txtWorkOrder.Text = d.WorkOrderNo;
            txtSerial.Text = d.SerialNo;
            cmbRecipe.Text = d.Recipe;
            cmbDirection.Text = d.Direction;
            txtPosY.Text = d.PositionY;
            txtWidth.Text = d.Width;
            txtHeight.Text = d.Height;
            cmbResult.Text = d.Result;
            txtPath.Text = d.SavePath;
            txtUser.Text = d.User;

            txtCurrentData.Text =
                $"Date      : {d.Date:yyyy-MM-dd}{Environment.NewLine}" +
                $"MeasureId : {d.MeasureId}{Environment.NewLine}" +
                $"WorkOrder : {d.WorkOrderNo}{Environment.NewLine}" +
                $"SerialNo  : {d.SerialNo}{Environment.NewLine}" +
                $"Recipe    : {d.Recipe}{Environment.NewLine}" +
                $"Direction : {d.Direction}{Environment.NewLine}" +
                $"PositionY : {d.PositionY}{Environment.NewLine}" +
                $"Width     : {d.Width}{Environment.NewLine}" +
                $"Height    : {d.Height}{Environment.NewLine}" +
                $"Result    : {d.Result}{Environment.NewLine}" +
                $"SavePath  : {d.SavePath}{Environment.NewLine}" +
                $"User      : {d.User}";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                int port = int.Parse(txtPort.Text.Trim());
                _server.Start(port);

                var snapshot = _server.GetSnapshot();
                LoadSnapshotToUi(snapshot);

                lblStatus.Text = $"Server started on {txtIp.Text}:{port}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Start Error");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _server.Stop();
            lblStatus.Text = "Server stopped";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            var snapshot = _server.GetSnapshot();
            LoadSnapshotToUi(snapshot);
        }

        private void btnShowRegs_Click(object sender, EventArgs e)
        {
            var regs = _server.GetHoldingRegistersSnapshot();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < regs.Length; i++)
            {
                if (regs[i] != 0)
                {
                    sb.AppendLine($"4{(i + 1).ToString("0000")} (offset {i}) = {regs[i]}");
                }
            }

            if (sb.Length == 0)
                sb.AppendLine("No non-zero registers.");

            txtCurrentData.Text = sb.ToString();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                var data = new SupervisorModbusServer.LatestData
                {
                    Date = dtpDate.Value.Date,
                    MeasureId = txtMeasureId.Text.Trim(),
                    WorkOrderNo = txtWorkOrder.Text.Trim(),
                    SerialNo = txtSerial.Text.Trim(),
                    Recipe = cmbRecipe.Text.Trim(),
                    Direction = cmbDirection.Text.Trim(),
                    PositionY = txtPosY.Text.Trim(),
                    Width = txtWidth.Text.Trim(),
                    Height = txtHeight.Text.Trim(),
                    Result = cmbResult.Text.Trim(),
                    SavePath = txtPath.Text.Trim(),
                    User = txtUser.Text.Trim()
                };

                _server.UpdateData(data);
                LoadSnapshotToUi(_server.GetSnapshot());

                MessageBox.Show("資料已更新");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Apply Error");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _server.Stop();
        }
    }
}