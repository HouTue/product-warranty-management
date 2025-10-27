using System;
using System.Drawing;
using System.Windows.Forms;
using NoSQL_QL_BaoHanh.Auth;

namespace NoSQL_QL_BaoHanh.Forms
{
    public class UserFormPopup : Form
    {
        private TextBox txtUsername, txtPassword, txtFullName;
        private ComboBox cmbRole, cmbStatus;
        private Button btnSave, btnCancel, btnTogglePassword;
        private bool showPassword = false;
        private readonly UserRepository userRepo;
        private readonly FormMode mode;
        private readonly UserRecord existingUser;

        public UserFormPopup(FormMode mode, UserRecord user = null)
        {
            this.mode = mode;
            this.existingUser = user;
            userRepo = new UserRepository();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = mode == FormMode.Add ? "Thêm người dùng" : "Sửa người dùng";
            this.Size = new Size(400, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblUsername = new Label { Text = "Username:", Location = new Point(30, 30), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(150, 25), Width = 200 };

            Label lblPassword = new Label { Text = "Password:", Location = new Point(30, 80), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(150, 75), Width = 165, UseSystemPasswordChar = true };
            btnTogglePassword = new Button
            {
                Text = "👁",
                Location = new Point(320, 75),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat
            };
            btnTogglePassword.Click += TogglePasswordVisibility;

            Label lblFullName = new Label { Text = "Full Name:", Location = new Point(30, 130), AutoSize = true };
            txtFullName = new TextBox { Location = new Point(150, 125), Width = 200 };

            Label lblRole = new Label { Text = "Role:", Location = new Point(30, 180), AutoSize = true };
            cmbRole = new ComboBox { Location = new Point(150, 175), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new string[] { "admin", "staff", "tech" });

            Label lblStatus = new Label { Text = "Status:", Location = new Point(30, 230), AutoSize = true };
            cmbStatus = new ComboBox { Location = new Point(150, 225), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus.Items.AddRange(new string[] { "active", "deactive" });

            btnSave = new Button
            {
                Text = "Lưu",
                Location = new Point(80, 300),
                Size = new Size(100, 40),
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Hủy",
                Location = new Point(220, 300),
                Size = new Size(100, 40),
                BackColor = Color.IndianRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.Click += (s, e) => this.Close();

            Controls.Add(lblUsername);
            Controls.Add(txtUsername);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(btnTogglePassword);
            Controls.Add(lblFullName);
            Controls.Add(txtFullName);
            Controls.Add(lblRole);
            Controls.Add(cmbRole);
            Controls.Add(lblStatus);
            Controls.Add(cmbStatus);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
        }

        private void TogglePasswordVisibility(object sender, EventArgs e)
        {
            showPassword = !showPassword;
            txtPassword.UseSystemPasswordChar = !showPassword;
            btnTogglePassword.Text = showPassword ? "🙈" : "👁";
        }

        private void LoadData()
        {
            if (mode == FormMode.Edit && existingUser != null)
            {
                txtUsername.Text = existingUser.Username;
                txtUsername.Enabled = false;
                txtPassword.Text = existingUser.Password;
                txtFullName.Text = existingUser.FullName;
                cmbRole.SelectedItem = existingUser.Role;
                cmbStatus.SelectedItem = existingUser.Status;
            }
            else
            {
                cmbRole.SelectedIndex = 0;
                cmbStatus.SelectedIndex = 0;
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            var user = new UserRecord
            {
                Username = txtUsername.Text.Trim(),
                Password = txtPassword.Text.Trim(),
                FullName = txtFullName.Text.Trim(),
                Role = cmbRole.SelectedItem.ToString(),
                Status = cmbStatus.SelectedItem.ToString()
            };

            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.FullName))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            try
            {
                if (mode == FormMode.Add)
                {
                    await userRepo.AddUserAsync(user);
                    MessageBox.Show("Thêm user thành công!");
                }
                else
                {
                    await userRepo.UpdateUserAsync(user);
                    MessageBox.Show("Cập nhật user thành công!");
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
    }
}
