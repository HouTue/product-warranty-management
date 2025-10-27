//using System;

//namespace NoSQL_QL_BaoHanh.Auth
//{
//    public class WarrantyTicketRecord
//    {
//        public string TicketId { get; set; }
//        public string SerialNumber { get; set; }
//        public string CustomerId { get; set; }
//        public DateTime CreatedAt { get; set; }
//        public string IssueDescription { get; set; }
//        public string Status { get; set; }
//        public string TechnicianId { get; set; }
//    }
//}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using NoSQL_QL_BaoHanh.CassandraServices;

namespace NoSQL_QL_BaoHanh.Auth
{
    public class WarrantyTicketRecord
    {
        public string TechnicianId { get; set; }
        public string TicketId { get; set; }
        public string SerialNumber { get; set; }
        public string CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string IssueDescription { get; set; }
        public string Status { get; set; }
    }

    
}
