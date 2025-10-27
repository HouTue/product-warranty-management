using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using NoSQL_QL_BaoHanh.Auth;
using NoSQL_QL_BaoHanh.CassandraServices;

namespace NoSQL_QL_BaoHanh.Forms
{
    public class UserManagementForm : Form
    {
        private DataGridView dgvUsers;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;
        private TextBox txtSearch;
        private Label lblSearch;
        private UserRepository userRepo;

        public UserManagementForm()
        {
            userRepo = new UserRepository();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý người dùng";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(900, 600);
            this.Font = new Font("Segoe UI", 11);
            this.BackColor = Color.White;

            // Tìm kiếm
            lblSearch = new Label
            {
                Text = "Tìm kiếm:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            txtSearch = new TextBox
            {
                Location = new Point(100, 15),
                Width = 400,
                PlaceholderText = "Nhập username, full name hoặc role..."
            };
            txtSearch.TextChanged += async (s, e) => await LoadUsers();

            // DataGridView chiếm 65% chiều rộng form
            dgvUsers = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(550, 450),
                AutoGenerateColumns = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Username", DataPropertyName = "Username", Width = 120 });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Full Name", DataPropertyName = "FullName", Width = 180 });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Role", DataPropertyName = "Role", Width = 100 });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status", Width = 100 });

            // Panel chứa các nút bên phải
            Panel panelButtons = new Panel
            {
                Location = new Point(600, 60),
                Size = new Size(250, 450),
                BackColor = Color.WhiteSmoke
            };

            btnAdd = CreateButton("➕ Thêm user");
            btnAdd.Top = 10;
            btnAdd.Click += BtnAdd_Click;

            btnEdit = CreateButton("✏ Sửa");
            btnEdit.Top = 80;
            btnEdit.Click += BtnEdit_Click;

            btnDelete = CreateButton("🗑 Xóa");
            btnDelete.Top = 150;
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = CreateButton("🔄 Làm mới");
            btnRefresh.Top = 220;
            btnRefresh.Click += async (s, e) => await LoadUsers(true);

            panelButtons.Controls.Add(btnAdd);
            panelButtons.Controls.Add(btnEdit);
            panelButtons.Controls.Add(btnDelete);
            panelButtons.Controls.Add(btnRefresh);

            this.Controls.Add(lblSearch);
            this.Controls.Add(txtSearch);
            this.Controls.Add(dgvUsers);
            this.Controls.Add(panelButtons);

            this.Load += async (s, e) => await LoadUsers();
        }

        private Button CreateButton(string text)
        {
            return new Button
            {
                Text = text,
                Size = new Size(200, 50),
                BackColor = Color.FromArgb(0, 160, 170), // cyan dịu mắt
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Left = 20
            };
        }


        private async Task LoadUsers(bool clearSearch = false)
        {
            if (clearSearch) txtSearch.Text = "";
            var users = string.IsNullOrWhiteSpace(txtSearch.Text)
                ? await userRepo.GetAllUsersAsync()
                : await userRepo.SearchUsersAsync(txtSearch.Text.Trim());
            dgvUsers.DataSource = users;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var popup = new UserFormPopup(FormMode.Add);
            popup.ShowDialog();
            _ = LoadUsers(true);
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chọn user để sửa!");
                return;
            }

            var user = dgvUsers.SelectedRows[0].DataBoundItem as UserRecord;
            var popup = new UserFormPopup(FormMode.Edit, user);
            popup.ShowDialog();
            _ = LoadUsers();
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chọn user để xóa!");
                return;
            }

            var user = dgvUsers.SelectedRows[0].DataBoundItem as UserRecord;
            if (MessageBox.Show($"Xóa user {user.Username}?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                await userRepo.DeleteUserAsync(user.Username);
                MessageBox.Show("Đã xóa!");
                await LoadUsers();
            }
        }
    }

    public enum FormMode
    {
        Add,
        Edit
    }
}
