using Microsoft.Data.SqlClient;

namespace QLNhaThuoc.DAL
{
    /// <summary>
    /// Lớp cha của tất cả DAL — quản lý kết nối CSDL theo chuẩn N-Layer
    /// </summary>
    public class DBConnect
    {
        protected SqlConnection _conn;

        public DBConnect(string connectionString)
        {
            _conn = new SqlConnection(connectionString);
        }
    }
}
