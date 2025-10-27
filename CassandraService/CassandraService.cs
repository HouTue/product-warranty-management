using Cassandra;
using System;

namespace NoSQL_QL_BaoHanh.CassandraServices
{
    public sealed class CassandraService
    {
        private static readonly Lazy<CassandraService> _instance =
            new Lazy<CassandraService>(() => new CassandraService());

        public static CassandraService Instance => _instance.Value;

        private Cluster _cluster;
        public ISession Session { get; private set; }

        private CassandraService() { }

        public void Connect(string host = "127.0.0.1", string keyspace = "warranty_app_v3")
        {
            try
            {
                if (Session != null) return;

                _cluster = Cluster.Builder()
                    .AddContactPoint(host)
                    .Build();

                Session = _cluster.Connect(keyspace);
                Console.WriteLine("Kết nối Cassandra thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi kết nối Cassandra: " + ex.Message);
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                Session?.Dispose();
                _cluster?.Dispose();
                Console.WriteLine("Đã ngắt kết nối Cassandra.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi đóng kết nối: " + ex.Message);
            }
        }
    }
}
