using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using NoSQL_QL_BaoHanh.CassandraServices;

namespace NoSQL_QL_BaoHanh.Auth
{
    public class UserRepository
    {
        private readonly ISession _session;

        public UserRepository()
        {
            _session = CassandraService.Instance.Session;
        }

        // ✅ Lấy theo username (dùng cho đăng nhập)
        public async Task<UserRecord?> GetByUsernameAsync(string username)
        {
            var query = "SELECT username, password, full_name, role, status FROM users_by_username WHERE username = ? LIMIT 1";
            var statement = new SimpleStatement(query, username);
            var result = await _session.ExecuteAsync(statement);
            var row = result.FirstOrDefault();

            if (row == null) return null;

            return new UserRecord
            {
                Username = row.GetValue<string>("username"),
                Password = row.GetValue<string>("password"),
                FullName = row.GetValue<string>("full_name"),
                Role = row.GetValue<string>("role"),
                Status = row.GetValue<string>("status")
            };
        }

        // ✅ Lấy tất cả user
        public async Task<List<UserRecord>> GetAllUsersAsync()
        {
            var query = "SELECT username, password, full_name, role, status FROM users_by_username";
            var result = await _session.ExecuteAsync(new SimpleStatement(query));
            return result.Select(row => new UserRecord
            {
                Username = row.GetValue<string>("username"),
                Password = row.GetValue<string>("password"),
                FullName = row.GetValue<string>("full_name"),
                Role = row.GetValue<string>("role"),
                Status = row.GetValue<string>("status")
            }).ToList();
        }

        // ✅ Tìm kiếm user theo từ khóa
        public async Task<List<UserRecord>> SearchUsersAsync(string keyword)
        {
            keyword = keyword.ToLower();
            var allUsers = await GetAllUsersAsync();
            return allUsers.Where(u =>
                u.Username.ToLower().Contains(keyword) ||
                u.FullName.ToLower().Contains(keyword) ||
                u.Role.ToLower().Contains(keyword) ||
                u.Status.ToLower().Contains(keyword)).ToList();
        }

        // ✅ Thêm user mới
        public async Task AddUserAsync(UserRecord user)
        {
            var query = "INSERT INTO users_by_username (username, password, full_name, role, status) VALUES (?, ?, ?, ?, ?)";
            var statement = new SimpleStatement(query, user.Username, user.Password, user.FullName, user.Role, user.Status);
            await _session.ExecuteAsync(statement);
        }

        // ✅ Cập nhật user
        public async Task UpdateUserAsync(UserRecord user)
        {
            var query = "UPDATE users_by_username SET password=?, full_name=?, role=?, status=? WHERE username=?";
            var statement = new SimpleStatement(query, user.Password, user.FullName, user.Role, user.Status, user.Username);
            await _session.ExecuteAsync(statement);
        }

        // ✅ Xóa user
        public async Task DeleteUserAsync(string username)
        {
            var query = "DELETE FROM users_by_username WHERE username=?";
            var statement = new SimpleStatement(query, username);
            await _session.ExecuteAsync(statement);
        }
    }
}
