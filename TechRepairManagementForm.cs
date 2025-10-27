using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using NoSQL_QL_BaoHanh.Auth;

namespace NoSQL_QL_BaoHanh.Forms
{
    public class TechRepairManagementForm : Form
    {
        private readonly string _technicianId;
        private readonly WarrantyTicketRepository _ticketRepo = new WarrantyTicketRepository();
        //private readonly RepairRepository _repairRepo = new RepairRepository();


        private Label lblTitle;
        private FlowLayoutPanel statusPanel;
        private Button btnAll, btnPending, btnInProgress, btnCompleted;
        private DataGridView dgvRepairs;
        private Button btnViewDetail, btnDoRepair, btnClose;

        public TechRepairManagementForm(string technicianId)
        {
            _technicianId = technicianId;
            InitializeComponent();
        }

        private async void InitializeComponent()
        {
            this.Text = "Quản lý phiếu sửa chữa bảo hành";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 11);

            // Tiêu đề
            lblTitle = new Label()
            {
                Text = $"📋 Danh sách phiếu sửa chữa của bạn ({_technicianId})",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            // Panel lọc trạng thái
            statusPanel = new FlowLayoutPanel()
            {
                Location = new Point(20, 70),
                Size = new Size(940, 50),
                FlowDirection = FlowDirection.LeftToRight
            };

            btnAll = CreateStatusButton("Tất cả", "all");
            btnPending = CreateStatusButton("Chờ xử lý", "assigned");
            btnInProgress = CreateStatusButton("Đang sửa", "in_progress");
            btnCompleted = CreateStatusButton("Hoàn thành", "completed");




            statusPanel.Controls.AddRange(new Control[] { btnAll, btnPending, btnInProgress, btnCompleted });

            // Grid hiển thị danh sách
            dgvRepairs = new DataGridView()
            {
                Location = new Point(20, 130),
                Size = new Size(940, 400),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvRepairs.Columns.Add("TicketId", "Mã phiếu");
            dgvRepairs.Columns.Add("SerialNumber", "Sản phẩm");
            dgvRepairs.Columns.Add("CustomerId", "Mã khách hàng");
            dgvRepairs.Columns.Add("StartDate", "Ngày bắt đầu");
            dgvRepairs.Columns.Add("Status", "Trạng thái");

            // Các nút hành động
            btnViewDetail = CreateActionButton("🔍 Xem chi tiết", BtnViewDetail_Click);
            btnDoRepair = CreateActionButton("🛠 Thực hiện sửa chữa", BtnDoRepair_Click);
            btnClose = CreateActionButton("❌ Đóng", (s, e) => this.Close());

            btnViewDetail.Location = new Point(20, 540);
            btnDoRepair.Location = new Point(200, 540);
            btnClose.Location = new Point(380, 540);

            this.Controls.Add(lblTitle);
            this.Controls.Add(statusPanel);
            this.Controls.Add(dgvRepairs);
            this.Controls.Add(btnViewDetail);
            this.Controls.Add(btnDoRepair);
            this.Controls.Add(btnClose);

            await LoadRepairs(); // load tất cả khi mở
        }

        private Button CreateStatusButton(string text, string statusValue)
        {
            //var btn = new Button()
            //{
            //    Text = text,
            //    Tag = statusValue,
            //    Size = new Size(150, 40),
            //    BackColor = Color.FromArgb(0, 120, 215),
            //    ForeColor = Color.White,
            //    FlatStyle = FlatStyle.Flat
            //};
            //btn.FlatAppearance.BorderSize = 0;
            //btn.Margin = new Padding(5);
            //btn.Click += async (s, e) =>
            //{
            //    await LoadRepairs(statusValue);
            //};
            //return btn;
            var btn = new Button()
            {
                Text = text,
                Tag = statusValue,
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Margin = new Padding(5);

            //btn.Click += async (s, e) =>
            //{
            //    string status = statusValue;

            //    // Map trạng thái hiển thị → trạng thái thực trong DB
            //    if (status == "pending") status = "assigned"; // Khi kỹ thuật viên nhận
            //    await LoadRepairs(status);
            //};
            btn.Click += async (s, e) =>
            {
                await LoadRepairs(statusValue); // Gọi hàm lọc theo trạng thái
            };
            return btn;
        }

        private Button CreateActionButton(string text, EventHandler onClick)
        {
            var btn = new Button()
            {
                Text = text,
                Size = new Size(160, 40),
                BackColor = Color.FromArgb(0, 150, 136),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btn.FlatAppearance.BorderSize = 0;
            if (onClick != null) btn.Click += onClick;
            return btn;
        }

        //private async Task LoadRepairs(string statusFilter = "all")
        //{
        //    dgvRepairs.Rows.Clear();
        //    var repairs = await _repairRepo.GetRepairsByTechnicianAsync(_technicianId);

        //    foreach (var r in repairs)
        //    {
        //        if (statusFilter != "all" && r.Status != statusFilter) continue;

        //        dgvRepairs.Rows.Add(
        //            r.RepairId,
        //            r.TicketId,
        //            r.SerialNumber,
        //            r.StartDate.ToString("yyyy-MM-dd"),
        //            ConvertStatusToVietnamese(r.Status)
        //        );
        //    }
        //}
        private async Task LoadRepairs(string statusFilter = "all")
        {
            //dgvRepairs.Rows.Clear();
            //var tickets = await _ticketRepo.GetTicketsByTechnicianAsync(_technicianId, statusFilter);

            //foreach (var t in tickets)
            //{
            //    dgvRepairs.Rows.Add(
            //        t.TicketId,
            //        t.SerialNumber,
            //        t.CustomerId,
            //        t.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
            //        ConvertStatusToVietnamese(t.Status)
            //    );
            //}
            dgvRepairs.Rows.Clear();

            // Lấy danh sách theo trạng thái
            var tickets = await _ticketRepo.GetTicketsByTechnicianAsync(_technicianId, statusFilter);

            foreach (var t in tickets)
            {
                dgvRepairs.Rows.Add(
                    t.TicketId,
                    t.SerialNumber,
                    t.CustomerId,
                    t.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                    ConvertStatusToVietnamese(t.Status)
                );
            }

            HighlightSelectedFilter(statusFilter);
        }
        private void HighlightSelectedFilter(string selectedStatus)
        {
            foreach (Control c in statusPanel.Controls)
            {
                if (c is Button btn)
                {
                    if ((string)btn.Tag == selectedStatus || (selectedStatus == "all" && (string)btn.Tag == "all"))
                        btn.BackColor = Color.FromArgb(0, 180, 120); // màu đang chọn
                    else
                        btn.BackColor = Color.FromArgb(0, 120, 215); // màu mặc định
                }
            }
        }

        private string ConvertStatusToVietnamese(string status)
        {
            return status switch
            { 
                "in_progress" => "Đang sửa",
                "completed" => "Hoàn thành",
                "assigned" => "Đã giao",
                _ => status
            };
        }

        private async void BtnViewDetail_Click(object sender, EventArgs e)
        {
            if (dgvRepairs.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một phiếu để xem chi tiết.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy ticket_id từ dòng đã chọn
            string ticketId = dgvRepairs.SelectedRows[0].Cells["TicketId"].Value.ToString();

            // Lấy danh sách phiếu theo technician
            var tickets = await _ticketRepo.GetTicketsByTechnicianAsync(_technicianId);
            var selectedTicket = tickets.FirstOrDefault(t => t.TicketId == ticketId);

            if (selectedTicket == null)
            {
                MessageBox.Show("Không tìm thấy thông tin phiếu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Hiển thị thông tin phiếu (read-only)
            string detail = $"📄 THÔNG TIN PHIẾU BẢO HÀNH\n" +
                            $"----------------------------------\n" +
                            $"🔹 Mã phiếu: {selectedTicket.TicketId}\n" +
                            $"🔹 Mã sản phẩm: {selectedTicket.SerialNumber}\n" +
                            $"🔹 Mã khách hàng: {selectedTicket.CustomerId}\n" +
                            $"🔹 Ngày tạo: {selectedTicket.CreatedAt:yyyy-MM-dd HH:mm}\n" +
                            $"🔹 Mô tả lỗi: {selectedTicket.IssueDescription}\n" +
                            $"🔹 Trạng thái: {ConvertStatusToVietnamese(selectedTicket.Status)}\n" +
                            $"🔹 Kỹ thuật viên: {selectedTicket.TechnicianId ?? "Chưa phân công"}";

            MessageBox.Show(detail, "Chi tiết phiếu bảo hành", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private async void BtnDoRepair_Click(object sender, EventArgs e)
        {
            if (dgvRepairs.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một phiếu để thực hiện.", "Thông báo");
                return;
            }

            string ticketId = dgvRepairs.SelectedRows[0].Cells["TicketId"].Value.ToString();
            var tickets = await _ticketRepo.GetTicketsByTechnicianAsync(_technicianId);
            var selectedTicket = tickets.FirstOrDefault(t => t.TicketId == ticketId);

            if (selectedTicket == null)
            {
                MessageBox.Show("Không tìm thấy dữ liệu phiếu!", "Lỗi");
                return;
            }

            // Tạo bản ghi REPAIR ban đầu
            var repairRecord = new RepairRecord
            {
                TechnicianId = _technicianId,
                RepairId = await new RepairRepository().GenerateRepairIdAsync(),
                TicketId = selectedTicket.TicketId,
                SerialNumber = selectedTicket.SerialNumber,
                StartDate = DateTime.Now,
                Status = "in_progress"
            };

            // Mở form nhập thông tin sửa chữa
            var form = new RepairDetailForm(repairRecord);
            form.ShowDialog();

            // Reload lại danh sách
            await LoadRepairs();
        }


    }
}
