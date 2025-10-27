//using Cassandra;
//using NoSQL_QL_BaoHanh.CassandraServices;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace NoSQL_QL_BaoHanh.Auth
//{
//    public class RepairRecord
//    {
//        public string TechnicianId { get; set; }
//        public string RepairId { get; set; }
//        public string TicketId { get; set; }
//        public string SerialNumber { get; set; }
//        public DateTime StartDate { get; set; }
//        public DateTime? CompleteDate { get; set; }
//        public string RepairDescription { get; set; }
//        public string PartsUsed { get; set; }
//        public decimal Cost { get; set; }
//        public string Status { get; set; } // pending | in_progress | completed
//    }

//    public class RepairRepository
//    {
//        private readonly ISession _session;

//        public RepairRepository()
//        {
//            _session = CassandraService.Instance.Session;
//        }

//        #region Sinh mã tự động (REPAIR001, REPAIR002...)
//        public async Task<string> GenerateRepairIdAsync()
//        {
//            try
//            {
//                var querySelect = "SELECT last_number FROM repair_sequence WHERE key = 'REPAIR'";
//                var row = (await _session.ExecuteAsync(new SimpleStatement(querySelect))).FirstOrDefault();

//                int lastNumber = row != null ? row.GetValue<int>("last_number") : 0;
//                int newNumber = lastNumber + 1;

//                var queryUpdate = "UPDATE repair_sequence SET last_number = ? WHERE key = 'REPAIR'";
//                await _session.ExecuteAsync(new SimpleStatement(queryUpdate, newNumber));

//                return $"REPAIR{newNumber.ToString("D3")}";
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Lỗi sinh mã sửa chữa: {ex.Message}");
//                return "REPAIR001";
//            }
//        }
//        #endregion

//        #region Tạo phiếu sửa chữa
//        public async Task<bool> CreateRepairAsync(RepairRecord repair)
//        {
//            try
//            {
//                string query = @"INSERT INTO repairs_by_technician 
//                    (technician_id, repair_id, ticket_id, serial_number, start_date, complete_date, repair_description, parts_used, cost, status)
//                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

//                await _session.ExecuteAsync(new SimpleStatement(query,
//                    repair.TechnicianId,
//                    repair.RepairId,
//                    repair.TicketId,
//                    repair.SerialNumber,
//                    repair.StartDate,
//                    repair.CompleteDate,
//                    repair.RepairDescription,
//                    repair.PartsUsed,
//                    repair.Cost,
//                    repair.Status));

//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Lỗi tạo phiếu sửa chữa: {ex.Message}");
//                return false;
//            }
//        }
//        #endregion

//        #region Lấy danh sách phiếu theo kỹ thuật viên
//        public async Task<List<RepairRecord>> GetRepairsByTechnicianAsync(string technicianId)
//        {
//            var repairs = new List<RepairRecord>();

//            try
//            {
//                string query = @"SELECT technician_id, repair_id, ticket_id, serial_number, start_date, complete_date, repair_description, parts_used, cost, status
//                                 FROM repairs_by_technician WHERE technician_id = ?";

//                var rows = await _session.ExecuteAsync(new SimpleStatement(query, technicianId));
//                foreach (var row in rows)
//                {
//                    repairs.Add(new RepairRecord
//                    {
//                        TechnicianId = row.GetValue<string>("technician_id"),
//                        RepairId = row.GetValue<string>("repair_id"),
//                        TicketId = row.GetValue<string>("ticket_id"),
//                        SerialNumber = row.GetValue<string>("serial_number"),
//                        StartDate = row.GetValue<DateTime>("start_date"),
//                        CompleteDate = row.GetValue<DateTime?>("complete_date"),
//                        RepairDescription = row.GetValue<string>("repair_description"),
//                        PartsUsed = row.GetValue<string>("parts_used"),
//                        Cost = row.GetValue<decimal>("cost"),
//                        Status = row.GetValue<string>("status")
//                    });
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Lỗi khi lấy danh sách sửa chữa: {ex.Message}");
//            }

//            return repairs;
//        }
//        #endregion

//        #region Cập nhật trạng thái phiếu sửa chữa
//        public async Task<bool> UpdateRepairStatusAsync(string technicianId, string repairId, string newStatus, DateTime? completeDate = null)
//        {
//            try
//            {
//                string query = @"UPDATE repairs_by_technician SET status = ?, complete_date = ?
//                                 WHERE technician_id = ? AND repair_id = ?";

//                await _session.ExecuteAsync(new SimpleStatement(query,
//                    newStatus, completeDate, technicianId, repairId));

//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Lỗi cập nhật trạng thái sửa chữa: {ex.Message}");
//                return false;
//            }
//        }
//        #endregion
//        #region Cập nhật trạng thái phiếu bảo hành liên kết
//        public async Task<bool> UpdateTicketStatusAsync(string ticketId, string serialNumber, string newStatus, string technicianId)
//        {
//            try
//            {
//                // Bảng product
//                string query1 = "UPDATE warranty_tickets_by_product SET status = ? WHERE serial_number = ? AND ticket_id = ?";
//                await _session.ExecuteAsync(new SimpleStatement(query1, newStatus, serialNumber, ticketId));

//                // Bảng technician
//                string query2 = "UPDATE warranty_tickets_by_technician SET status = ? WHERE technician_id = ? AND ticket_id = ?";
//                await _session.ExecuteAsync(new SimpleStatement(query2, newStatus, technicianId, ticketId));

