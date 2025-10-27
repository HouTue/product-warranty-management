using System;
using System.Drawing;
using System.Windows.Forms;
using NoSQL_QL_BaoHanh.Auth;
using NoSQL_QL_BaoHanh.Forms;

namespace NoSQL_QL_BaoHanh
{
    public class MainForm : Form
    {
        private readonly UserRecord _currentUser;
        private Label lblWelcome;
        private FlowLayoutPanel menuPanel;
        private Button btnUserManagement;
        private Button btnRevenueStats;
        private Button btnTicketManagement;
        private Button btnCustomerManagement;
        private Button btnProductManagement;
        private Button btnTechRepair;
        private Button btnLogout;
        public MainForm(UserRecord user)
        {
            _currentUser = user;
            InitializeComponent();
            ApplyRolePermissions();

        }

        private void InitializeComponent()
        {

            this.Text = "Hệ thống quản lý bảo hành sản phẩm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new Size(300, 550);
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 11);

            lblWelcome = new Label
            {
                Text = $"👋 Xin chào: {_currentUser.FullName} \n({_currentUser.Role})",
                AutoSize = true,
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            };

            menuPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 70),
                Size = new Size(300, 600),
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true
            };
            this.Load += (s, e) =>
            {
                foreach (Control ctrl in menuPanel.Controls)
                {
                    ctrl.Left = (menuPanel.ClientSize.Width - ctrl.Width) / 2;
                }
            };
            // Thêm nút quản lý người dùng
            btnUserManagement = AddMenuButton("Quản lý người dùng", BtnUserManagement_Click);
            btnRevenueStats = AddMenuButton("Thống kê doanh thu", BtnRevenueStats_Click);
            btnTicketManagement = AddMenuButton("Phiếu yêu cầu bảo hành", BtnTicketManagement_Click);
            btnCustomerManagement = AddMenuButton("Quản lý khách hàng", BtnCustomerManagement_Click);
            btnProductManagement = AddMenuButton("Quản lý sản phẩm", BtnProductManagement_Click);
            btnTechRepair = AddMenuButton("Quản lý sửa chữa bảo hành", BtnTechRepair_Click);




            btnLogout = new Button
            {
                Text = "🚪 Đăng xuất",
                Size = new Size(250, 50),
                BackColor = Color.IndianRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Margin = new Padding(5),

            };
            btnLogout.Click += BtnLogout_Click;
            menuPanel.Controls.Add(btnLogout);

            this.Controls.Add(lblWelcome);
            this.Controls.Add(menuPanel);

        }

        private Button AddMenuButton(string text, EventHandler onClick)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(250, 50),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Margin = new Padding(5)
            };

            if (onClick != null)
                btn.Click += onClick;
            menuPanel.Controls.Add(btn);
            return btn;
        }

        private void BtnUserManagement_Click(object sender, EventArgs e)
        {
            var form = new UserManagementForm();
            form.ShowDialog();
        }
        private void BtnProductManagement_Click(object sender, EventArgs e)
        {
            var form = new ProductManagementForm();
            form.ShowDialog();
        }
        private void BtnTicketManagement_Click(object sender, EventArgs e)
        {
            var form = new TicketManagementForm(_currentUser);
            form.ShowDialog();
        }
        private void BtnCustomerManagement_Click(object sender, EventArgs e)
        {
            var form = new CustomerManagementForm();
            form.ShowDialog();
        }
        private void BtnTechRepair_Click(object sender, EventArgs e)
        {
            var form = new TechRepairManagementForm(_currentUser.Username);
            form.ShowDialog();
        }
        private void BtnRevenueStats_Click(object sender, EventArgs e)
        {
            var form = new RevenueStatisticsForm(_currentUser);
            form.ShowDialog();
        }
        private void BtnLogout_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Hide();
                var loginForm = new DangNhap();
                loginForm.ShowDialog();
                this.Close();
            }
        }
        private void ApplyRolePermissions()
        {
            string role = _currentUser.Role.ToLower();

            // Mặc định tất cả bị disable và chuyển màu xám
            void SetDisabled(Button btn)
            {
                btn.Enabled = false;
                btn.BackColor = Color.LightGray;
                btn.ForeColor = Color.DarkGray;
            }

            void SetEnabled(Button btn)
            {
                btn.Enabled = true;
                btn.BackColor = Color.FromArgb(0, 120, 215);
                btn.ForeColor = Color.White;
            }

            // Reset tất cả về disabled trước
            Button[] allButtons = {
        btnUserManagement, btnRevenueStats, btnTicketManagement,
        btnCustomerManagement, btnProductManagement, btnTechRepair
    };

            foreach (var btn in allButtons)
                SetDisabled(btn);

            // Áp dụng quyền
            if (role == "admin")
            {
                SetEnabled(btnUserManagement);
                SetEnabled(btnRevenueStats);
                SetEnabled(btnTicketManagement);
                SetEnabled(btnCustomerManagement);
                SetEnabled(btnProductManagement);
                // btnTechRepair bị vô hiệu hóa
            }
            else if (role == "staff")
            {
                SetEnabled(btnCustomerManagement);
                SetEnabled(btnProductManagement);
            }
            else if (role == "tech")
            {
                SetEnabled(btnTechRepair);
            }


        }
    }
}
