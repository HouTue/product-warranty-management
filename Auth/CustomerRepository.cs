using Cassandra;
using NoSQL_QL_BaoHanh.CassandraServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSQL_QL_BaoHanh.Auth
{
    public class CustomerRecord
    {
        public string CustomerId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Status { get; set; } // active | deactive
    }

    public class CustomerRepository
    {
        private readonly ISession _session;

        public CustomerRepository()
        {
            _session = CassandraService.Instance.Session;
        }

        #region Sinh mã tự động (CUST001, CUST002...)
        public async Task<string> GenerateCustomerIdAsync()
        {
            try
            {
                var querySelect = "SELECT last_number FROM customer_sequence WHERE key = 'CUSTOMER'";
                var row = (await _session.ExecuteAsync(new SimpleStatement(querySelect))).FirstOrDefault();

                int lastNumber = row != null ? row.GetValue<int>("last_number") : 0;
                int newNumber = lastNumber + 1;

                var queryUpdate = "UPDATE customer_sequence SET last_number = ? WHERE key = 'CUSTOMER'";
                await _session.ExecuteAsync(new SimpleStatement(queryUpdate, newNumber));

                return $"CUST{newNumber.ToString("D3")}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi sinh mã khách hàng: {ex.Message}");
                return "CUST001";
            }
        }
        #endregion

        #region Thêm khách hàng mới
        public async Task<bool> AddCustomerAsync(CustomerRecord customer)
        {
            try
            {
                string query1 = @"INSERT INTO customers_by_id 
                (customer_id, full_name, phone, email, address, status) 
                VALUES (?, ?, ?, ?, ?, ?)";

                string query2 = @"INSERT INTO customers_by_phone 
                (phone, customer_id, full_name, email, address, status) 
                VALUES (?, ?, ?, ?, ?, ?)";

                await _session.ExecuteAsync(new SimpleStatement(query1,
                    customer.CustomerId,
                    customer.FullName,
                    customer.Phone,
                    customer.Email,
                    customer.Address,
                    customer.Status));

                await _session.ExecuteAsync(new SimpleStatement(query2,
                    customer.Phone,
                    customer.CustomerId,
                    customer.FullName,
                    customer.Email,
                    customer.Address,
                    customer.Status));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi thêm khách hàng: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Lấy toàn bộ khách hàng
        public async Task<List<CustomerRecord>> GetAllCustomersAsync()
        {
            var customers = new List<CustomerRecord>();

            try
            {
                string query = "SELECT customer_id, full_name, phone, email, address, status FROM customers_by_id";
                var result = await _session.ExecuteAsync(new SimpleStatement(query));

                foreach (var row in result)
                {
                    customers.Add(new CustomerRecord
                    {
                        CustomerId = row.GetValue<string>("customer_id"),
                        FullName = row.GetValue<string>("full_name"),
                        Phone = row.GetValue<string>("phone"),
                        Email = row.GetValue<string>("email"),
                        Address = row.GetValue<string>("address"),
                        Status = row.GetValue<string>("status")
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy danh sách khách hàng: {ex.Message}");
            }

            return customers;
        }
        #endregion

        #region Tìm theo ID
        public async Task<CustomerRecord> SearchByIdAsync(string customerId)
        {
            try
            {
                string query = "SELECT customer_id, full_name, phone, email, address, status FROM customers_by_id WHERE customer_id = ?";
                var result = await _session.ExecuteAsync(new SimpleStatement(query, customerId));
                var row = result.FirstOrDefault();

                if (row != null)
                {
                    return new CustomerRecord
                    {
                        CustomerId = row.GetValue<string>("customer_id"),
                        FullName = row.GetValue<string>("full_name"),
                        Phone = row.GetValue<string>("phone"),
                        Email = row.GetValue<string>("email"),
                        Address = row.GetValue<string>("address"),
                        Status = row.GetValue<string>("status")
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tìm khách hàng theo ID: {ex.Message}");
            }

            return null;
        }
        #endregion

        #region Tìm theo số điện thoại
        public async Task<CustomerRecord> SearchByPhoneAsync(string phone)
        {
            try
            {
                string query = "SELECT customer_id, full_name, phone, email, address, status FROM customers_by_phone WHERE phone = ?";
                var result = await _session.ExecuteAsync(new SimpleStatement(query, phone));
                var row = result.FirstOrDefault();

                if (row != null)
                {
                    return new CustomerRecord
                    {
                        CustomerId = row.GetValue<string>("customer_id"),
                        FullName = row.GetValue<string>("full_name"),
                        Phone = row.GetValue<string>("phone"),
                        Email = row.GetValue<string>("email"),
                        Address = row.GetValue<string>("address"),
                        Status = row.GetValue<string>("status")
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tìm khách hàng theo phone: {ex.Message}");
            }

            return null;
        }
        #endregion

        #region Cập nhật khách hàng
        public async Task<bool> UpdateCustomerAsync(CustomerRecord customer)
        {
            try
            {
                string query1 = @"UPDATE customers_by_id SET 
                    full_name = ?, email = ?, address = ?, status = ?
                    WHERE customer_id = ?";

                string query2 = @"UPDATE customers_by_phone SET 
                    full_name = ?, email = ?, address = ?, status = ?
                    WHERE phone = ?";

                await _session.ExecuteAsync(new SimpleStatement(query1,
                    customer.FullName, customer.Email, customer.Address, customer.Status, customer.CustomerId));

                await _session.ExecuteAsync(new SimpleStatement(query2,
                    customer.FullName, customer.Email, customer.Address, customer.Status, customer.Phone));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi cập nhật khách hàng: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Vô hiệu hóa khách hàng
        public async Task<bool> DeactivateCustomerAsync(string customerId)
        {
            try
            {
                // Lấy thông tin khách để đồng bộ bảng phone
                var customer = await SearchByIdAsync(customerId);
                if (customer == null) return false;

                string query1 = "UPDATE customers_by_id SET status = 'deactive' WHERE customer_id = ?";
                string query2 = "UPDATE customers_by_phone SET status = 'deactive' WHERE phone = ?";

                await _session.ExecuteAsync(new SimpleStatement(query1, customerId));
                await _session.ExecuteAsync(new SimpleStatement(query2, customer.Phone));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi vô hiệu hóa khách hàng: {ex.Message}");
                return false;
            }
        }
        #endregion
    }
}
