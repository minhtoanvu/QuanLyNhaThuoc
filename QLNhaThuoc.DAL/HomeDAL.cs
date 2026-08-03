using Microsoft.Data.SqlClient;

namespace QLNhaThuoc.DAL
{
    public class HomeDAL : DBConnect
    {
        public HomeDAL(string connectionString) : base(connectionString) { }

        public class DashboardData
        {
            public decimal DoanhThuHomNay { get; set; }
            public decimal DoanhThuThang { get; set; }
            public int SoDonHomNay { get; set; }
            public int TongKhachHang { get; set; }
            public int SuCoChoXuLy { get; set; }
            public List<SanPhamTon> SanPhamSapHet { get; set; } = new();
            public List<DoanhThuNgay> DoanhThu7NgayQua { get; set; } = new();
        }

        public class SanPhamTon
        {
            public string Ma { get; set; }
            public string Ten { get; set; }
            public int Ton { get; set; }
        }

        public class DoanhThuNgay
        {
            public string Ngay { get; set; }
            public decimal DoanhThu { get; set; }
        }

        public DashboardData GetDashboard()
        {
            var data = new DashboardData();
            try
            {
                _conn.Open();

                // 1. Doanh thu & đơn hôm nay
                using (var cmd = new SqlCommand(
                    "SELECT ISNULL(SUM(TongTien),0) AS DT, COUNT(MaHD) AS SD FROM HoaDon WHERE CAST(NgayXuatHD AS DATE) = CAST(GETDATE() AS DATE)",
                    _conn))
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        data.DoanhThuHomNay = r["DT"] != DBNull.Value ? Convert.ToDecimal(r["DT"]) : 0;
                        data.SoDonHomNay = r["SD"] != DBNull.Value ? Convert.ToInt32(r["SD"]) : 0;
                    }
                }

                // 2. Doanh thu tháng
                using (var cmd = new SqlCommand(
                    "SELECT ISNULL(SUM(TongTien),0) FROM HoaDon WHERE MONTH(NgayXuatHD)=MONTH(GETDATE()) AND YEAR(NgayXuatHD)=YEAR(GETDATE())",
                    _conn))
                {
                    var res = cmd.ExecuteScalar();
                    data.DoanhThuThang = res != DBNull.Value ? Convert.ToDecimal(res) : 0;
                }

                // 3. Tổng KH & sự cố
                using (var cmd = new SqlCommand("SELECT COUNT(MaKH) FROM KhachHang", _conn))
                    data.TongKhachHang = Convert.ToInt32(cmd.ExecuteScalar());

                using (var cmd = new SqlCommand("SELECT COUNT(MaBCSC) FROM BaoCaoSuCo WHERE TrangThai = N'Chờ xử lý'", _conn))
                    data.SuCoChoXuLy = Convert.ToInt32(cmd.ExecuteScalar());

                // 4. Sản phẩm sắp hết
                using (var cmd = new SqlCommand(
                    "SELECT TOP 6 MaSP, TenSP, SoLuongTonKho FROM SanPham WHERE SoLuongTonKho < 20 ORDER BY SoLuongTonKho ASC",
                    _conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        data.SanPhamSapHet.Add(new SanPhamTon { Ma = r["MaSP"].ToString(), Ten = r["TenSP"].ToString(), Ton = Convert.ToInt32(r["SoLuongTonKho"]) });

                // 5. Doanh thu 7 ngày
                using (var cmd = new SqlCommand(
                    @"SELECT TOP 7 CAST(NgayXuatHD AS DATE) AS Ngay, ISNULL(SUM(TongTien),0) AS DT
                      FROM HoaDon WHERE NgayXuatHD >= DATEADD(day,-7,GETDATE())
                      GROUP BY CAST(NgayXuatHD AS DATE) ORDER BY Ngay ASC",
                    _conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        data.DoanhThu7NgayQua.Add(new DoanhThuNgay { Ngay = Convert.ToDateTime(r["Ngay"]).ToString("dd/MM"), DoanhThu = Convert.ToDecimal(r["DT"]) });
            }
            finally { _conn.Close(); }

            return data;
        }
    }
}