//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Lỗi đồng bộ trạng thái phiếu bảo hành: {ex.Message}");
//                return false;
//            }
//        }
//        #endregion

//    }
//}
using Cassandra;
using NoSQL_QL_BaoHanh.CassandraServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSQL_QL_BaoHanh.Auth
{
    public class RepairRecord
    {
        public string TechnicianId { get; set; }
        public string RepairId { get; set; }
        public string TicketId { get; set; }
        public string SerialNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? CompleteDate { get; set; }
        public string RepairDescription { get; set; }
        public string PartsUsed { get; set; }
        public decimal Cost { get; set; }
        public string Status { get; set; } // pending | in_progress | completed
    }

    public class RepairRepository
    {
        private readonly ISession _session;

        public RepairRepository()
        {
            _session = CassandraService.Instance.Session;
        }

        // Sinh mã REPAIR001, REPAIR002...
        public async Task<string> GenerateRepairIdAsync()
        {
            try
            {
                var result = await _session.ExecuteAsync(new SimpleStatement(
                    "SELECT last_number FROM repair_sequence WHERE key = 'REPAIR'"));

                var row = result.FirstOrDefault();
                int lastNumber = row != null ? row.GetValue<int>("last_number") : 0;
                int newNumber = lastNumber + 1;

                await _session.ExecuteAsync(new SimpleStatement(
                    "UPDATE repair_sequence SET last_number = ? WHERE key = 'REPAIR'", newNumber));

                return $"REPAIR{newNumber.ToString("D3")}";
            }
            catch
            {
                return "REPAIR001";
            }
        }

        // Tạo phiếu sửa chữa
        public async Task<bool> CreateRepairAsync(RepairRecord repair)
        {
            try
            {
                string query = @"INSERT INTO repairs_by_technician 
                (technician_id, repair_id, ticket_id, serial_number, start_date, complete_date, repair_description, parts_used, cost, status)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                await _session.ExecuteAsync(new SimpleStatement(query,
                    repair.TechnicianId,
                    repair.RepairId,
                    repair.TicketId,
                    repair.SerialNumber,
                    repair.StartDate,
                    repair.CompleteDate,
                    repair.RepairDescription,
                    repair.PartsUsed,
                    repair.Cost,
                    repair.Status
                ));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lưu sửa chữa: {ex.Message}");
                return false;
            }
        }

        // Lấy danh sách phiếu theo kỹ thuật viên
        public async Task<List<RepairRecord>> GetRepairsByTechnicianAsync(string technicianId)
        {
            var repairs = new List<RepairRecord>();

            try
            {
                string query = @"SELECT technician_id, repair_id, ticket_id, serial_number, 
                                start_date, complete_date, repair_description, parts_used, cost, status
                                FROM repairs_by_technician WHERE technician_id = ?";

                var result = await _session.ExecuteAsync(new SimpleStatement(query, technicianId));

                foreach (var row in result)
                {
                    repairs.Add(new RepairRecord
                    {
                        TechnicianId = row.GetValue<string>("technician_id"),
                        RepairId = row.GetValue<string>("repair_id"),
                        TicketId = row.GetValue<string>("ticket_id"),
                        SerialNumber = row.GetValue<string>("serial_number"),
                        StartDate = row.GetValue<DateTime>("start_date"),
                        CompleteDate = row.IsNull("complete_date") ? (DateTime?)null : row.GetValue<DateTime>("complete_date"),
                        RepairDescription = row.GetValue<string>("repair_description"),
                        PartsUsed = row.GetValue<string>("parts_used"),
                        Cost = row.GetValue<decimal>("cost"),
                        Status = row.GetValue<string>("status")
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy phiếu: {ex.Message}");
            }

            return repairs;
        }

        // Cập nhật trạng thái
        public async Task<bool> UpdateRepairStatusAsync(string technicianId, string repairId, string newStatus, DateTime? completeDate = null)
        {
            try
            {
                string query = @"UPDATE repairs_by_technician 
                                SET status = ?, complete_date = ?
                                WHERE technician_id = ? AND repair_id = ?";

                await _session.ExecuteAsync(new SimpleStatement(query,
                    newStatus,
                    completeDate,
                    technicianId,
                    repairId
                ));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi cập nhật trạng thái sửa chữa: {ex.Message}");
                return false;
            }
        }
        public async Task<List<RepairRecord>> GetAllRepairsAsync()
        {
            var repairs = new List<RepairRecord>();
            try
            {
                string query = @"SELECT technician_id, repair_id, ticket_id, serial_number, start_date, 
                                complete_date, repair_description, parts_used, cost, status
                         FROM repairs_by_technician";

                var rows = await _session.ExecuteAsync(new SimpleStatement(query));

                foreach (var row in rows)
                {
                    repairs.Add(new RepairRecord
                    {
                        TechnicianId = row.GetValue<string>("technician_id"),
                        RepairId = row.GetValue<string>("repair_id"),
                        TicketId = row.GetValue<string>("ticket_id"),
                        SerialNumber = row.GetValue<string>("serial_number"),
                        StartDate = row.GetValue<DateTime>("start_date"),
                        CompleteDate = row.GetValue<DateTime?>("complete_date"),
                        RepairDescription = row.GetValue<string>("repair_description"),
                        PartsUsed = row.GetValue<string>("parts_used"),
                        Cost = row.GetValue<decimal>("cost"),
                        Status = row.GetValue<string>("status")
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi truy vấn tất cả phiếu sửa chữa: {ex.Message}");
            }
            return repairs;
        }
    }
}
