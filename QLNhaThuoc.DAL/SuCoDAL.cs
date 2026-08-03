using Microsoft.Data.SqlClient;
using QLNhaThuoc.DTO;

namespace QLNhaThuoc.DAL
{
    public class SuCoDAL : DBConnect
    {
        public SuCoDAL(string connectionString) : base(connectionString) { }

        public List<BaoCaoSuCo> GetAll()
        {
            var list = new List<BaoCaoSuCo>();
            string sql = @"SELECT b.MaBCSC, b.MaKH, k.Ho + ' ' + k.Ten AS TenKH, b.MaHD, b.NgayGui, b.NoiDung, b.TrangThai 
                           FROM BaoCaoSuCo b
                           LEFT JOIN KhachHang k ON b.MaKH = k.MaKH
                           ORDER BY CASE WHEN b.TrangThai = N'Chờ xử lý' THEN 1 ELSE 2 END, b.NgayGui DESC";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                try
                {
                    _conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                            list.Add(new BaoCaoSuCo
                            {
                                MaBCSC = r["MaBCSC"].ToString(),
                                MaKH = r["MaKH"] != DBNull.Value ? r["MaKH"].ToString() : null,
                                TenKhachHang = r["TenKH"] != DBNull.Value ? r["TenKH"].ToString() : "Khách Lẻ",
                                MaHD = r["MaHD"] != DBNull.Value ? r["MaHD"].ToString() : null,
                                NgayGui = r["NgayGui"] != DBNull.Value ? Convert.ToDateTime(r["NgayGui"]) : DateTime.MinValue,
                                NoiDung = r["NoiDung"] != DBNull.Value ? r["NoiDung"].ToString() : null,
                                TrangThai = r["TrangThai"] != DBNull.Value ? r["TrangThai"].ToString() : null
                            });
                    }
                }
                finally { _conn.Close(); }
            }
            return list;
        }

        // ViewModel cho chi tiết sự cố (dùng trong Process)
        public class ChiTietSuCoDTO
        {
            public string MaBCSC { get; set; }
            public string TenKH { get; set; }
            public string SoDT { get; set; }
            public string MaHD { get; set; }
            public DateTime NgayGui { get; set; }
            public string NoiDung { get; set; }
            public string TrangThai { get; set; }
            public string NguoiXuLy { get; set; }
            public DateTime? NgayXuLy { get; set; }
            public string KetQua { get; set; }
        }

        public ChiTietSuCoDTO GetChiTiet(string maBCSC)
        {
            ChiTietSuCoDTO dto = null;
            string sql = @"SELECT b.MaBCSC, k.Ho + ' ' + k.Ten AS TenKH, k.SoDT, b.MaHD, b.NgayGui, b.NoiDung, b.TrangThai,
                                  x.KetQua, x.NgayXuLy, n.Ho + ' ' + n.Ten AS NguoiXuLy
                           FROM BaoCaoSuCo b
                           LEFT JOIN KhachHang k ON b.MaKH = k.MaKH
                           LEFT JOIN XuLySuCo x ON b.MaBCSC = x.MaBCSC
                           LEFT JOIN NhanVien n ON x.MaNV = n.MaNV
                           WHERE b.MaBCSC = @id";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                cmd.Parameters.AddWithValue("@id", maBCSC);
                try
                {
                    _conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                            dto = new ChiTietSuCoDTO
                            {
                                MaBCSC = r["MaBCSC"].ToString(),
                                TenKH = r["TenKH"] != DBNull.Value ? r["TenKH"].ToString() : "Khách Lẻ",
                                SoDT = r["SoDT"] != DBNull.Value ? r["SoDT"].ToString() : null,
                                MaHD = r["MaHD"] != DBNull.Value ? r["MaHD"].ToString() : null,
                                NgayGui = r["NgayGui"] != DBNull.Value ? Convert.ToDateTime(r["NgayGui"]) : DateTime.MinValue,
                                NoiDung = r["NoiDung"] != DBNull.Value ? r["NoiDung"].ToString() : null,
                                TrangThai = r["TrangThai"] != DBNull.Value ? r["TrangThai"].ToString() : null,
                                KetQua = r["KetQua"] != DBNull.Value ? r["KetQua"].ToString() : null,
                                NguoiXuLy = r["NguoiXuLy"] != DBNull.Value ? r["NguoiXuLy"].ToString() : null,
                                NgayXuLy = r["NgayXuLy"] != DBNull.Value ? Convert.ToDateTime(r["NgayXuLy"]) : (DateTime?)null
                            };
                    }
                }
                finally { _conn.Close(); }
            }
            return dto;
        }

        public class KhachHangSuCoDTO
        {
            public string maKH { get; set; }
            public string tenKH { get; set; }
        }

        public List<object> GetDanhSachKhachHang()
        {
            var list = new List<object>();
            string sql = "SELECT MaKH, Ho + ' ' + Ten AS TenKH FROM KhachHang";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                try
                {
                    _conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                        while (r.Read()) list.Add(new KhachHangSuCoDTO { maKH = r["MaKH"].ToString(), tenKH = r["TenKH"].ToString() });
                }
                finally { _conn.Close(); }
            }
            return list;
        }

        public bool Insert(BaoCaoSuCo sc)
        {
            string sql = @"INSERT INTO BaoCaoSuCo (MaBCSC, MaKH, MaHD, NgayGui, NoiDung, TrangThai) 
                           VALUES (@MaBCSC, @MaKH, @MaHD, @NgayGui, @NoiDung, N'Chờ xử lý')";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                cmd.Parameters.AddWithValue("@MaBCSC", sc.MaBCSC);
                cmd.Parameters.AddWithValue("@MaKH", sc.MaKH ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaHD", sc.MaHD ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NgayGui", sc.NgayGui);
                cmd.Parameters.AddWithValue("@NoiDung", sc.NoiDung);
                try { _conn.Open(); return cmd.ExecuteNonQuery() > 0; }
                finally { _conn.Close(); }
            }
        }

        public bool XuLy(string maBCSC, string ketQua, string maNV, string trangThai)
        {
            _conn.Open();
            using (var trans = _conn.BeginTransaction())
            {
                try
                {
                    using (var cmd = new SqlCommand(
                        "INSERT INTO XuLySuCo (MaBCSC, NgayXuLy, KetQua, MaNV) VALUES (@MaBCSC, @NgayXuLy, @KetQua, @MaNV)",
                        _conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@MaBCSC", maBCSC);
                        cmd.Parameters.AddWithValue("@NgayXuLy", DateTime.Now);
                        cmd.Parameters.AddWithValue("@KetQua", ketQua);
                        cmd.Parameters.AddWithValue("@MaNV", maNV ?? (object)DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new SqlCommand(
                        "UPDATE BaoCaoSuCo SET TrangThai = @TT WHERE MaBCSC = @MaBCSC",
                        _conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@TT", trangThai);
                        cmd.Parameters.AddWithValue("@MaBCSC", maBCSC);
                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                    return true;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
                finally { _conn.Close(); }
            }
        }
    }
}
