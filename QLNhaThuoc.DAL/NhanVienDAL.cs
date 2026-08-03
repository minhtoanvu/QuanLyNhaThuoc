using Microsoft.Data.SqlClient;
using QLNhaThuoc.DTO;

namespace QLNhaThuoc.DAL
{
    public class NhanVienDAL : DBConnect
    {
        public NhanVienDAL(string connectionString) : base(connectionString) { }

        public List<NhanVien> GetAll()
        {
            var list = new List<NhanVien>();
            string sql = "SELECT MaNV, Ho, Ten, SoDT, Email, DiaChi, Luong, ChucVu, TenDangNhap FROM NhanVien";
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

        public NhanVien GetById(string maNV)
        {
            NhanVien nv = null;
            string sql = "SELECT MaNV, Ho, Ten, SoDT, Email, DiaChi, Luong, ChucVu, TenDangNhap FROM NhanVien WHERE MaNV = @MaNV";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                cmd.Parameters.AddWithValue("@MaNV", maNV);
                try
                {
                    _conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                        if (r.Read()) nv = MapReader(r);
                }
                finally { _conn.Close(); }
            }
            return nv;
        }

        /// <summary>
        /// Dùng cho đăng nhập — trả về NhanVien nếu đúng username/password
        /// </summary>
        public NhanVien DangNhap(string tenDangNhap, string matKhau)
        {
            NhanVien nv = null;
            string sql = "SELECT MaNV, Ho, Ten, ChucVu FROM NhanVien WHERE TenDangNhap = @TDN AND MatKhau = @MK";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                cmd.Parameters.AddWithValue("@TDN", tenDangNhap);
                cmd.Parameters.AddWithValue("@MK", matKhau);
                try
                {
                    _conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            nv = new NhanVien
                            {
                                MaNV = r["MaNV"].ToString(),
                                Ho = r["Ho"].ToString(),
                                Ten = r["Ten"].ToString(),
                                ChucVu = r["ChucVu"].ToString()
                            };
                        }
                    }
                }
                finally { _conn.Close(); }
            }
            return nv;
        }

        public bool KiemTraTenDangNhap(string tenDangNhap)
        {
            string sql = "SELECT COUNT(1) FROM NhanVien WHERE TenDangNhap = @TDN";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                cmd.Parameters.AddWithValue("@TDN", tenDangNhap);
                try
                {
                    _conn.Open();
                    return (int)cmd.ExecuteScalar() > 0;
                }
                finally { _conn.Close(); }
            }
        }

        public bool Insert(NhanVien nv)
        {
            string sql = @"INSERT INTO NhanVien (MaNV, Ho, Ten, SoDT, Email, DiaChi, Luong, ChucVu, TenDangNhap, MatKhau) 
                           VALUES (@MaNV, @Ho, @Ten, @SoDT, @Email, @DiaChi, @Luong, @ChucVu, @TenDangNhap, @MatKhau)";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                AddParams(cmd, nv, includePassword: true);
                try { _conn.Open(); return cmd.ExecuteNonQuery() > 0; }
                finally { _conn.Close(); }
            }
        }

        public bool Update(NhanVien nv, string matKhauMoi = null)
        {
            string passClause = string.IsNullOrEmpty(matKhauMoi) ? "" : ", MatKhau = @MatKhauMoi";
            string sql = $@"UPDATE NhanVien SET Ho=@Ho, Ten=@Ten, SoDT=@SoDT, Email=@Email, 
                            DiaChi=@DiaChi, Luong=@Luong, ChucVu=@ChucVu{passClause} WHERE MaNV=@MaNV";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                AddParams(cmd, nv, includePassword: false);
                if (!string.IsNullOrEmpty(matKhauMoi))
                    cmd.Parameters.AddWithValue("@MatKhauMoi", matKhauMoi);
                try { _conn.Open(); return cmd.ExecuteNonQuery() > 0; }
                finally { _conn.Close(); }
            }
        }

        public bool Delete(string maNV)
        {
            string sql = "DELETE FROM NhanVien WHERE MaNV = @MaNV";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                cmd.Parameters.AddWithValue("@MaNV", maNV);
                try { _conn.Open(); return cmd.ExecuteNonQuery() > 0; }
                finally { _conn.Close(); }
            }
        }

        private NhanVien MapReader(SqlDataReader r)
        {
            return new NhanVien
            {
                MaNV = r["MaNV"].ToString(),
                Ho = r["Ho"].ToString(),
                Ten = r["Ten"].ToString(),
                SoDT = r["SoDT"] != DBNull.Value ? r["SoDT"].ToString() : null,
                Email = r["Email"] != DBNull.Value ? r["Email"].ToString() : null,
                DiaChi = r["DiaChi"] != DBNull.Value ? r["DiaChi"].ToString() : null,
                Luong = r["Luong"] != DBNull.Value ? Convert.ToDecimal(r["Luong"]) : 0,
                ChucVu = r["ChucVu"] != DBNull.Value ? r["ChucVu"].ToString() : null,
                TenDangNhap = r["TenDangNhap"] != DBNull.Value ? r["TenDangNhap"].ToString() : null
            };
        }

        private void AddParams(SqlCommand cmd, NhanVien nv, bool includePassword)
        {
            cmd.Parameters.AddWithValue("@MaNV", nv.MaNV);
            cmd.Parameters.AddWithValue("@Ho", nv.Ho);
            cmd.Parameters.AddWithValue("@Ten", nv.Ten);
            cmd.Parameters.AddWithValue("@SoDT", nv.SoDT ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", nv.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DiaChi", nv.DiaChi ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Luong", nv.Luong);
            cmd.Parameters.AddWithValue("@ChucVu", nv.ChucVu ?? (object)DBNull.Value);
            if (includePassword)
            {
                cmd.Parameters.AddWithValue("@TenDangNhap", nv.TenDangNhap ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MatKhau", nv.MatKhau ?? (object)DBNull.Value);
            }
        }
    }
}
