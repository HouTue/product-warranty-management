using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NoSQL_QL_BaoHanh.Auth;

namespace NoSQL_QL_BaoHanh.Forms
{
    public class CustomerManagementForm : Form
    {
        private Label lblTitle;
        private TextBox txtSearch;
        private ComboBox cboSearchType;
        private Button btnSearch, btnRefresh, btnAdd, btnEdit, btnDeactivate, btnViewProducts;
        private DataGridView dgvCustomers;

        private readonly CustomerRepository _customerRepo = new CustomerRepository();
        private List<CustomerRecord> _currentCustomers = new List<CustomerRecord>();

        public CustomerManagementForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form config
            this.Text = "Quản lý khách hàng";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 11);

            // Title
            lblTitle = new Label
            {
                Text = "👤 Quản lý khách hàng",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            // ComboBox chọn kiểu tìm kiếm
            cboSearchType = new ComboBox
            {
                Location = new Point(20, 70),
                Size = new Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboSearchType.Items.AddRange(new object[] { "Theo mã khách hàng", "Theo số điện thoại" });
            cboSearchType.SelectedIndex = 0;

            // TextBox tìm kiếm
            txtSearch = new TextBox
            {
                Location = new Point(230, 70),
                Size = new Size(300, 30),
                PlaceholderText = "Nhập từ khóa tìm kiếm..."
            };

            // Nút tìm kiếm
            btnSearch = new Button
            {
                Text = "🔍 Tìm",
                Location = new Point(540, 70),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(0, 160, 170),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += BtnSearch_Click;

            // Nút làm mới
            btnRefresh = new Button
            {
                Text = "⟳ Làm mới",
                Location = new Point(630, 70),
                Size = new Size(100, 30),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;

            // DataGridView
            dgvCustomers = new DataGridView
            {
                Location = new Point(20, 120),
                Size = new Size(940, 480),
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvCustomers.Columns.Add("CustomerId", "Mã khách hàng");
            dgvCustomers.Columns.Add("FullName", "Họ tên");
            dgvCustomers.Columns.Add("Phone", "Số điện thoại");
            dgvCustomers.Columns.Add("Email", "Email");
            dgvCustomers.Columns.Add("Address", "Địa chỉ");
            dgvCustomers.Columns.Add("Status", "Trạng thái");

            dgvCustomers.CellFormatting += DgvCustomers_CellFormatting;

            // Buttons (footer)
            btnAdd = new Button
            {
                Text = "➕ Thêm mới",
                Location = new Point(20, 620),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button
            {
                Text = "✏ Sửa thông tin",
                Location = new Point(190, 620),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(0, 160, 170),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += BtnEdit_Click;

            btnDeactivate = new Button
            {
                Text = "🚫 Vô hiệu hóa",
                Location = new Point(360, 620),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnDeactivate.FlatAppearance.BorderSize = 0;
            btnDeactivate.Click += BtnDeactivate_Click;


            this.Controls.Add(btnViewProducts);
            // Add Controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(cboSearchType);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnSearch);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(dgvCustomers);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDeactivate);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadAllCustomers();
        }

        private async Task LoadAllCustomers()
        {
            dgvCustomers.Rows.Clear();
            _currentCustomers = await _customerRepo.GetAllCustomersAsync();

            foreach (var c in _currentCustomers)
            {
                dgvCustomers.Rows.Add(c.CustomerId, c.FullName, c.Phone, c.Email, c.Address, c.Status);
            }
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                MessageBox.Show("Vui lòng nhập từ khóa tìm kiếm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dgvCustomers.Rows.Clear();
            CustomerRecord customer = null;

            if (cboSearchType.SelectedIndex == 0) // Theo mã
                customer = await _customerRepo.SearchByIdAsync(keyword);
            else // Theo điện thoại
                customer = await _customerRepo.SearchByPhoneAsync(keyword);

            if (customer != null)
            {
                dgvCustomers.Rows.Add(customer.CustomerId, customer.FullName, customer.Phone, customer.Email, customer.Address, customer.Status);
            }
            else
            {
                MessageBox.Show("Không tìm thấy khách hàng!", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            await LoadAllCustomers();
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new CustomerAddForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadAllCustomers();
                    MessageBox.Show("Khách hàng mới đã được thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để chỉnh sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string customerId = dgvCustomers.SelectedRows[0].Cells["CustomerId"].Value.ToString();
            var customer = await _customerRepo.SearchByIdAsync(customerId);

            if (customer == null)
            {
                MessageBox.Show("Không thể tìm thấy khách hàng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var form = new CustomerEditForm(customer))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadAllCustomers();
                    MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private async void BtnDeactivate_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một khách hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string customerId = dgvCustomers.SelectedRows[0].Cells["CustomerId"].Value.ToString();
            var confirm = MessageBox.Show($"Bạn có chắc muốn vô hiệu hóa khách hàng {customerId}?",
                                            "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                bool success = await _customerRepo.DeactivateCustomerAsync(customerId);
                if (success)
                {
                    await LoadAllCustomers();
                    MessageBox.Show("Khách hàng đã được vô hiệu hóa!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Không thể vô hiệu hóa khách hàng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void DgvCustomers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvCustomers.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                string status = e.Value.ToString().ToLower();
                if (status == "active")
                {
                    e.CellStyle.ForeColor = Color.Green;
                    e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
                }
                else if (status == "deactive")
                {
                    e.CellStyle.ForeColor = Color.Red;
                    e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
                }
            }
        }
      

    }
}
