using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using NoSQL_QL_BaoHanh.Auth;

namespace NoSQL_QL_BaoHanh.Forms
{
    public class RevenueStatisticsForm : Form
    {
        private readonly RepairRepository _repairRepo = new RepairRepository();
        private readonly UserRecord _currentUser;

        // Controls
        private ComboBox cboTimeRange, cboTechnician;
        private DateTimePicker dtFrom, dtTo;
        private Button btnFilter;
        private DataGridView dgvRepairs;
        private Label lblTotalRevenue, lblTitle;
        private Chart chartRevenue;

        private List<RepairRecord> _currentRepairs = new List<RepairRecord>();

        public RevenueStatisticsForm(UserRecord user)
        {
            _currentUser = user;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "📊 Thống kê doanh thu";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 11);

            // Title
            lblTitle = new Label()
            {
                Text = "📊 Thống kê doanh thu từ sửa chữa bảo hành",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                AutoSize = true,
                Location = new Point(20, 15)
            };

            // ComboBox Time Range
            cboTimeRange = new ComboBox()
            {
                Location = new Point(20, 60),
                Size = new Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboTimeRange.Items.AddRange(new object[] { "Hôm nay", "Tháng này", "Năm nay", "Tùy chọn" });
            cboTimeRange.SelectedIndexChanged += CboTimeRange_SelectedIndexChanged;
            cboTimeRange.SelectedIndex = 2; // mặc định: Năm nay

            // DateTimePicker From-To (ẩn mặc định)
            dtFrom = new DateTimePicker { Location = new Point(230, 60), Size = new Size(200, 30), Visible = false };
            dtTo = new DateTimePicker { Location = new Point(440, 60), Size = new Size(200, 30), Visible = false };

            // ComboBox Technician
            cboTechnician = new ComboBox()
            {
                Location = new Point(650, 60),
                Size = new Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Button Filter
            btnFilter = new Button()
            {
                Text = "Lọc dữ liệu",
                Location = new Point(860, 60),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnFilter.FlatAppearance.BorderSize = 0;
            btnFilter.Click += BtnFilter_Click;

            // Label total revenue
            lblTotalRevenue = new Label()
            {
                Text = "Tổng doanh thu: 0 VNĐ",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                AutoSize = true,
                Location = new Point(20, 100)
            };

            // DataGridView
            dgvRepairs = new DataGridView()
            {
                Location = new Point(20, 140),
                Size = new Size(1150, 250),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            dgvRepairs.Columns.Add("RepairId", "Mã sửa chữa");
            dgvRepairs.Columns.Add("TicketId", "Mã phiếu");
            dgvRepairs.Columns.Add("SerialNumber", "Serial");
            dgvRepairs.Columns.Add("TechnicianId", "Kỹ thuật viên");
            dgvRepairs.Columns.Add("Cost", "Chi phí (VNĐ)");
            dgvRepairs.Columns.Add("Status", "Trạng thái");
            dgvRepairs.Columns.Add("StartDate", "Ngày bắt đầu");
            dgvRepairs.Columns.Add("CompleteDate", "Ngày hoàn thành");

            // Chart
            chartRevenue = new Chart()
            {
                Location = new Point(20, 410),
                Size = new Size(1150, 300)
            };
            var chartArea = new ChartArea("MainChart");
            chartRevenue.ChartAreas.Add(chartArea);
            chartRevenue.Series.Add(new Series("Doanh thu")
            {
                ChartType = SeriesChartType.Column,
                XValueType = ChartValueType.String
            });

            // Add controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(cboTimeRange);
            this.Controls.Add(dtFrom);
            this.Controls.Add(dtTo);
            this.Controls.Add(cboTechnician);
            this.Controls.Add(btnFilter);
            this.Controls.Add(lblTotalRevenue);
            this.Controls.Add(dgvRepairs);
            this.Controls.Add(chartRevenue);

            this.Load += RevenueStatisticsForm_Load;
        }

        private async void RevenueStatisticsForm_Load(object sender, EventArgs e)
        {
            await LoadTechniciansAsync();
            await LoadDataAsync();
        }

        private void CboTimeRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isCustom = cboTimeRange.SelectedItem?.ToString() == "Tùy chọn";

            if (dtFrom != null && dtTo != null)
            {
                dtFrom.Visible = dtTo.Visible = isCustom;
            }
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            LoadDataAsync();
        }
        private async Task LoadTechniciansAsync()
        {
            try
            {
                // Lấy danh sách kỹ thuật viên từ bảng users_by_username
                var ticketRepo = new WarrantyTicketRepository();
                var technicians = await ticketRepo.GetTechniciansAsync();

                cboTechnician.Items.Clear();
                cboTechnician.Items.Add("Tất cả");
                foreach (var tech in technicians)
                {
                    cboTechnician.Items.Add($"{tech.Username} - {tech.FullName}");
                }
                cboTechnician.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải danh sách kỹ thuật viên: {ex.Message}");
            }
        }

        // ============================
        // 📌 LOAD & FILTER DATA
        // ============================
        private async Task LoadDataAsync()
        {
            try
            {
                // 1️⃣ Lấy tất cả dữ liệu từ bảng repairs_by_technician
                var allRepairs = await _repairRepo.GetAllRepairsAsync();// % = wildcard (bạn có thể tùy chỉnh)

                // 2️⃣ Lọc theo kỹ thuật viên
                string techFilter = cboTechnician.SelectedItem?.ToString();
                if (techFilter != "Tất cả" && techFilter != null)
                {
                    string username = techFilter.Split('-')[0].Trim();
                    allRepairs = allRepairs.Where(r => r.TechnicianId == username).ToList();
                }

                // 3️⃣ Lọc theo thời gian
                DateTime now = DateTime.Now;
                DateTime from = DateTime.MinValue;
                DateTime to = DateTime.MaxValue;

                switch (cboTimeRange.SelectedItem.ToString())
                {
                    case "Hôm nay":
                        from = now.Date;
                        to = now.Date.AddDays(1);
                        break;
                    case "Tháng này":
                        from = new DateTime(now.Year, now.Month, 1);
                        to = from.AddMonths(1);
                        break;
                    case "Năm nay":
                        from = new DateTime(now.Year, 1, 1);
                        to = from.AddYears(1);
                        break;
                    case "Tùy chọn":
                        from = dtFrom.Value.Date;
                        to = dtTo.Value.Date.AddDays(1);
                        break;
                }

                _currentRepairs = allRepairs
                    .Where(r => r.StartDate >= from && r.StartDate < to)
                    .ToList();

                // 4️⃣ Hiển thị DataGridView
                dgvRepairs.Rows.Clear();
                decimal totalRevenue = 0;

                foreach (var r in _currentRepairs)
                {
                    dgvRepairs.Rows.Add(
                        r.RepairId,
                        r.TicketId,
                        r.SerialNumber,
                        r.TechnicianId,
                        $"{r.Cost:N0} VNĐ",
                        r.Status,
                        r.StartDate.ToString("yyyy-MM-dd"),
                        r.CompleteDate?.ToString("yyyy-MM-dd") ?? "Chưa hoàn thành"
                    );

                    if (r.Status == "completed")
                        totalRevenue += r.Cost;
                }

                lblTotalRevenue.Text = $"Tổng doanh thu: {totalRevenue:N0} VNĐ";

                // 5️⃣ Cập nhật biểu đồ
                UpdateChart(totalRevenue);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}");
            }
        }

        // ============================
        // 📊 UPDATE CHART
        // ============================
        private void UpdateChart(decimal totalRevenue)
        {
            chartRevenue.Series["Doanh thu"].Points.Clear();

            // Nhóm theo tháng
            var grouped = _currentRepairs
                .Where(r => r.Status == "completed")
                .GroupBy(r => r.StartDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Revenue = g.Sum(x => x.Cost)
                })
                .OrderBy(x => x.Month);

            foreach (var item in grouped)
            {
                chartRevenue.Series["Doanh thu"].Points.AddXY($"Tháng {item.Month}", item.Revenue);
            }
        }
    }
}
