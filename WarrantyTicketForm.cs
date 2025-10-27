using System;
using System.Drawing;
using System.Windows.Forms;
using NoSQL_QL_BaoHanh.Auth;

namespace NoSQL_QL_BaoHanh.Forms
{
    public class WarrantyTicketForm : Form
    {
        private readonly string _serialNumber;
        private readonly string _customerId;
        private readonly WarrantyTicketRepository _ticketRepo;

        private Label lblSerial;
        private Label lblCustomer;
        private TextBox txtIssue;
        private Button btnSubmit;
        private Button btnCancel;

        public WarrantyTicketForm(string serialNumber, string customerId)
        {
            _serialNumber = serialNumber;
            _customerId = customerId;
            _ticketRepo = new WarrantyTicketRepository();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Tạo Phiếu Yêu Cầu Bảo Hành";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 11);

            // Label Serial
            lblSerial = new Label
            {
                Text = $"Serial Number: {_serialNumber}",
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Label Customer
            lblCustomer = new Label
            {
                Text = $"Customer ID: {_customerId}",
                Location = new Point(20, 60),
                AutoSize = true
            };

            // TextBox Issue
            Label lblIssue = new Label
            {
                Text = "Mô tả lỗi / triệu chứng:",
                Location = new Point(20, 100),
                AutoSize = true
            };

            txtIssue = new TextBox
            {
                Location = new Point(20, 130),
                Size = new Size(440, 120),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // Buttons
            btnSubmit = new Button
            {
                Text = "✅ Xác nhận",
                Location = new Point(100, 280),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(0, 160, 170),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Click += BtnSubmit_Click;

            btnCancel = new Button
            {
                Text = "❌ Hủy",
                Location = new Point(260, 280),
                Size = new Size(120, 40),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();

            // Add Controls
            this.Controls.Add(lblSerial);
            this.Controls.Add(lblCustomer);
            this.Controls.Add(lblIssue);
            this.Controls.Add(txtIssue);
            this.Controls.Add(btnSubmit);
            this.Controls.Add(btnCancel);
        }

        private async void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIssue.Text))
            {
                MessageBox.Show("Vui lòng nhập mô tả lỗi!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Sinh mã phiếu tự động
            string newTicketId = await _ticketRepo.GenerateNewTicketIdAsync(_serialNumber);

            var ticket = new WarrantyTicketRecord
            {
                TicketId = newTicketId,
                SerialNumber = _serialNumber,
                CustomerId = _customerId,
                CreatedAt = DateTime.Now,
                IssueDescription = txtIssue.Text.Trim(),
                Status = "pending",
                TechnicianId = null // Chưa gán kỹ thuật viên
            };

            bool success = await _ticketRepo.CreateWarrantyTicketAsync(ticket);

            if (success)
            {
                MessageBox.Show($"Tạo phiếu bảo hành thành công!\nMã phiếu: {ticket.TicketId}",
                                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Không thể tạo phiếu! Vui lòng kiểm tra kết nối Cassandra.",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
