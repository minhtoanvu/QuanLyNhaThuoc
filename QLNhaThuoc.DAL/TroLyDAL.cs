using Microsoft.Data.SqlClient;

namespace QLNhaThuoc.DAL
{
    public class TroLyDAL : DBConnect
    {
        public TroLyDAL(string connectionString) : base(connectionString) { }

        // ====== Nested DTOs ======

        public class SanPhamTroLy
        {
            public string MaSP { get; set; }
            public string TenSP { get; set; }
            public string DonViTinh { get; set; }
            public int SoLuongTonKho { get; set; }
            public decimal GiaBan { get; set; }
            public string CongDung { get; set; }
            public string ThanhPhan { get; set; }
            public string TenDanhMuc { get; set; }
        }

        public class ThongKeTonKho
        {
            public int TongSanPham { get; set; }
            public int TongSapHet { get; set; }
            public int TongHetHang { get; set; }
            public decimal TongGiaTriKho { get; set; }
        }

        public class SuCoTomTat
        {
            public string MaBCSC { get; set; }
            public string TenKH { get; set; }
            public DateTime NgayGui { get; set; }
            public string NoiDung { get; set; }
        }

        public class KhachHangTomTat
        {
            public string MaKH { get; set; }
            public string HoTen { get; set; }
            public string SoDT { get; set; }
            public int DiemTichLuy { get; set; }
            public string HangThanhVien { get; set; }
        }

        public class SanPhamBanChay
        {
            public string TenSP { get; set; }
            public int SoLuongBan { get; set; }
            public decimal DoanhThu { get; set; }
        }

        // ====== 1. Tìm kiếm sản phẩm theo tên ======
        public List<SanPhamTroLy> TimKiemSanPham(string keyword)
        {
            var list = new List<SanPhamTroLy>();
            string sql = @"SELECT TOP 10 sp.MaSP, sp.TenSP, sp.DonViTinh, sp.SoLuongTonKho, sp.GiaBan, 
                           sp.CongDung, sp.ThanhPhan, dm.TenDM AS TenDanhMuc
                           FROM SanPham sp
                           LEFT JOIN DanhMuc dm ON sp.MaDM = dm.MaDM
                           WHERE sp.TenSP LIKE @keyword
                           ORDER BY sp.TenSP";
            try
            {
                _conn.Open();
                using (var cmd = new SqlCommand(sql, _conn))
                {
                    cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            list.Add(MapSanPham(r));
                }
            }
            finally { _conn.Close(); }
            return list;
        }

        // ====== 1.5. Tìm kiếm sản phẩm theo công dụng / triệu chứng ======
        public List<SanPhamTroLy> GoiYThuocTheoTrieuChung(string trieuChung)
        {
            var list = new List<SanPhamTroLy>();
            string sql = @"SELECT TOP 10 sp.MaSP, sp.TenSP, sp.DonViTinh, sp.SoLuongTonKho, sp.GiaBan, 
                           sp.CongDung, sp.ThanhPhan, dm.TenDM AS TenDanhMuc
                           FROM SanPham sp
                           LEFT JOIN DanhMuc dm ON sp.MaDM = dm.MaDM
                           WHERE sp.CongDung LIKE @keyword OR dm.TenDM LIKE @keyword
                           ORDER BY sp.SoLuongTonKho DESC";
            try
            {
                _conn.Open();
                using (var cmd = new SqlCommand(sql, _conn))
                {
                    cmd.Parameters.AddWithValue("@keyword", "%" + trieuChung + "%");
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            list.Add(MapSanPham(r));
                }
            }
            finally { _conn.Close(); }
            return list;
        }

        // ====== 2. Tra cứu tồn kho 1 sản phẩm ======
        public SanPhamTroLy GetTonKho(string tenSP)
        {
            SanPhamTroLy sp = null;
            string sql = @"SELECT TOP 1 sp.MaSP, sp.TenSP, sp.DonViTinh, sp.SoLuongTonKho, sp.GiaBan, 
                           sp.CongDung, sp.ThanhPhan, dm.TenDM AS TenDanhMuc
                           FROM SanPham sp
                           LEFT JOIN DanhMuc dm ON sp.MaDM = dm.MaDM
                           WHERE sp.TenSP LIKE @keyword
                           ORDER BY CASE WHEN sp.TenSP = @exact THEN 0 ELSE 1 END";
            try
            {
                _conn.Open();
                using (var cmd = new SqlCommand(sql, _conn))
                {
                    cmd.Parameters.AddWithValue("@keyword", "%" + tenSP + "%");
                    cmd.Parameters.AddWithValue("@exact", tenSP);
                    using (var r = cmd.ExecuteReader())
                        if (r.Read()) sp = MapSanPham(r);
                }
            }
            finally { _conn.Close(); }
            return sp;
        }

