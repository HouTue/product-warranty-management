using System;

namespace NoSQL_QL_BaoHanh.Auth
{
    public class ProductRecord
    {
        public string SerialNumber { get; set; }
        public string ProductName { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int WarrantyMonths { get; set; }
        public string CustomerId { get; set; }
        public string Status { get; set; }
        public string ImageUrl { get; set; }

        // Tính ngày hết hạn bảo hành (dựa trên PurchaseDate + WarrantyMonths)
        public DateTime ExpiryDate => PurchaseDate.AddMonths(WarrantyMonths);
    }
}
