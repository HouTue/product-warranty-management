using Cassandra;
using NoSQL_QL_BaoHanh.CassandraServices;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NoSQL_QL_BaoHanh.Auth
{
    public class ProductRepository
    {
        private readonly ISession _session;

        public ProductRepository()
        {
            _session = CassandraService.Instance.Session;
        }

        /// <summary>
        /// Lấy thông tin sản phẩm theo Serial Number
        /// </summary>
        public async Task<ProductRecord?> GetBySerialAsync(string serial)
        {
            try
            {
                var query = "SELECT serial_number, product_name, purchase_date, warranty_months, customer_id, status, image_url " +
                            "FROM products_by_serial WHERE serial_number = ?";

                var statement = new SimpleStatement(query, serial);
                var result = await _session.ExecuteAsync(statement);
                var row = result.FirstOrDefault();

                if (row == null) return null;

                var localDate = row.GetValue<LocalDate>("purchase_date");
                var purchaseDate = new DateTime(localDate.Year, localDate.Month, localDate.Day);

                return new ProductRecord
                {
                    SerialNumber = row.GetValue<string>("serial_number"),
                    ProductName = row.GetValue<string>("product_name"),
                    PurchaseDate = purchaseDate,
                    WarrantyMonths = row.GetValue<int>("warranty_months"),
                    CustomerId = row.GetValue<string>("customer_id"),
                    Status = row.GetValue<string>("status"),
                    ImageUrl = row.GetValue<string>("image_url")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi truy vấn sản phẩm: {ex.Message}");
                return null;
            }
        }
    }
}
