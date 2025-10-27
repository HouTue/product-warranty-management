using System;
using System.Drawing;
using System.Windows.Forms;
using NoSQL_QL_BaoHanh.Auth;

namespace NoSQL_QL_BaoHanh.Forms
{
    public class CustomerAddForm : Form
    {
        private TextBox txtCustomerId, txtFullName, txtPhone, txtEmail, txtAddress;
        private Button btnSave, btnCancel;
        private Label lblTitle;
        private readonly CustomerRepository _customerRepo = new CustomerRepository();

        public CustomerAddForm()
        {
            InitializeComponent();
        }

        private async void InitializeComponent()
        {
            // Form settings
            this.Text = "Thêm khách hàng mới";
            this.Size = new Size(500, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 11);

            lblTitle = new Label
            {
                Text = "➕ Thêm khách hàng",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            Label lblCustomerId = new Label { Text = "Mã khách hàng:", Location = new Point(20, 70), AutoSize = true };
            txtCustomerId = new TextBox { Location = new Point(150, 68), Width = 300, ReadOnly = true };

            Label lblFullName = new Label { Text = "Họ tên:", Location = new Point(20, 110), AutoSize = true };
            txtFullName = new TextBox { Location = new Point(150, 108), Width = 300 };

            Label lblPhone = new Label { Text = "Số điện thoại:", Location = new Point(20, 150), AutoSize = true };
            txtPhone = new TextBox { Location = new Point(150, 148), Width = 300 };

            Label lblEmail = new Label { Text = "Email:", Location = new Point(20, 190), AutoSize = true };
            txtEmail = new TextBox { Location = new Point(150, 188), Width = 300 };

            Label lblAddress = new Label { Text = "Địa chỉ:", Location = new Point(20, 230), AutoSize = true };
            txtAddress = new TextBox
            {
                Location = new Point(150, 228),
                Width = 300,
                Height = 80,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            btnSave = new Button
            {
                Text = "💾 Lưu",
                Location = new Point(150, 330),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "✖ Hủy",
                Location = new Point(280, 330),
                Size = new Size(120, 40),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitle);
            this.Controls.AddRange(new Control[]
            {
                lblCustomerId, txtCustomerId,
                lblFullName, txtFullName,
                lblPhone, txtPhone,
                lblEmail, txtEmail,
                lblAddress, txtAddress,
                btnSave, btnCancel
            });

            // Load Customer ID tự sinh
            string newCustomerId = await _customerRepo.GenerateCustomerIdAsync();
            txtCustomerId.Text = newCustomerId;
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Tên và Số điện thoại là bắt buộc!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var customer = new CustomerRecord
            {
                CustomerId = txtCustomerId.Text,
                FullName = txtFullName.Text,
                Phone = txtPhone.Text,
                Email = txtEmail.Text,
                Address = txtAddress.Text,
                Status = "active"
            };

            bool success = await _customerRepo.AddCustomerAsync(customer);
            if (success)
            {
                MessageBox.Show("Thêm khách hàng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Lỗi! Không thể thêm khách hàng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
