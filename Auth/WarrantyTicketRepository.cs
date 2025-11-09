using Cassandra;
using NoSQL_QL_BaoHanh.CassandraServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSQL_QL_BaoHanh.Auth
{
    public class WarrantyTicketRepository
    {
        private readonly ISession _session;

        public WarrantyTicketRepository()
        {
            _session = CassandraService.Instance.Session;
        }

        #region Lấy danh sách kỹ thuật viên (role = tech, status = active)
        public async Task<List<UserRecord>> GetTechniciansAsync()
        {
            var technicians = new List<UserRecord>();
            try
            {
                var query = "SELECT username, full_name, role, status FROM users_by_username WHERE role = 'tech';";
                var result = await _session.ExecuteAsync(new SimpleStatement(query));

                foreach (var row in result)
                {
                    string status = row.GetValue<string>("status") ?? "";
                    if (status.ToLower() == "active")
                    {
                        technicians.Add(new UserRecord
                        {
                            Username = row.GetValue<string>("username"),
                            FullName = row.GetValue<string>("full_name"),
                            Role = row.GetValue<string>("role"),
                            Status = status
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy danh sách kỹ thuật viên: {ex.Message}");
            }

            return technicians;
        }
        #endregion

        #region Lấy danh sách phiếu theo trạng thái
        public async Task<List<WarrantyTicketRecord>> GetTicketsByStatusAsync(string status = "all")
        {
            var tickets = new List<WarrantyTicketRecord>();

            try
            {
                RowSet result;
                if (status == "all")
                {
                    result = await _session.ExecuteAsync(new SimpleStatement(
                        "SELECT ticket_id, serial_number, customer_id, created_at, issue_description, status, technician_id FROM warranty_tickets_by_product"));
                }
                else
                {
                    result = await _session.ExecuteAsync(new SimpleStatement(
                        "SELECT ticket_id, serial_number, customer_id, created_at, issue_description, status, technician_id FROM warranty_tickets_by_product WHERE status = ? ALLOW FILTERING", status));
                }

                foreach (var row in result)
                {
                    tickets.Add(new WarrantyTicketRecord
                    {
                        TicketId = row.GetValue<string>("ticket_id"),
                        SerialNumber = row.GetValue<string>("serial_number"),
                        CustomerId = row.GetValue<string>("customer_id"),
                        CreatedAt = row.GetValue<DateTime>("created_at"),
                        IssueDescription = row.GetValue<string>("issue_description"),
                        Status = row.GetValue<string>("status"),
                        TechnicianId = row.GetValue<string>("technician_id")
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi truy vấn phiếu bảo hành: {ex.Message}");
            }

            return tickets;
        }
        #endregion

        #region Sinh mã tự động (WR0001, WR0002,...)
        public async Task<string> GenerateNewTicketIdAsync(string serialNumber)
        {
            //try
            //{
            //    var query = "SELECT ticket_id FROM warranty_tickets_by_product WHERE serial_number = ? ORDER BY ticket_id DESC LIMIT 1";
            //    var result = await _session.ExecuteAsync(new SimpleStatement(query, serialNumber));
            //    var row = result.FirstOrDefault();

            //    if (row != null)
            //    {
            //        string lastId = row.GetValue<string>("ticket_id");
            //        int number = int.Parse(lastId.Replace("WR", "").Trim());
            //        return $"WR{(number + 1).ToString("D4")}";
            //    }

            //    return "WR0001";
            //}
            //catch
            //{
            //    return "WR0001";
            //}
            try
            {
                // 1. Lấy số mới nhất
                var row = await _session.ExecuteAsync(new SimpleStatement("SELECT last_number FROM ticket_sequence WHERE key = 'TICKET'"));
                int lastNumber = 0;

                var firstRow = row.FirstOrDefault();
                if (firstRow != null)
                {
                    lastNumber = firstRow.GetValue<int>("last_number");
                }

                // 2. Tăng số thứ tự
                int newNumber = lastNumber + 1;

                // 3. Cập nhật lại bảng sequence
                await _session.ExecuteAsync(new SimpleStatement("UPDATE ticket_sequence SET last_number = ? WHERE key = 'TICKET'", newNumber));

                // 4. Trả về mã mới
                return $"WR{newNumber.ToString("D4")}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi sinh mã tự động: {ex.Message}");
                return "WR0001"; // fallback
            }
        }
        #endregion

        #region Tạo phiếu bảo hành mới (pending)
        public async Task<bool> CreateWarrantyTicketAsync(WarrantyTicketRecord ticket)
        {
            try
            {
                string query1 = @"INSERT INTO warranty_tickets_by_product 
                (ticket_id, serial_number, customer_id, created_at, issue_description, status, technician_id)
                VALUES (?, ?, ?, ?, ?, ?, ?)";

                await _session.ExecuteAsync(new SimpleStatement(query1,
                    ticket.TicketId,
                    ticket.SerialNumber,
                    ticket.CustomerId,
                    ticket.CreatedAt,
                    ticket.IssueDescription,
                    "pending",
                    null));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tạo phiếu mới: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Phân công kỹ thuật viên
        public async Task<bool> AssignTechnicianAsync(string ticketId, string serialNumber, string technicianId)
        {
            try
            {
                // Cập nhật bảng product
                string query1 = "UPDATE warranty_tickets_by_product SET technician_id = ?, status = 'assigned' WHERE serial_number = ? AND ticket_id = ?";
                await _session.ExecuteAsync(new SimpleStatement(query1, technicianId, serialNumber, ticketId));

                // Lấy dữ liệu để insert vào bảng technician
                string queryGet = "SELECT customer_id, created_at, issue_description FROM warranty_tickets_by_product WHERE serial_number = ? AND ticket_id = ?";
                var row = (await _session.ExecuteAsync(new SimpleStatement(queryGet, serialNumber, ticketId))).FirstOrDefault();

                if (row == null) return false;

                string customerId = row.GetValue<string>("customer_id");
                DateTime createdAt = row.GetValue<DateTime>("created_at");
                string issue = row.GetValue<string>("issue_description");

                // Cập nhật bảng technician
                string query2 = @"INSERT INTO warranty_tickets_by_technician 
                (technician_id, ticket_id, serial_number, customer_id, created_at, issue_description, status)
                VALUES (?, ?, ?, ?, ?, ?, 'assigned')";
                await _session.ExecuteAsync(new SimpleStatement(query2, technicianId, ticketId, serialNumber, customerId, createdAt, issue));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi phân công kỹ thuật viên: {ex.Message}");
                return false;
            }
        }
        #endregion

        //#region Cập nhật trạng thái phiếu
        //public async Task<bool> UpdateTicketStatusAsync(string ticketId, string serialNumber, string newStatus)
        //{
        //    try
        //    {
        //        // Lấy technician_id hiện tại
        //        string queryGetTech = "SELECT technician_id FROM warranty_tickets_by_product WHERE serial_number = ? AND ticket_id = ?";
        //        var row = (await _session.ExecuteAsync(new SimpleStatement(queryGetTech, serialNumber, ticketId))).FirstOrDefault();

        //        string technicianId = row?.GetValue<string>("technician_id");

        //        // Cập nhật vào bảng product
        //        string query1 = "UPDATE warranty_tickets_by_product SET status = ? WHERE serial_number = ? AND ticket_id = ?";
        //        await _session.ExecuteAsync(new SimpleStatement(query1, newStatus, serialNumber, ticketId));

        //        // Nếu có technician thì cập nhật bảng technician
        //        if (!string.IsNullOrWhiteSpace(technicianId))
        //        {
        //            string query2 = "UPDATE warranty_tickets_by_technician SET status = ? WHERE technician_id = ? AND ticket_id = ?";
        //            await _session.ExecuteAsync(new SimpleStatement(query2, newStatus, technicianId, ticketId));
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Lỗi cập nhật trạng thái: {ex.Message}");
        //        return false;
        //    }
        //}
        //#endregion
        #region Cập nhật trạng thái phiếu bảo hành (đầy đủ tham số)
        public async Task<bool> UpdateTicketStatusAsync(string ticketId, string serialNumber, string newStatus, string technicianId)
        {
            try
            {
                // Cập nhật bảng warranty_tickets_by_product
                string query1 = "UPDATE warranty_tickets_by_product SET status = ?, technician_id = ? WHERE serial_number = ? AND ticket_id = ?";
                await _session.ExecuteAsync(new SimpleStatement(query1, newStatus, technicianId, serialNumber, ticketId));

                // Cập nhật bảng warranty_tickets_by_technician nếu đã được assign
                if (!string.IsNullOrEmpty(technicianId))
                {
                    string query2 = "UPDATE warranty_tickets_by_technician SET status = ? WHERE technician_id = ? AND ticket_id = ?";
                    await _session.ExecuteAsync(new SimpleStatement(query2, newStatus, technicianId, ticketId));
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi cập nhật trạng thái phiếu bảo hành: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Lấy phiếu theo kỹ thuật viên (dùng cho TECH đăng nhập)
        public async Task<List<WarrantyTicketRecord>> GetTicketsByTechnicianAsync(string technicianId, string status = "all")
        {
            var tickets = new List<WarrantyTicketRecord>();

            try
            {
                RowSet result;

                if (status == "all")
                {
                    result = await _session.ExecuteAsync(new SimpleStatement(
                        "SELECT technician_id, ticket_id, serial_number, customer_id, created_at, issue_description, status FROM warranty_tickets_by_technician WHERE technician_id = ?", technicianId));
                }
                else
                {
                    result = await _session.ExecuteAsync(new SimpleStatement(
                        "SELECT technician_id, ticket_id, serial_number, customer_id, created_at, issue_description, status FROM warranty_tickets_by_technician WHERE technician_id = ? AND status = ? ALLOW FILTERING;", technicianId, status));
                }

                foreach (var row in result)
                {
                    tickets.Add(new WarrantyTicketRecord
                    {
                        TechnicianId = row.GetValue<string>("technician_id"),
                        TicketId = row.GetValue<string>("ticket_id"),
                        SerialNumber = row.GetValue<string>("serial_number"),
                        CustomerId = row.GetValue<string>("customer_id"),
                        CreatedAt = row.GetValue<DateTime>("created_at"),
                        IssueDescription = row.GetValue<string>("issue_description"),
                        Status = row.GetValue<string>("status")
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi truy vấn phiếu theo kỹ thuật viên: {ex.Message}");
            }

            return tickets;
        }
        #endregion
    }
}
