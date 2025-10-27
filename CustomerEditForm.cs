using System;
using System.Drawing;
using System.Windows.Forms;
using NoSQL_QL_BaoHanh.Auth;

namespace NoSQL_QL_BaoHanh.Forms
{
    public class CustomerEditForm : Form
    {
        private TextBox txtCustomerId, txtFullName, txtPhone, txtEmail, txtAddress;
        private ComboBox cboStatus;
        private Button btnSave, btnCancel;
        private Label lblTitle;
        private readonly CustomerRepository _customerRepo = new CustomerRepository();
        private readonly CustomerRecord _customer;

        public CustomerEditForm(CustomerRecord customer)
        {
            _customer = customer;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "Chỉnh sửa khách hàng";
            this.Size = new Size(500, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 11);

            lblTitle = new Label
            {
                Text = "✏ Chỉnh sửa khách hàng",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            Label lblCustomerId = new Label { Text = "Mã khách hàng:", Location = new Point(20, 80), AutoSize = true };
            txtCustomerId = new TextBox { Location = new Point(150, 78), Width = 300, ReadOnly = true };

            Label lblFullName = new Label { Text = "Họ tên:", Location = new Point(20, 120), AutoSize = true };
            txtFullName = new TextBox { Location = new Point(150, 118), Width = 300 };

            Label lblPhone = new Label { Text = "Số điện thoại:", Location = new Point(20, 160), AutoSize = true };
            txtPhone = new TextBox { Location = new Point(150, 158), Width = 300 };

            Label lblEmail = new Label { Text = "Email:", Location = new Point(20, 200), AutoSize = true };
            txtEmail = new TextBox { Location = new Point(150, 198), Width = 300 };

            Label lblAddress = new Label { Text = "Địa chỉ:", Location = new Point(20, 240), AutoSize = true };
            txtAddress = new TextBox
            {
                Location = new Point(150, 238),
                Width = 300,
                Height = 100,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            Label lblStatus = new Label { Text = "Trạng thái:", Location = new Point(20, 350), AutoSize = true };
            cboStatus = new ComboBox
            {
                Location = new Point(150, 348),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboStatus.Items.AddRange(new object[] { "active", "deactive" });

            btnSave = new Button
            {
                Text = "💾 Lưu",
                Location = new Point(150, 400),
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
                Location = new Point(280, 400),
                Size = new Size(120, 40),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[]
            {
                lblTitle, lblCustomerId, txtCustomerId,
                lblFullName, txtFullName,
                lblPhone, txtPhone,
                lblEmail, txtEmail,
                lblAddress, txtAddress,
                lblStatus, cboStatus,
                btnSave, btnCancel
            });

            LoadCustomerData();
        }

        private void LoadCustomerData()
        {
            txtCustomerId.Text = _customer.CustomerId;
            txtFullName.Text = _customer.FullName;
            txtPhone.Text = _customer.Phone;
            txtEmail.Text = _customer.Email;
            txtAddress.Text = _customer.Address;
            cboStatus.SelectedItem = _customer.Status;
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Tên và số điện thoại là bắt buộc!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _customer.FullName = txtFullName.Text;
            _customer.Phone = txtPhone.Text;
            _customer.Email = txtEmail.Text;
            _customer.Address = txtAddress.Text;
            _customer.Status = cboStatus.SelectedItem.ToString();

            bool success = await _customerRepo.UpdateCustomerAsync(_customer);
            if (success)
            {
                MessageBox.Show("Cập nhật thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Không thể cập nhật khách hàng!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
