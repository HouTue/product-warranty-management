//using System;
//using System.Drawing;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using NoSQL_QL_BaoHanh.Auth;

//namespace NoSQL_QL_BaoHanh.Forms
//{
//    public class RepairDetailForm : Form
//    {
//        private readonly string _technicianId;
//        private readonly RepairRecord _repairRecord;
//        private readonly RepairRepository _repairRepo = new RepairRepository();

//        private Label lblTicket, lblSerial, lblStartDate;
//        private TextBox txtDescription, txtPartsUsed, txtCost;
//        private Button btnStartRepair, btnCompleteRepair, btnClose;

//        public RepairDetailForm(string technicianId, RepairRecord repairRecord)
//        {
//            _technicianId = technicianId;
//            _repairRecord = repairRecord;
//            InitializeComponent();
//        }

//        private void InitializeComponent()
//        {
//            this.Text = "Chi tiết sửa chữa bảo hành";
//            this.Size = new Size(600, 500);
//            this.StartPosition = FormStartPosition.CenterParent;
//            this.BackColor = Color.White;
//            this.Font = new Font("Segoe UI", 11);

//            int left = 30;
//            int top = 30;

//            lblTicket = new Label() { Text = $"Mã phiếu yêu cầu: {_repairRecord.TicketId}", Location = new Point(left, top), AutoSize = true };
//            top += 30;
//            lblSerial = new Label() { Text = $"Sản phẩm: {_repairRecord.SerialNumber}", Location = new Point(left, top), AutoSize = true };
//            top += 30;
//            lblStartDate = new Label() { Text = $"Ngày bắt đầu: {_repairRecord.StartDate:yyyy-MM-dd}", Location = new Point(left, top), AutoSize = true };

//            top += 40;
//            txtDescription = new TextBox()
//            {
//                Location = new Point(left, top),
//                Size = new Size(500, 100),
//                Multiline = true,
//                PlaceholderText = "Nhập mô tả quá trình sửa chữa..."
//            };

//            top += 120;
//            txtPartsUsed = new TextBox()
//            {
//                Location = new Point(left, top),
//                Size = new Size(500, 30),
//                PlaceholderText = "Nhập linh kiện đã sử dụng (nếu có)"
//            };

//            top += 40;
//            txtCost = new TextBox()
//            {
//                Location = new Point(left, top),
//                Size = new Size(200, 30),
//                PlaceholderText = "Chi phí (VND)"
//            };

//            btnStartRepair = new Button()
//            {
//                Text = "🚀 Bắt đầu sửa",
//                Location = new Point(left, top + 50),
//                Size = new Size(150, 40),
//                BackColor = Color.FromArgb(0, 120, 215),
//                ForeColor = Color.White,
//                FlatStyle = FlatStyle.Flat
//            };
//            btnStartRepair.Click += BtnStartRepair_Click;

//            btnCompleteRepair = new Button()
//            {
//                Text = "✅ Hoàn thành",
//                Location = new Point(left + 160, top + 50),
//                Size = new Size(150, 40),
//                BackColor = Color.FromArgb(0, 150, 136),
//                ForeColor = Color.White,
//                FlatStyle = FlatStyle.Flat
//            };
//            btnCompleteRepair.Click += BtnCompleteRepair_Click;

//            btnClose = new Button()
//            {
//                Text = "❌ Đóng",
//                Location = new Point(left + 320, top + 50),
//                Size = new Size(120, 40),
//                BackColor = Color.Gray,
//                ForeColor = Color.White,
//                FlatStyle = FlatStyle.Flat
//            };
//            btnClose.Click += (s, e) => this.Close();

//            this.Controls.Add(lblTicket);
//            this.Controls.Add(lblSerial);
//            this.Controls.Add(lblStartDate);
//            this.Controls.Add(txtDescription);
//            this.Controls.Add(txtPartsUsed);
//            this.Controls.Add(txtCost);
//            this.Controls.Add(btnStartRepair);
//            this.Controls.Add(btnCompleteRepair);
//            this.Controls.Add(btnClose);
//        }

//        private async void BtnStartRepair_Click(object sender, EventArgs e)
//        {
//            bool success = await _repairRepo.UpdateRepairStatusAsync(_technicianId, _repairRecord.RepairId, "in_progress");
//            if (success)
//            {
//                MessageBox.Show("Đã chuyển sang trạng thái: Đang sửa!", "Thành công");
//                this.Close();
//            }
//        }

