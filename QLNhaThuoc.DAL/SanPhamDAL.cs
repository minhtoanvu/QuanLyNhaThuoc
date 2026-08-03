using Microsoft.Data.SqlClient;
using QLNhaThuoc.DTO;

namespace QLNhaThuoc.DAL
{
    public class SanPhamDAL : DBConnect
    {
        public SanPhamDAL(string connectionString) : base(connectionString) { }

        public List<SanPham> GetAll()
        {
            var list = new List<SanPham>();
            string sql = "SELECT MaSP, TenSP, DonViTinh, ThanhPhan, CongDung, GiaBan, SoLuongTonKho, MoTa, MaDM FROM SanPham";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                try
                {
                    _conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            list.Add(MapReader(reader));
                    }
                }
                finally { _conn.Close(); }
            }
            return list;
        }

        public SanPham GetById(string maSP)
        {
            SanPham sp = null;
            string sql = "SELECT MaSP, TenSP, DonViTinh, ThanhPhan, CongDung, GiaBan, SoLuongTonKho, MoTa, MaDM FROM SanPham WHERE MaSP = @MaSP";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                cmd.Parameters.AddWithValue("@MaSP", maSP);
                try
                {
                    _conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) sp = MapReader(reader);
                    }
                }
                finally { _conn.Close(); }
            }
            return sp;
        }

        public bool Insert(SanPham sp)
        {
            string sql = @"INSERT INTO SanPham (MaSP, TenSP, MaDM, DonViTinh, ThanhPhan, GiaBan, SoLuongTonKho, MoTa) 
                           VALUES (@MaSP, @TenSP, @MaDM, @DonViTinh, @ThanhPhan, @GiaBan, @SoLuongTonKho, @MoTa)";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                AddSanPhamParams(cmd, sp);
                try
                {
                    _conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                finally { _conn.Close(); }
            }
        }

        public bool Update(SanPham sp)
        {
            string sql = @"UPDATE SanPham SET TenSP=@TenSP, MaDM=@MaDM, DonViTinh=@DonViTinh, 
                           ThanhPhan=@ThanhPhan, GiaBan=@GiaBan, SoLuongTonKho=@SoLuongTonKho, MoTa=@MoTa
                           WHERE MaSP=@MaSP";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                AddSanPhamParams(cmd, sp);
                try
                {
                    _conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                finally { _conn.Close(); }
            }
        }

        public bool Delete(string maSP)
        {
            string sql = "DELETE FROM SanPham WHERE MaSP = @MaSP";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                cmd.Parameters.AddWithValue("@MaSP", maSP);
                try
                {
                    _conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                finally { _conn.Close(); }
            }
        }

        private SanPham MapReader(SqlDataReader reader)
        {
            return new SanPham
            {
                MaSP = reader["MaSP"].ToString(),
                TenSP = reader["TenSP"].ToString(),
                DonViTinh = reader["DonViTinh"] != DBNull.Value ? reader["DonViTinh"].ToString() : null,
                ThanhPhan = reader["ThanhPhan"] != DBNull.Value ? reader["ThanhPhan"].ToString() : null,
                CongDung = reader["CongDung"] != DBNull.Value ? reader["CongDung"].ToString() : null,
                GiaBan = reader["GiaBan"] != DBNull.Value ? Convert.ToDecimal(reader["GiaBan"]) : 0,
                SoLuongTonKho = reader["SoLuongTonKho"] != DBNull.Value ? Convert.ToInt32(reader["SoLuongTonKho"]) : 0,
                MoTa = reader["MoTa"] != DBNull.Value ? reader["MoTa"].ToString() : null,
                MaDM = reader["MaDM"] != DBNull.Value ? reader["MaDM"].ToString() : null
            };
        }

        private void AddSanPhamParams(SqlCommand cmd, SanPham sp)
        {
            cmd.Parameters.AddWithValue("@MaSP", sp.MaSP ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@TenSP", sp.TenSP ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@MaDM", sp.MaDM ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DonViTinh", sp.DonViTinh ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ThanhPhan", sp.ThanhPhan ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@GiaBan", sp.GiaBan);
            cmd.Parameters.AddWithValue("@SoLuongTonKho", sp.SoLuongTonKho);
            cmd.Parameters.AddWithValue("@MoTa", sp.MoTa ?? (object)DBNull.Value);
        }
    }
}