        // ====== 3. Sản phẩm sắp hết (< ngưỡng) ======
        public List<SanPhamTroLy> GetSanPhamSapHet(int nguong = 20)
        {
            var list = new List<SanPhamTroLy>();
            string sql = @"SELECT sp.MaSP, sp.TenSP, sp.DonViTinh, sp.SoLuongTonKho, sp.GiaBan, 
                           sp.CongDung, sp.ThanhPhan, dm.TenDM AS TenDanhMuc
                           FROM SanPham sp
                           LEFT JOIN DanhMuc dm ON sp.MaDM = dm.MaDM
                           WHERE sp.SoLuongTonKho < @nguong AND sp.SoLuongTonKho > 0
                           ORDER BY sp.SoLuongTonKho ASC";
            try
            {
                _conn.Open();
                using (var cmd = new SqlCommand(sql, _conn))
                {
                    cmd.Parameters.AddWithValue("@nguong", nguong);
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            list.Add(MapSanPham(r));
                }
            }
            finally { _conn.Close(); }
            return list;
        }

        // ====== 4. Sản phẩm hết hàng ======
        public List<SanPhamTroLy> GetSanPhamHetHang()
        {
            var list = new List<SanPhamTroLy>();
            string sql = @"SELECT sp.MaSP, sp.TenSP, sp.DonViTinh, sp.SoLuongTonKho, sp.GiaBan, 
                           sp.CongDung, sp.ThanhPhan, dm.TenDM AS TenDanhMuc
                           FROM SanPham sp
                           LEFT JOIN DanhMuc dm ON sp.MaDM = dm.MaDM
                           WHERE sp.SoLuongTonKho = 0
                           ORDER BY sp.TenSP";
            try
            {
                _conn.Open();
                using (var cmd = new SqlCommand(sql, _conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add(MapSanPham(r));
            }
            finally { _conn.Close(); }
            return list;
        }

        // ====== 5. Thống kê tồn kho tổng quan ======
        public ThongKeTonKho GetThongKeTonKho()
        {
            var data = new ThongKeTonKho();
            try
            {
                _conn.Open();

                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM SanPham", _conn))
                    data.TongSanPham = Convert.ToInt32(cmd.ExecuteScalar());

                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM SanPham WHERE SoLuongTonKho < 20 AND SoLuongTonKho > 0", _conn))
                    data.TongSapHet = Convert.ToInt32(cmd.ExecuteScalar());

                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM SanPham WHERE SoLuongTonKho = 0", _conn))
                    data.TongHetHang = Convert.ToInt32(cmd.ExecuteScalar());

                using (var cmd = new SqlCommand("SELECT ISNULL(SUM(CAST(GiaBan AS DECIMAL(18,2)) * SoLuongTonKho), 0) FROM SanPham", _conn))
                {
                    var res = cmd.ExecuteScalar();
                    data.TongGiaTriKho = res != DBNull.Value ? Convert.ToDecimal(res) : 0;
                }
            }
            finally { _conn.Close(); }
            return data;
        }

        // ====== 6. Sự cố chờ xử lý ======
        public (int soLuong, List<SuCoTomTat> danhSach) GetSuCoChoXuLy()
        {
            int count = 0;
            var list = new List<SuCoTomTat>();
            try
            {
                _conn.Open();

                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM BaoCaoSuCo WHERE TrangThai = N'Chờ xử lý'", _conn))
                    count = Convert.ToInt32(cmd.ExecuteScalar());

                string sql = @"SELECT TOP 5 b.MaBCSC, k.Ho + ' ' + k.Ten AS TenKH, b.NgayGui, b.NoiDung
                               FROM BaoCaoSuCo b
                               LEFT JOIN KhachHang k ON b.MaKH = k.MaKH
                               WHERE b.TrangThai = N'Chờ xử lý'
                               ORDER BY b.NgayGui DESC";
                using (var cmd = new SqlCommand(sql, _conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add(new SuCoTomTat
                        {
                            MaBCSC = r["MaBCSC"].ToString(),
                            TenKH = r["TenKH"] != DBNull.Value ? r["TenKH"].ToString() : "Khách Lẻ",
                            NgayGui = Convert.ToDateTime(r["NgayGui"]),
                            NoiDung = r["NoiDung"] != DBNull.Value
                                ? (r["NoiDung"].ToString().Length > 100 ? r["NoiDung"].ToString().Substring(0, 100) + "..." : r["NoiDung"].ToString())
                                : ""
                        });
            }
            finally { _conn.Close(); }
            return (count, list);
        }

        // ====== 7. Doanh thu hôm nay ======
        public (decimal doanhThu, int soDon) GetDoanhThuHomNay()
        {
            decimal dt = 0;
            int sd = 0;
            try
            {
                _conn.Open();
                using (var cmd = new SqlCommand(
                    "SELECT ISNULL(SUM(TongTien),0) AS DT, COUNT(MaHD) AS SD FROM HoaDon WHERE CAST(NgayXuatHD AS DATE) = CAST(GETDATE() AS DATE)",
                    _conn))
                using (var r = cmd.ExecuteReader())
                    if (r.Read())
                    {
                        dt = r["DT"] != DBNull.Value ? Convert.ToDecimal(r["DT"]) : 0;
                        sd = r["SD"] != DBNull.Value ? Convert.ToInt32(r["SD"]) : 0;
                    }
            }
            finally { _conn.Close(); }
            return (dt, sd);
        }

        // ====== 8. Doanh thu tháng ======
        public decimal GetDoanhThuThang()
        {
            decimal dt = 0;
            try
            {
                _conn.Open();
                using (var cmd = new SqlCommand(
                    "SELECT ISNULL(SUM(TongTien),0) FROM HoaDon WHERE MONTH(NgayXuatHD)=MONTH(GETDATE()) AND YEAR(NgayXuatHD)=YEAR(GETDATE())",
                    _conn))
                {
                    var res = cmd.ExecuteScalar();
                    dt = res != DBNull.Value ? Convert.ToDecimal(res) : 0;
                }
            }
            finally { _conn.Close(); }
            return dt;
        }

        // ====== 9. Tổng khách hàng ======
        public int GetTongKhachHang()
        {
            int count = 0;
            try
            {
                _conn.Open();
                using (var cmd = new SqlCommand("SELECT COUNT(MaKH) FROM KhachHang", _conn))
                    count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            finally { _conn.Close(); }
            return count;
        }

        // ====== 10. Tìm khách hàng ======
        public List<KhachHangTomTat> TimKhachHang(string keyword)
        {
            var list = new List<KhachHangTomTat>();
            string sql = @"SELECT TOP 5 MaKH, Ho + ' ' + Ten AS HoTen, SoDT, DiemTichLuy, HangThanhVien
                           FROM KhachHang
                           WHERE Ho + ' ' + Ten LIKE @kw OR SoDT LIKE @kw
                           ORDER BY Ho, Ten";
            try
            {
                _conn.Open();
                using (var cmd = new SqlCommand(sql, _conn))
                {
                    cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            list.Add(new KhachHangTomTat
                            {
                                MaKH = r["MaKH"].ToString(),
                                HoTen = r["HoTen"].ToString(),
                                SoDT = r["SoDT"] != DBNull.Value ? r["SoDT"].ToString() : "",
                                DiemTichLuy = r["DiemTichLuy"] != DBNull.Value ? Convert.ToInt32(r["DiemTichLuy"]) : 0,
                                HangThanhVien = r["HangThanhVien"] != DBNull.Value ? r["HangThanhVien"].ToString() : "Mới"
                            });
                }
            }
            finally { _conn.Close(); }
            return list;
        }

        // ====== 11. Top 5 sản phẩm bán chạy tháng này ======
        public List<SanPhamBanChay> GetTop5SanPhamBanChay()
        {
            var list = new List<SanPhamBanChay>();
            string sql = @"SELECT TOP 5 sp.TenSP, SUM(ct.SoLuong) AS TongBan, SUM(ct.ThanhTien) AS TongDT
                           FROM ChiTietHoaDon ct
                           INNER JOIN HoaDon hd ON ct.MaHD = hd.MaHD
                           INNER JOIN SanPham sp ON ct.MaSP = sp.MaSP
                           WHERE MONTH(hd.NgayXuatHD) = MONTH(GETDATE()) AND YEAR(hd.NgayXuatHD) = YEAR(GETDATE())
                           GROUP BY sp.TenSP
                           ORDER BY TongBan DESC";
            try
            {
                _conn.Open();
                using (var cmd = new SqlCommand(sql, _conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add(new SanPhamBanChay
                        {
                            TenSP = r["TenSP"].ToString(),
                            SoLuongBan = Convert.ToInt32(r["TongBan"]),
                            DoanhThu = Convert.ToDecimal(r["TongDT"])
                        });
            }
            finally { _conn.Close(); }
            return list;
        }

        // ====== Helper: Map SqlDataReader → SanPhamTroLy ======
        private SanPhamTroLy MapSanPham(SqlDataReader r)
        {
            return new SanPhamTroLy
            {
                MaSP = r["MaSP"].ToString(),
                TenSP = r["TenSP"].ToString(),
                DonViTinh = r["DonViTinh"] != DBNull.Value ? r["DonViTinh"].ToString() : "",
                SoLuongTonKho = r["SoLuongTonKho"] != DBNull.Value ? Convert.ToInt32(r["SoLuongTonKho"]) : 0,
                GiaBan = r["GiaBan"] != DBNull.Value ? Convert.ToDecimal(r["GiaBan"]) : 0,
                CongDung = r["CongDung"] != DBNull.Value ? r["CongDung"].ToString() : "",
                ThanhPhan = r["ThanhPhan"] != DBNull.Value ? r["ThanhPhan"].ToString() : "",
                TenDanhMuc = r["TenDanhMuc"] != DBNull.Value ? r["TenDanhMuc"].ToString() : ""
            };
        }
    }
}