//        private async void BtnCompleteRepair_Click(object sender, EventArgs e)
//        {
//            if (!decimal.TryParse(txtCost.Text, out decimal cost))
//            {
//                MessageBox.Show("Chi phí không hợp lệ!", "Lỗi");
//                return;
//            }

//            _repairRecord.RepairDescription = txtDescription.Text;
//            _repairRecord.PartsUsed = txtPartsUsed.Text;
//            _repairRecord.Cost = cost;
//            _repairRecord.CompleteDate = DateTime.Now;
//            _repairRecord.Status = "completed";

//            bool success = await _repairRepo.CreateRepairAsync(_repairRecord);
//            if (success)
//            {
//                MessageBox.Show("Đã hoàn thành sửa chữa!", "Thành công");
//                this.Close();
//            }
//            else
//            {
//                MessageBox.Show("Lỗi khi lưu thông tin!", "Lỗi");
//            }
//        }
//    }
//}

//using System;
//using System.Drawing;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using NoSQL_QL_BaoHanh.Auth;

//namespace NoSQL_QL_BaoHanh.Forms
//{
//    public class RepairDetailForm : Form
//    {
//        private readonly RepairRepository _repairRepo;
//        private readonly WarrantyTicketRepository _ticketRepo;
//        private readonly RepairRecord _repairRecord;

//        // UI Controls
//        private Label lblTicketId, lblSerialNumber, lblStartDate;
//        private TextBox txtIssueDescription, txtPartsUsed, txtRepairNotes, txtCost;
//        private Button btnStartRepair, btnCompleteRepair, btnClose;

//        public RepairDetailForm(RepairRecord repairRecord)
//        {
//            _repairRecord = repairRecord;
//            _repairRepo = new RepairRepository();
//            _ticketRepo = new WarrantyTicketRepository();

//            InitializeUI();
//            LoadRepairInfo();
//        }

//        private void InitializeUI()
//        {
//            this.Text = "Chi tiết sửa chữa bảo hành";
//            this.Size = new Size(550, 500);
//            this.StartPosition = FormStartPosition.CenterParent;
//            this.Font = new Font("Segoe UI", 11);
//            this.BackColor = Color.White;

//            // Labels
//            AddLabel("Mã phiếu yêu cầu:", 20, 20);
//            lblTicketId = AddLabel("", 200, 20);

//            AddLabel("Mã sản phẩm:", 20, 60);
//            lblSerialNumber = AddLabel("", 200, 60);

//            AddLabel("Ngày bắt đầu:", 20, 100);
//            lblStartDate = AddLabel("", 200, 100);

//            AddLabel("Mô tả lỗi:", 20, 140);
//            txtIssueDescription = AddTextBox(20, 170, 480, 60, true);

//            AddLabel("Linh kiện sử dụng:", 20, 240);
//            txtPartsUsed = AddTextBox(20, 270, 480, 30);

//            AddLabel("Chi phí sửa chữa (VND):", 20, 310);
//            txtCost = AddTextBox(20, 340, 200, 30);

//            AddLabel("Ghi chú sửa chữa:", 20, 380);
//            txtRepairNotes = AddTextBox(20, 410, 480, 60, false, true);

//            // Buttons
//            btnStartRepair = AddButton("🛠 Bắt đầu sửa", 20, 480, BtnStartRepair_Click);
//            btnCompleteRepair = AddButton("✅ Hoàn thành", 200, 480, BtnCompleteRepair_Click);
//            btnClose = AddButton("✖ Đóng", 380, 480, (s, e) => this.Close());
//        }

//        private Label AddLabel(string text, int left, int top)
//        {
//            var lbl = new Label
//            {
//                Text = text,
//                Left = left,
//                Top = top,
//                AutoSize = true
//            };
//            this.Controls.Add(lbl);
//            return lbl;
//        }

//        private TextBox AddTextBox(int left, int top, int width, int height, bool readOnly = false, bool multiline = false)
//        {
//            var txt = new TextBox
//            {
//                Left = left,
//                Top = top,
//                Width = width,
//                Height = height,
//                ReadOnly = readOnly,
//                Multiline = multiline,
//                ScrollBars = multiline ? ScrollBars.Vertical : ScrollBars.None
//            };
//            this.Controls.Add(txt);
//            return txt;
//        }

