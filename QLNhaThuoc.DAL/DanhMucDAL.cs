using Microsoft.Data.SqlClient;
using QLNhaThuoc.DTO;

namespace QLNhaThuoc.DAL
{
    public class DanhMucDAL : DBConnect
    {
        public DanhMucDAL(string connectionString) : base(connectionString) { }

        public List<DanhMuc> GetAll()
        {
            var list = new List<DanhMuc>();
            string sql = "SELECT MaDM, TenDM, MoTa FROM DanhMuc";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                try
                {
                    _conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new DanhMuc
                            {
                                MaDM = reader["MaDM"].ToString(),
                                TenDM = reader["TenDM"].ToString(),
                                MoTa = reader["MoTa"] != DBNull.Value ? reader["MoTa"].ToString() : null
                            });
                        }
                    }
                }
                finally { _conn.Close(); }
            }
            return list;
        }

        public DanhMuc GetById(string maDM)
        {
            DanhMuc dm = null;
            string sql = "SELECT MaDM, TenDM, MoTa FROM DanhMuc WHERE MaDM = @MaDM";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                cmd.Parameters.AddWithValue("@MaDM", maDM);
                try
                {
                    _conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dm = new DanhMuc
                            {
                                MaDM = reader["MaDM"].ToString(),
                                TenDM = reader["TenDM"].ToString(),
                                MoTa = reader["MoTa"] != DBNull.Value ? reader["MoTa"].ToString() : null
                            };
                        }
                    }
                }
                finally { _conn.Close(); }
            }
            return dm;
        }

        public bool Insert(DanhMuc dm)
        {
            string sql = "INSERT INTO DanhMuc (MaDM, TenDM, MoTa) VALUES (@MaDM, @TenDM, @MoTa)";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                cmd.Parameters.AddWithValue("@MaDM", dm.MaDM);
                cmd.Parameters.AddWithValue("@TenDM", dm.TenDM);
                cmd.Parameters.AddWithValue("@MoTa", dm.MoTa ?? (object)DBNull.Value);
                try
                {
                    _conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                finally { _conn.Close(); }
            }
        }

        public bool Update(DanhMuc dm)
        {
            string sql = "UPDATE DanhMuc SET TenDM = @TenDM, MoTa = @MoTa WHERE MaDM = @MaDM";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                cmd.Parameters.AddWithValue("@MaDM", dm.MaDM);
                cmd.Parameters.AddWithValue("@TenDM", dm.TenDM);
                cmd.Parameters.AddWithValue("@MoTa", dm.MoTa ?? (object)DBNull.Value);
                try
                {
                    _conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                finally { _conn.Close(); }
            }
        }

        public bool Delete(string maDM)
        {
            string sql = "DELETE FROM DanhMuc WHERE MaDM = @MaDM";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                cmd.Parameters.AddWithValue("@MaDM", maDM);
                try
                {
                    _conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                finally { _conn.Close(); }
            }
        }
    }
}
