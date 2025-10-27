//using NoSQL_QL_BaoHanh.Auth;
//using System;
//using System.Drawing;
//using System.Windows.Forms;

//namespace NoSQL_QL_BaoHanh.Forms
//{
//    public class TicketManagementForm : Form
//    {
//        private ComboBox cboStatusFilter;
//        private Button btnFilter;
//        private DataGridView dgvTickets;
//        private Button btnAssignTech;
//        private Button btnUpdateStatus;
//        private Button btnViewDetail;
//        private Label lblTitle;
//        private readonly WarrantyTicketRepository _ticketRepo = new WarrantyTicketRepository();

//        public TicketManagementForm()
//        {
//            InitializeComponent();
//        }

//        private void InitializeComponent()
//        {
//            // Form Settings
//            this.Text = "Quản lý phiếu yêu cầu bảo hành";
//            this.Size = new Size(1000, 700);
//            this.StartPosition = FormStartPosition.CenterScreen;
//            this.FormBorderStyle = FormBorderStyle.FixedDialog;
//            this.MaximizeBox = false;
//            this.BackColor = Color.White;
//            this.Font = new Font("Segoe UI", 11);

//            // Title Label
//            lblTitle = new Label
//            {
//                Text = "📄 Danh sách phiếu yêu cầu bảo hành",
//                Font = new Font("Segoe UI", 16, FontStyle.Bold),
//                ForeColor = Color.FromArgb(0, 120, 215),
//                AutoSize = true,
//                Location = new Point(20, 20)
//            };

//            // Status Filter ComboBox
//            cboStatusFilter = new ComboBox
//            {
//                Location = new Point(20, 70),
//                Size = new Size(200, 30),
//                DropDownStyle = ComboBoxStyle.DropDownList
//            };
//            cboStatusFilter.Items.AddRange(new object[]
//            {
//                "Tất cả",
//                "pending",
//                "assigned",
//                "in_progress",
//                "completed"
//            });
//            cboStatusFilter.SelectedIndex = 0;

//            // Filter Button
//            btnFilter = new Button
//            {
//                Text = "Lọc",
//                Location = new Point(230, 70),
//                Size = new Size(80, 30),
//                BackColor = Color.FromArgb(0, 160, 170),
//                ForeColor = Color.White,
//                FlatStyle = FlatStyle.Flat
//            };
//            btnFilter.FlatAppearance.BorderSize = 0;

//            // DataGridView
//            dgvTickets = new DataGridView
//            {
//                Location = new Point(20, 120),
//                Size = new Size(940, 450),
//                ReadOnly = true,
//                AllowUserToAddRows = false,
//                BackgroundColor = Color.White,
//                BorderStyle = BorderStyle.FixedSingle,
//                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
//            };

//            dgvTickets.Columns.Add("TicketId", "Mã Phiếu");
//            dgvTickets.Columns.Add("SerialNumber", "Serial");
//            dgvTickets.Columns.Add("CustomerId", "Khách hàng");
//            dgvTickets.Columns.Add("TechnicianId", "Kỹ thuật viên");
//            dgvTickets.Columns.Add("Status", "Trạng thái");
//            dgvTickets.Columns.Add("CreatedAt", "Ngày tạo");

//            // Assign Technician Button
//            btnAssignTech = new Button
//            {
//                Text = "👨‍🔧 Phân công kỹ thuật viên",
//                Location = new Point(20, 600),
//                Size = new Size(250, 40),
//                BackColor = Color.FromArgb(0, 120, 215),
//                ForeColor = Color.White,
//                FlatStyle = FlatStyle.Flat
//            };
//            btnAssignTech.FlatAppearance.BorderSize = 0;

//            // Update Status Button
//            btnUpdateStatus = new Button
//            {
//                Text = "🔄 Cập nhật trạng thái",
//                Location = new Point(290, 600),
//                Size = new Size(200, 40),
//                BackColor = Color.FromArgb(0, 160, 170),
//                ForeColor = Color.White,
//                FlatStyle = FlatStyle.Flat
//            };
//            btnUpdateStatus.FlatAppearance.BorderSize = 0;

//            // View Detail Button
//            btnViewDetail = new Button
//            {
//                Text = "🔍 Xem chi tiết",
//                Location = new Point(510, 600),
//                Size = new Size(150, 40),
//                BackColor = Color.Gray,
//                ForeColor = Color.White,
//                FlatStyle = FlatStyle.Flat
//            };
//            btnViewDetail.FlatAppearance.BorderSize = 0;

