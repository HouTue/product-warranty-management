using System;
using System.Drawing;
using System.Windows.Forms;
using NoSQL_QL_BaoHanh.Auth;
using NoSQL_QL_BaoHanh.Forms;

namespace NoSQL_QL_BaoHanh
{
    public partial class ProductManagementForm : Form
    {
        private TextBox txtSerialSearch;
        private Button btnSearch;
        private Panel panelInfo;
        private Label lblProductName;
        private Label lblPurchaseDate;
        private Label lblWarrantyMonths;
        private Label lblExpiryDate;
        private Label lblRemainingDays;
        private Label lblStatus;
        private PictureBox picProductImage;
        private Button btnCreateRequest;
        private DataGridView dgvHistory;
        private readonly ProductRepository _productRepo = new ProductRepository();
        private ProductRecord _lastLoadedProduct;

        public ProductManagementForm()
        {
            InitializeComponent();

        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý sản phẩm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1000, 700);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 11);



            Label lblSerial = new Label
            {
                Text = "Serial Number:",
                Location = new Point(30, 25),
                AutoSize = true
            };

            txtSerialSearch = new TextBox
            {
                Location = new Point(150, 20),
                Size = new Size(300, 30)
            };

            btnSearch = new Button
            {
                Text = "🔍 Tra cứu",
                Location = new Point(470, 20),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(0, 160, 170),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += BtnSearch_Click;

            panelInfo = new Panel
            {
                Location = new Point(30, 70),
                Size = new Size(500, 300),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            int labelLeft = 20;
            int startTop = 20;
            int space = 40;

            lblProductName = CreateInfoLabel("Tên sản phẩm:", labelLeft, startTop);
            lblPurchaseDate = CreateInfoLabel("Ngày mua:", labelLeft, startTop + space);
            lblWarrantyMonths = CreateInfoLabel("Thời hạn bảo hành:", labelLeft, startTop + space * 2);
            lblExpiryDate = CreateInfoLabel("Ngày hết hạn:", labelLeft, startTop + space * 3);
            lblRemainingDays = CreateInfoLabel("Thời gian còn lại:", labelLeft, startTop + space * 4);
            lblStatus = CreateInfoLabel("Trạng thái bảo hành:", labelLeft, startTop + space * 5);

            panelInfo.Controls.Add(lblProductName);
            panelInfo.Controls.Add(lblPurchaseDate);
            panelInfo.Controls.Add(lblWarrantyMonths);
            panelInfo.Controls.Add(lblExpiryDate);
            panelInfo.Controls.Add(lblRemainingDays);
            panelInfo.Controls.Add(lblStatus);

            picProductImage = new PictureBox
            {
                Location = new Point(550, 70),
                Size = new Size(300, 300),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            btnCreateRequest = new Button
            {
                Text = "🛠 Tạo phiếu yêu cầu bảo hành",
                Location = new Point(350, 380),
                Size = new Size(300, 45),
                BackColor = Color.FromArgb(0, 160, 170),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Visible = false
            };
            btnCreateRequest.FlatAppearance.BorderSize = 0;
            btnCreateRequest.Click += BtnCreateRequest_Click;

            dgvHistory = new DataGridView
            {
                Location = new Point(30, 450),
                Size = new Size(920, 200),
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvHistory.Columns.Add("SerialNumber", "Serial Number");
            dgvHistory.Columns.Add("ProductName", "Tên sản phẩm");
            dgvHistory.Columns.Add("ExpiryDate", "Ngày hết hạn");
            dgvHistory.Columns.Add("Status", "Trạng thái");
            dgvHistory.Columns.Add("LookupTime", "Thời gian tra cứu");

            this.Controls.Add(lblSerial);
            this.Controls.Add(txtSerialSearch);
            this.Controls.Add(btnSearch);
            this.Controls.Add(panelInfo);
            this.Controls.Add(picProductImage);
            this.Controls.Add(btnCreateRequest);
            this.Controls.Add(dgvHistory);
        }

        private Label CreateInfoLabel(string text, int left, int top)
        {
            return new Label
            {
                Text = text,
                Location = new Point(left, top),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Regular)
            };
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            string serial = txtSerialSearch.Text.Trim();
            if (string.IsNullOrEmpty(serial))
            {
                MessageBox.Show("Vui lòng nhập Serial Number!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var product = await _productRepo.GetBySerialAsync(serial);
            if (product == null)
            {
                MessageBox.Show("Không tìm thấy sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearProductInfo();
                return;
            }

            DisplayProductInfo(product);
        }

        private void ClearProductInfo()
        {
            lblProductName.Text = "Tên sản phẩm:";
            lblPurchaseDate.Text = "Ngày mua:";
            lblWarrantyMonths.Text = "Thời hạn bảo hành:";
            lblExpiryDate.Text = "Ngày hết hạn:";
            lblRemainingDays.Text = "Thời gian còn lại:";
            lblStatus.Text = "Trạng thái bảo hành:";
            picProductImage.Image = null;
            btnCreateRequest.Visible = false;
        }

        private void DisplayProductInfo(ProductRecord product)
        {
            _lastLoadedProduct = product;
            lblProductName.Text = $"Tên sản phẩm: {product.ProductName}";
            lblPurchaseDate.Text = $"Ngày mua: {product.PurchaseDate:yyyy-MM-dd}";
            lblWarrantyMonths.Text = $"Thời hạn bảo hành: {product.WarrantyMonths} tháng";

            DateTime expiryDate = product.ExpiryDate;
            lblExpiryDate.Text = $"Ngày hết hạn: {expiryDate:yyyy-MM-dd}";

            int remainingDays = (expiryDate - DateTime.Now).Days;
            remainingDays = remainingDays < 0 ? 0 : remainingDays;
            lblRemainingDays.Text = $"Thời gian còn lại: {remainingDays} ngày";

            bool isInWarranty = expiryDate >= DateTime.Now;

            if (isInWarranty)
            {
                lblStatus.Text = "Trạng thái bảo hành: ✅ Còn hạn";
                lblStatus.ForeColor = Color.Green;
                btnCreateRequest.Visible = true;
            }
            else
            {
                lblStatus.Text = "Trạng thái bảo hành: ❌ Hết hạn";
                lblStatus.ForeColor = Color.Red;
                btnCreateRequest.Visible = false;
            }

            try
            {
                picProductImage.Load(product.ImageUrl);
            }
            catch
            {
                picProductImage.Image = null;
            }

            dgvHistory.Rows.Add(product.SerialNumber, product.ProductName,
                expiryDate.ToString("yyyy-MM-dd"),
                isInWarranty ? "Còn hạn" : "Hết hạn",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private void BtnCreateRequest_Click(object sender, EventArgs e)
        {
            string serial = txtSerialSearch.Text.Trim();
            string customerId = lblPurchaseDate.Text.Replace("Ngày mua: ", ""); // nếu bạn giữ đúng format thì giữ nguyên dòng này

            // KHÔNG lấy từ label purchase date, mà lấy đúng từ Repository (đã có)
            var form = new WarrantyTicketForm(serial, _lastLoadedProduct.CustomerId);
            form.ShowDialog();
        }

    }
}
