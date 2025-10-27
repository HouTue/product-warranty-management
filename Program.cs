using System;
using System.Windows.Forms;

namespace NoSQL_QL_BaoHanh
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Khởi chạy form đăng nhập
            Application.Run(new DangNhap());
        }

    }
}