//            // Add Controls
//            this.Controls.Add(lblTitle);
//            this.Controls.Add(cboStatusFilter);
//            this.Controls.Add(btnFilter);
//            this.Controls.Add(dgvTickets);
//            this.Controls.Add(btnAssignTech);
//            this.Controls.Add(btnUpdateStatus);
//            this.Controls.Add(btnViewDetail);
//        }
//        private async void LoadTickets(string status = "all")
//        {
//            dgvTickets.Rows.Clear();
//            var tickets = await _ticketRepo.GetTicketsByStatusAsync(status);

//            foreach (var ticket in tickets)
//            {
//                dgvTickets.Rows.Add(
//                    ticket.TicketId,
//                    ticket.SerialNumber,
//                    ticket.CustomerId,
//                    ticket.TechnicianId ?? "Chưa phân công",
//                    ticket.Status,
//                    ticket.CreatedAt.ToString("yyyy-MM-dd HH:mm")
//                );
//            }
//        }
//        protected override void OnLoad(EventArgs e)
//        {
//            base.OnLoad(e);
//            LoadTickets("all");
//        }
//        private void BtnFilter_Click(object sender, EventArgs e)
//        {
//            string status = cboStatusFilter.SelectedItem.ToString();
//            if (status == "Tất cả") status = "all";
//            LoadTickets(status);
//        }

//    }
//}

