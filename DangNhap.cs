using System;
using System.Drawing;
using System.Windows.Forms;
using NoSQL_QL_BaoHanh.Auth;
using NoSQL_QL_BaoHanh.CassandraServices;

namespace NoSQL_QL_BaoHanh
{
    public class DangNhap : Form
    {
        private Label lblTitle;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnExit;
        private Label lblStatus;

        private readonly UserRepository _userRepo;

        public DangNhap()
        {
            // Kết nối Cassandra
            CassandraService.Instance.Connect("127.0.0.1", "warranty_app_v3");
            _userRepo = new UserRepository();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Đăng nhập hệ thống";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new Size(380, 265);
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);

            // Title
            lblTitle = new Label
            {
                Text = "🔐 Đăng nhập hệ thống",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Padding = new Padding(0, 20, 0, 20)
            };

            // Username
            lblUsername = new Label { Text = "Tên đăng nhập:", AutoSize = true, Location = new Point(30, 70) };
            txtUsername = new TextBox { Location = new Point(150, 65), Size = new Size(200, 30) };

            // Password
            lblPassword = new Label { Text = "Mật khẩu:", AutoSize = true, Location = new Point(30, 120) };
            txtPassword = new TextBox { Location = new Point(150, 115), Size = new Size(200, 30), UseSystemPasswordChar = true };

            // Login button
            btnLogin = new Button
            {
                Text = "Đăng nhập",
                Size = new Size(120, 40),
                Location = new Point(60, 180),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += btnLogin_Click;

            // Exit button
            btnExit = new Button
            {
                Text = "Thoát",
                Size = new Size(120, 40),
                Location = new Point(200, 180),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Click += btnExit_Click;

            // Status label
            lblStatus = new Label
            {
                ForeColor = Color.Red,
                AutoSize = true,
                Location = new Point(30, 240),
                Size = new Size(320, 30)
            };

            // Add Controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnExit);
            this.Controls.Add(lblStatus);
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";
            btnLogin.Enabled = false;

            try
            {
                var username = txtUsername.Text.Trim();
                var password = txtPassword.Text.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    lblStatus.Text = "⚠ Vui lòng nhập đầy đủ thông tin!";
                    return;
                }

                var user = await _userRepo.GetByUsernameAsync(username);
                if (user == null)
                {
                    lblStatus.Text = "❌ Tài khoản không tồn tại!";
                    return;
                }

                if (user.Status != "active")
                {
                    lblStatus.Text = "🚫 Tài khoản đã bị khóa!";
                    return;
                }

                if (password != user.Password)
                {
                    lblStatus.Text = "🔑 Sai mật khẩu!";
                    return;
                }

                MessageBox.Show($"✅ Đăng nhập thành công!\nXin chào {user.FullName} ({user.Role})",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Hide();
                var mainForm = new MainForm(user);
                mainForm.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi: " + ex.Message;
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            CassandraService.Instance.Dispose();
            Application.Exit();
        }
    }
}