//        private Button AddButton(string text, int left, int top, EventHandler onClick)
//        {
//            var btn = new Button
//            {
//                Text = text,
//                Left = left,
//                Top = top,
//                Width = 150,
//                Height = 40,
//                BackColor = Color.FromArgb(0, 120, 215),
//                ForeColor = Color.White,
//                FlatStyle = FlatStyle.Flat
//            };
//            btn.Click += onClick;
//            this.Controls.Add(btn);
//            return btn;
//        }

//        private void LoadRepairInfo()
//        {
//            lblTicketId.Text = _repairRecord.TicketId;
//            lblSerialNumber.Text = _repairRecord.SerialNumber;
//            lblStartDate.Text = _repairRecord.StartDate.ToString("yyyy-MM-dd HH:mm");

//            txtIssueDescription.Text = _repairRecord.RepairDescription ?? "(Không có mô tả lỗi)";
//        }

//        private async void BtnStartRepair_Click(object sender, EventArgs e)
//        {
//            bool updated = await _ticketRepo.UpdateTicketStatusAsync(
//                _repairRecord.TicketId, _repairRecord.SerialNumber, "in_progress", _repairRecord.TechnicianId);

//            if (updated)
//            {
//                _repairRecord.Status = "in_progress";
//                MessageBox.Show("✅ Đã bắt đầu sửa chữa!", "Thông báo");
//            }
//            else
//            {
//                MessageBox.Show("❌ Không thể cập nhật trạng thái.", "Lỗi");
//            }
//        }

//        private async void BtnCompleteRepair_Click(object sender, EventArgs e)
//        {
//            if (string.IsNullOrWhiteSpace(txtPartsUsed.Text) || !decimal.TryParse(txtCost.Text, out decimal cost))
//            {
//                MessageBox.Show("⚠️ Vui lòng nhập đầy đủ linh kiện và chi phí hợp lệ!", "Cảnh báo");
//                return;
//            }

//            _repairRecord.PartsUsed = txtPartsUsed.Text;
//            _repairRecord.Cost = cost;
//            _repairRecord.RepairNotes = txtRepairNotes.Text;
//            _repairRecord.CompleteDate = DateTime.Now;
//            _repairRecord.Status = "completed";

//            bool saved = await _repairRepo.CreateRepairAsync(_repairRecord);
//            await _ticketRepo.  UpdateTicketStatusAsync(
//                _repairRecord.TicketId, _repairRecord.SerialNumber, "completed", _repairRecord.TechnicianId);

//            if (saved)
//            {
//                MessageBox.Show("✅ Hoàn thành sửa chữa và lưu thành công!", "Thành công");
//                this.DialogResult = DialogResult.OK;
//                this.Close();
//            }
//            else
//            {
//                MessageBox.Show("❌ Lưu thất bại!", "Lỗi");
//            }
//        }
//    }
//}

using System;
using System.Drawing;
using System.Windows.Forms;
using NoSQL_QL_BaoHanh.Auth;

namespace NoSQL_QL_BaoHanh.Forms
{
    public class RepairDetailForm : Form
    {
        private readonly RepairRepository _repairRepo;
        private readonly WarrantyTicketRepository _ticketRepo;
        private readonly RepairRecord _repairRecord;

        // UI Controls
        private Label lblTicketId, lblSerialNumber, lblStartDate;
        private TextBox txtRepairDescription, txtPartsUsed, txtCost;
        private Button btnStartRepair, btnCompleteRepair, btnClose;

        public RepairDetailForm(RepairRecord repairRecord)
        {
            _repairRecord = repairRecord;
            _repairRepo = new RepairRepository();
            _ticketRepo = new WarrantyTicketRepository();

            InitializeUI();
            LoadRepairInfo();
        }

        private void InitializeUI()
        {
            this.Text = "Chi tiết sửa chữa bảo hành";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 11);
            this.BackColor = Color.White;

            int xLabel = 20;
            int xValue = 180;
            int y = 20;
            int lineHeight = 30;

            AddLabel("Mã phiếu bảo hành:", xLabel, y);
            lblTicketId = AddLabel("", xValue, y);
            y += lineHeight;