using Microsoft.VisualBasic.ApplicationServices;
using NoSQL_QL_BaoHanh.Auth;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NoSQL_QL_BaoHanh.Forms
{
    public class TicketManagementForm : Form
    {
        private ComboBox cboStatusFilter;
        private Button btnFilter;
        private DataGridView dgvTickets;
        private Button btnAssignTech;
        private Button btnUpdateStatus;
        private Button btnViewDetail;
        private Label lblTitle;
        private UserRecord _currentUser;
        // Repo & state
        private readonly WarrantyTicketRepository _ticketRepo = new WarrantyTicketRepository();
        private List<WarrantyTicketRecord> _currentTickets = new List<WarrantyTicketRecord>();

        public TicketManagementForm(UserRecord user)
        {
            _currentUser = user;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form Settings
            this.Text = "Quản lý phiếu yêu cầu bảo hành";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 11);

            // Title
            lblTitle = new Label
            {
                Text = "📄 Danh sách phiếu yêu cầu bảo hành",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            // Filter
            cboStatusFilter = new ComboBox
            {
                Location = new Point(20, 70),
                Size = new Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboStatusFilter.Items.AddRange(new object[]
            {
                "Tất cả",
                "pending",
                "assigned",
                "in_progress",
                "completed"
            });
            cboStatusFilter.SelectedIndex = 0;

            btnFilter = new Button
            {
                Text = "Lọc",
                Location = new Point(230, 70),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(0, 160, 170),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnFilter.FlatAppearance.BorderSize = 0;
            btnFilter.Click += BtnFilter_Click;

            // Grid
            dgvTickets = new DataGridView
            {
                Location = new Point(20, 120),
                Size = new Size(940, 450),
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            dgvTickets.Columns.Add("TicketId", "Mã Phiếu");
            dgvTickets.Columns.Add("SerialNumber", "Serial");
            dgvTickets.Columns.Add("CustomerId", "Khách hàng");
            dgvTickets.Columns.Add("TechnicianId", "Kỹ thuật viên");
            dgvTickets.Columns.Add("Status", "Trạng thái");
            dgvTickets.Columns.Add("CreatedAt", "Ngày tạo");

            // Buttons
            btnAssignTech = new Button
            {
                Text = "👨‍🔧 Phân công kỹ thuật viên",
                Location = new Point(20, 600),
                Size = new Size(250, 40),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAssignTech.FlatAppearance.BorderSize = 0;
            btnAssignTech.Click += BtnAssignTech_Click;

            btnUpdateStatus = new Button
            {
                Text = "🔄 Cập nhật trạng thái",
                Location = new Point(290, 600),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(0, 160, 170),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnUpdateStatus.FlatAppearance.BorderSize = 0;
            btnUpdateStatus.Click += BtnUpdateStatus_Click;

            btnViewDetail = new Button
            {
                Text = "🔍 Xem chi tiết",
                Location = new Point(510, 600),
                Size = new Size(150, 40),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnViewDetail.FlatAppearance.BorderSize = 0;
            btnViewDetail.Click += BtnViewDetail_Click;

            // Add Controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(cboStatusFilter);
            this.Controls.Add(btnFilter);
            this.Controls.Add(dgvTickets);
            this.Controls.Add(btnAssignTech);
            this.Controls.Add(btnUpdateStatus);
            this.Controls.Add(btnViewDetail);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadTickets("all");
        }

        // === LOAD & FILTER ===
        private async void LoadTickets(string status)
        {
            try
            {
                dgvTickets.Rows.Clear();
                _currentTickets = await _ticketRepo.GetTicketsByStatusAsync(status);

                foreach (var ticket in _currentTickets)
                {
                    dgvTickets.Rows.Add(
                        ticket.TicketId,
                        ticket.SerialNumber,
                        ticket.CustomerId,
                        string.IsNullOrWhiteSpace(ticket.TechnicianId) ? "Chưa phân công" : ticket.TechnicianId,
                        ticket.Status,
                        ticket.CreatedAt.ToString("yyyy-MM-dd HH:mm")
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách phiếu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            string status = cboStatusFilter.SelectedItem?.ToString() ?? "all";
            if (status == "Tất cả") status = "all";
            LoadTickets(status);
        }

        // === ASSIGN TECHNICIAN ===
        private async void BtnAssignTech_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedTicket();
            if (selected == null) return;

            if (selected.Status != "pending")
            {
                MessageBox.Show("Chỉ có thể phân công cho phiếu đang ở trạng thái 'pending'.",
                    "Không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy danh sách kỹ thuật viên
            var technicians = await _ticketRepo.GetTechniciansAsync();
            var activeTechs = technicians
                .Where(t => string.Equals(t.Role, "tech", StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(t.Status, "active", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (activeTechs.Count == 0)
            {
                MessageBox.Show("Không có kỹ thuật viên 'active' nào.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Popup chọn kỹ thuật viên
            using (var form = new Form())
            {
                form.Text = "Chọn kỹ thuật viên";
                form.Size = new Size(420, 180);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.BackColor = Color.White;
                form.Font = new Font("Segoe UI", 11);

                var lbl = new Label { Text = "Kỹ thuật viên:", AutoSize = true, Location = new Point(20, 20) };
                var cboTech = new ComboBox
                {
                    DataSource = activeTechs,
                    DisplayMember = "FullName",
                    ValueMember = "Username",
                    Location = new Point(20, 50),
                    Size = new Size(360, 30),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                var btnOk = new Button
                {
                    Text = "Xác nhận",
                    Location = new Point(200, 95),
                    Size = new Size(90, 32),
                    DialogResult = DialogResult.OK,
                    BackColor = Color.FromArgb(0, 160, 170),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnOk.FlatAppearance.BorderSize = 0;

                var btnCancel = new Button
                {
                    Text = "Hủy",
                    Location = new Point(290, 95),
                    Size = new Size(90, 32),
                    DialogResult = DialogResult.Cancel,
                    BackColor = Color.Gray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnCancel.FlatAppearance.BorderSize = 0;

                form.Controls.Add(lbl);
                form.Controls.Add(cboTech);
                form.Controls.Add(btnOk);
                form.Controls.Add(btnCancel);

                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    var techId = cboTech.SelectedValue?.ToString();
                    if (string.IsNullOrWhiteSpace(techId))
                    {
                        MessageBox.Show("Vui lòng chọn kỹ thuật viên.", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var ok = await _ticketRepo.AssignTechnicianAsync(selected.TicketId, selected.SerialNumber, techId);
                    if (ok)
                    {
                        MessageBox.Show("Phân công thành công.", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RefreshWithCurrentFilter();
                    }
                    else
                    {
                        MessageBox.Show("Không thể phân công. Kiểm tra kết nối Cassandra.", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // === UPDATE STATUS (auto progress) ===
        //private async void BtnUpdateStatus_Click(object sender, EventArgs e)
        //{
        //    var selected = GetSelectedTicket();
        //    if (selected == null) return;

        //    string next = GetNextStatus(selected.Status, selected.TechnicianId);
        //    if (next == null)
        //    {
        //        MessageBox.Show("Không thể cập nhật trạng thái từ trạng thái hiện tại.",
        //            "Không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    // Xác nhận
        //    if (MessageBox.Show($"Chuyển trạng thái từ '{selected.Status}' → '{next}'?",
        //        "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        //        return;

        //    var ok = await _ticketRepo.UpdateTicketStatusAsync(
        //        selected.TicketId,
        //        selected.SerialNumber,
        //        next,
        //        _currentUser.Username // hoặc _currentUser.Username tuỳ bạn đặt biến
        //    );
        //    if (ok)
        //    {
        //        MessageBox.Show("Cập nhật trạng thái thành công.", "Thành công",
        //            MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        RefreshWithCurrentFilter();
        //    }
        //    else
        //    {
        //        MessageBox.Show("Không thể cập nhật trạng thái. Kiểm tra kết nối Cassandra.", "Lỗi",
        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        private async void BtnUpdateStatus_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedTicket();
            if (selected == null) return;

            string next = GetNextStatus(selected.Status, selected.TechnicianId);
            if (next == null)
            {
                MessageBox.Show("Không thể cập nhật trạng thái từ trạng thái hiện tại.",
                    "Không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Chuyển trạng thái từ '{selected.Status}' → '{next}'?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var ok = await _ticketRepo.UpdateTicketStatusAsync(
                selected.TicketId,
                selected.SerialNumber,
                next,
                selected.TechnicianId // ✅ lấy đúng kỹ thuật viên được phân công
            );

            if (ok)
            {
                MessageBox.Show("Cập nhật trạng thái thành công.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshWithCurrentFilter();
            }
            else
            {
                MessageBox.Show("Không thể cập nhật trạng thái. Kiểm tra kết nối Cassandra.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetNextStatus(string current, string technicianId)
        {
            current = (current ?? "").Trim().ToLowerInvariant();

            switch (current)
            {
                case "pending":
                    // cần phân công trước khi cập nhật
                    return null; // dùng nút Phân công để chuyển -> assigned

                case "assigned":
                    // phải có technician
                    if (string.IsNullOrWhiteSpace(technicianId))
                        return null;
                    return "in_progress";

                case "in_progress":
                    return "completed";

                case "completed":
                    return null;

                default:
                    return null;
            }
        }

        // === VIEW DETAIL ===
        private void BtnViewDetail_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedTicket();
            if (selected == null) return;

            using (var form = new Form())
            {
                form.Text = $"Chi tiết phiếu {selected.TicketId}";
                form.Size = new Size(560, 460);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.BackColor = Color.White;
                form.Font = new Font("Segoe UI", 11);

                var y = 20;
                var pad = 34;

                form.Controls.Add(MakeLabel($"Mã phiếu: {selected.TicketId}", 20, y)); y += pad;
                form.Controls.Add(MakeLabel($"Serial: {selected.SerialNumber}", 20, y)); y += pad;
                form.Controls.Add(MakeLabel($"Khách hàng: {selected.CustomerId}", 20, y)); y += pad;
                form.Controls.Add(MakeLabel($"Kỹ thuật viên: {(string.IsNullOrWhiteSpace(selected.TechnicianId) ? "Chưa phân công" : selected.TechnicianId)}", 20, y)); y += pad;
                form.Controls.Add(MakeLabel($"Trạng thái: {selected.Status}", 20, y)); y += pad;
                form.Controls.Add(MakeLabel($"Ngày tạo: {selected.CreatedAt:yyyy-MM-dd HH:mm}", 20, y)); y += pad;

                var lblIssue = new Label
                {
                    Text = "Mô tả lỗi:",
                    Location = new Point(20, y),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold)
                };
                y += 28;

                var txtIssue = new TextBox
                {
                    Text = selected.IssueDescription,
                    Location = new Point(20, y),
                    Size = new Size(500, 180),
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical
                };

                var btnClose = new Button
                {
                    Text = "Đóng",
                    Location = new Point(430, 370),
                    Size = new Size(90, 32),
                    BackColor = Color.Gray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    DialogResult = DialogResult.OK
                };
                btnClose.FlatAppearance.BorderSize = 0;

                form.Controls.Add(lblIssue);
                form.Controls.Add(txtIssue);
                form.Controls.Add(btnClose);

                form.ShowDialog(this);
            }
        }

        private Label MakeLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true
            };
        }

        // === Helpers ===
        private WarrantyTicketRecord GetSelectedTicket()
        {
            if (dgvTickets.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một phiếu.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            var row = dgvTickets.SelectedRows[0];
            var ticketId = row.Cells["TicketId"]?.Value?.ToString();
            var serial = row.Cells["SerialNumber"]?.Value?.ToString();

            var ticket = _currentTickets.FirstOrDefault(t =>
                string.Equals(t.TicketId, ticketId, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(t.SerialNumber, serial, StringComparison.OrdinalIgnoreCase));

            if (ticket == null)
            {
                MessageBox.Show("Không tìm thấy dữ liệu phiếu trong bộ nhớ tạm.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ticket;
        }

        private void RefreshWithCurrentFilter()
        {
            var status = cboStatusFilter.SelectedItem?.ToString() ?? "all";
            if (status == "Tất cả") status = "all";
            LoadTickets(status);
        }
    }
}

