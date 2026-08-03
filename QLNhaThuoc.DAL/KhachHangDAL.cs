using Microsoft.Data.SqlClient;
using QLNhaThuoc.DTO;

namespace QLNhaThuoc.DAL
{
    public class KhachHangDAL : DBConnect
    {
        public KhachHangDAL(string connectionString) : base(connectionString) { }

        public List<KhachHang> GetAll()
        {
            var list = new List<KhachHang>();
            string sql = "SELECT MaKH, Ho, Ten, SoDT, Email, DiaChi, DiemTichLuy, HangThanhVien FROM KhachHang";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                try
                {
                    _conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                        while (r.Read()) list.Add(MapReader(r));
                }
                finally { _conn.Close(); }
            }
            return list;
        }

        public KhachHang GetById(string maKH)
        {
            KhachHang kh = null;
            string sql = "SELECT MaKH, Ho, Ten, SoDT, Email, DiaChi, DiemTichLuy, HangThanhVien FROM KhachHang WHERE MaKH = @MaKH";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                cmd.Parameters.AddWithValue("@MaKH", maKH);
                try
                {
                    _conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                        if (r.Read()) kh = MapReader(r);
                }
                finally { _conn.Close(); }
            }
            return kh;
        }

        public bool Insert(KhachHang kh)
        {
            string sql = @"INSERT INTO KhachHang (MaKH, Ho, Ten, SoDT, Email, DiaChi, DiemTichLuy, HangThanhVien) 
                           VALUES (@MaKH, @Ho, @Ten, @SoDT, @Email, @DiaChi, @DiemTichLuy, @HangThanhVien)";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                AddParams(cmd, kh);
                try { _conn.Open(); return cmd.ExecuteNonQuery() > 0; }
                finally { _conn.Close(); }
            }
        }

        public bool Update(KhachHang kh)
        {
            string sql = @"UPDATE KhachHang SET Ho=@Ho, Ten=@Ten, SoDT=@SoDT, Email=@Email, 
                           DiaChi=@DiaChi, DiemTichLuy=@DiemTichLuy, HangThanhVien=@HangThanhVien WHERE MaKH=@MaKH";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                AddParams(cmd, kh);
                try { _conn.Open(); return cmd.ExecuteNonQuery() > 0; }
                finally { _conn.Close(); }
            }
        }

        public bool Delete(string maKH)
        {
            string sql = "DELETE FROM KhachHang WHERE MaKH = @MaKH";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                cmd.Parameters.AddWithValue("@MaKH", maKH);
                try { _conn.Open(); return cmd.ExecuteNonQuery() > 0; }
                finally { _conn.Close(); }
            }
        }

        private KhachHang MapReader(SqlDataReader r)
        {
            return new KhachHang
            {
                MaKH = r["MaKH"].ToString(),
                Ho = r["Ho"].ToString(),
                Ten = r["Ten"].ToString(),
                SoDT = r["SoDT"] != DBNull.Value ? r["SoDT"].ToString() : null,
                Email = r["Email"] != DBNull.Value ? r["Email"].ToString() : null,
                DiaChi = r["DiaChi"] != DBNull.Value ? r["DiaChi"].ToString() : null,
                DiemTichLuy = r["DiemTichLuy"] != DBNull.Value ? Convert.ToInt32(r["DiemTichLuy"]) : 0,
                HangThanhVien = r["HangThanhVien"] != DBNull.Value ? r["HangThanhVien"].ToString() : null
            };
        }

        private void AddParams(SqlCommand cmd, KhachHang kh)
        {
            cmd.Parameters.AddWithValue("@MaKH", kh.MaKH);
            cmd.Parameters.AddWithValue("@Ho", kh.Ho);
            cmd.Parameters.AddWithValue("@Ten", kh.Ten);
            cmd.Parameters.AddWithValue("@SoDT", kh.SoDT ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", kh.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DiaChi", kh.DiaChi ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DiemTichLuy", kh.DiemTichLuy);
            cmd.Parameters.AddWithValue("@HangThanhVien", kh.HangThanhVien ?? (object)DBNull.Value);
        }
    }
}