            AddLabel("Mã sản phẩm:", xLabel, y);
            lblSerialNumber = AddLabel("", xValue, y);
            y += lineHeight;

            AddLabel("Ngày bắt đầu:", xLabel, y);
            lblStartDate = AddLabel("", xValue, y);
            y += lineHeight + 10;

            AddLabel("Mô tả sửa chữa:", xLabel, y);
            txtRepairDescription = AddTextBox(xLabel, y + 25, 430, 60, multiline: true);
            y += 90;

            AddLabel("Linh kiện sử dụng:", xLabel, y);
            txtPartsUsed = AddTextBox(xLabel, y + 25, 430, 30);
            y += 60;

            AddLabel("Chi phí (VND):", xLabel, y);
            txtCost = AddTextBox(xLabel, y + 25, 200, 30);
            y += 70;

            btnStartRepair = AddButton("🛠 Bắt đầu sửa", xLabel, y, BtnStartRepair_Click);
            btnCompleteRepair = AddButton("✅ Hoàn thành", xLabel + 160, y, BtnCompleteRepair_Click);
            btnClose = AddButton("✖ Đóng", xLabel + 320, y, (s, e) => this.Close());
        }

        private Label AddLabel(string text, int left, int top)
        {
            var lbl = new Label
            {
                Text = text,
                Left = left,
                Top = top,
                AutoSize = true
            };
            this.Controls.Add(lbl);
            return lbl;
        }

        private TextBox AddTextBox(int left, int top, int width, int height, bool multiline = false)
        {
            var txt = new TextBox
            {
                Left = left,
                Top = top,
                Width = width,
                Height = height,
                Multiline = multiline,
                ScrollBars = multiline ? ScrollBars.Vertical : ScrollBars.None
            };
            this.Controls.Add(txt);
            return txt;
        }

        private Button AddButton(string text, int left, int top, EventHandler onClick)
        {
            var btn = new Button
            {
                Text = text,
                Left = left,
                Top = top,
                Width = 140,
                Height = 40,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btn.Click += onClick;
            this.Controls.Add(btn);
            return btn;
        }

        private void LoadRepairInfo()
        {
            lblTicketId.Text = _repairRecord.TicketId;
            lblSerialNumber.Text = _repairRecord.SerialNumber;
            lblStartDate.Text = _repairRecord.StartDate.ToString("yyyy-MM-dd HH:mm");
        }

        private async void BtnStartRepair_Click(object sender, EventArgs e)
        {
            bool updated = await _ticketRepo.UpdateTicketStatusAsync(
                _repairRecord.TicketId, _repairRecord.SerialNumber, "in_progress", _repairRecord.TechnicianId);

            if (updated)
            {
                _repairRecord.Status = "in_progress";
                MessageBox.Show("✅ Đã bắt đầu sửa chữa!", "Thông báo");
            }
            else
            {
                MessageBox.Show("❌ Lỗi cập nhật trạng thái!", "Lỗi");
            }
        }

        private async void BtnCompleteRepair_Click(object sender, EventArgs e)
        {
            // Kiểm tra hợp lệ
            if (string.IsNullOrWhiteSpace(txtRepairDescription.Text) ||
                string.IsNullOrWhiteSpace(txtPartsUsed.Text) ||
                !decimal.TryParse(txtCost.Text, out decimal cost))
            {
                MessageBox.Show("⚠️ Vui lòng nhập đầy đủ mô tả, linh kiện và chi phí hợp lệ!", "Cảnh báo");
                return;
            }

            // Gán dữ liệu
            _repairRecord.RepairDescription = txtRepairDescription.Text;
            _repairRecord.PartsUsed = txtPartsUsed.Text;
            _repairRecord.Cost = cost;
            _repairRecord.CompleteDate = DateTime.Now;
            _repairRecord.Status = "completed";

            // Lưu vào Cassandra
            bool saved = await _repairRepo.CreateRepairAsync(_repairRecord);
            await _ticketRepo.UpdateTicketStatusAsync(
                _repairRecord.TicketId, _repairRecord.SerialNumber, "completed", _repairRecord.TechnicianId);

            if (saved)
            {
                MessageBox.Show("✅ Lưu thành công và hoàn tất sửa chữa!", "Thành công");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("❌ Lỗi khi lưu dữ liệu!", "Lỗi");
            }
        }
    }
}
