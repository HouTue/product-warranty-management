using BCrypt.Net;

namespace NoSQL_QL_BaoHanh.Auth
{
    public static class PasswordHasher
    {
        // Dùng khi tạo tài khoản
        public static string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        // Kiểm tra đăng nhập
        public static bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
