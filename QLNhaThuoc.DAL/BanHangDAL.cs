using Microsoft.Data.SqlClient;
using QLNhaThuoc.DTO;

namespace QLNhaThuoc.DAL
{
    public class BanHangDAL : DBConnect
    {
        public BanHangDAL(string connectionString) : base(connectionString) { }

        public List<SanPhamBanDTO> GetSanPhamBan()
        {
            var list = new List<SanPhamBanDTO>();
            string sql = "SELECT s.MaSP, s.TenSP, d.TenDM, s.DonViTinh, s.GiaBan, s.SoLuongTonKho FROM SanPham s LEFT JOIN DanhMuc d ON s.MaDM = d.MaDM WHERE s.SoLuongTonKho > 0";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                try
                {
                    _conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                            list.Add(new SanPhamBanDTO
                            {
                                maSP = r["MaSP"].ToString(),
                                tenSP = r["TenSP"].ToString(),
                                tenDM = r["TenDM"] != DBNull.Value ? r["TenDM"].ToString() : "",
                                donViTinh = r["DonViTinh"].ToString(),
                                giaBan = r["GiaBan"] != DBNull.Value ? Convert.ToDecimal(r["GiaBan"]) : 0,
                                soLuongTonKho = r["SoLuongTonKho"] != DBNull.Value ? Convert.ToInt32(r["SoLuongTonKho"]) : 0
                            });
                    }
                }
                finally { _conn.Close(); }
            }
            return list;
        }

        public List<KhachHangBanDTO> GetKhachHangBan()
        {
            var list = new List<KhachHangBanDTO>();
            string sql = "SELECT MaKH, Ho, Ten, SoDT FROM KhachHang";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                try
                {
                    _conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                            list.Add(new KhachHangBanDTO
                            {
                                maKH = r["MaKH"].ToString(),
                                hoTen = r["Ho"].ToString() + " " + r["Ten"].ToString(),
                                soDT = r["SoDT"].ToString()
                            });
                    }
                }
                finally { _conn.Close(); }
            }
            return list;
        }

        public List<HoaDon> GetLichSuHoaDon()
        {
            var list = new List<HoaDon>();
            string sql = @"SELECT h.MaHD, h.NgayXuatHD, h.TongTien, k.Ho, k.Ten 
                           FROM HoaDon h 
                           LEFT JOIN KhachHang k ON h.MaKH = k.MaKH 
                           ORDER BY h.NgayXuatHD DESC";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                try
                {
                    _conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var hd = new HoaDon
                            {
                                MaHD = r["MaHD"].ToString(),
                                NgayXuatHD = Convert.ToDateTime(r["NgayXuatHD"]),
                                TongTien = Convert.ToDecimal(r["TongTien"])
                            };
                            if (r["Ho"] != DBNull.Value)
                            {
                                hd.KhachHang = new KhachHang
                                {
                                    Ho = r["Ho"].ToString(),
                                    Ten = r["Ten"].ToString()
                                };
                            }
                            list.Add(hd);
                        }
                    }
                }
                finally { _conn.Close(); }
            }
            return list;
        }

        public List<ChiTietHoaDon> GetChiTietHoaDon(string maHD)
        {
            var list = new List<ChiTietHoaDon>();
            string sql = @"SELECT c.MaSP, s.TenSP, c.SoLuong, c.DonGia, c.ThanhTien
                           FROM ChiTietHoaDon c
                           INNER JOIN SanPham s ON c.MaSP = s.MaSP
                           WHERE c.MaHD = @MaHD";
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                cmd.Parameters.AddWithValue("@MaHD", maHD);
                try
                {
                    _conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                            list.Add(new ChiTietHoaDon
                            {
                                MaSP = r["MaSP"].ToString(),
                                SoLuong = Convert.ToInt32(r["SoLuong"]),
                                DonGia = Convert.ToDecimal(r["DonGia"]),
                                ThanhTien = Convert.ToDecimal(r["ThanhTien"]),
                                SanPham = new SanPham { TenSP = r["TenSP"].ToString() }
                            });
                    }
                }
                finally { _conn.Close(); }
            }
            return list;
        }

        /// <summary>
        /// Thực hiện thanh toán hoàn chỉnh trong 1 transaction:
        /// Kiểm tra tồn kho → Tạo HoaDon → Tạo ChiTietHoaDon → Cập nhật SoLuongTonKho
        /// </summary>
        /// <returns>MaHD nếu thành công, ném exception nếu thất bại</returns>
        public async Task<string> Checkout(CheckoutDTO data)
        {
            string maHD = "HD" + DateTime.Now.ToString("yyMMddHHmmss");
            decimal tongTien = data.Items.Sum(x => x.ThanhTien) - data.TienGiamTru;

            await _conn.OpenAsync();
            using (SqlTransaction trans = _conn.BeginTransaction())
            {
                try
                {
                    // 1. Kiểm tra tồn kho
                    foreach (var item in data.Items)
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT SoLuongTonKho, TenSP FROM SanPham WHERE MaSP = @MaSP", _conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@MaSP", item.MaSP);
                            using (SqlDataReader r = await cmd.ExecuteReaderAsync())
                            {
                                if (await r.ReadAsync())
                                {
                                    int slTon = Convert.ToInt32(r["SoLuongTonKho"]);
                                    string tenSP = r["TenSP"].ToString();
                                    if (slTon < item.SoLuong)
                                        throw new Exception($"Sản phẩm {tenSP} không đủ số lượng trong kho (Còn: {slTon}).");
                                }
                                else throw new Exception($"Không tìm thấy sản phẩm {item.MaSP}.");
                            }
                        }
                    }

                    // 2. Insert HoaDon
                    string insertHD = @"INSERT INTO HoaDon (MaHD, NgayXuatHD, MaNV, MaKH, TongTien, PhuongThucThanhToan, DiemSuDung, TienGiamTru) 
                                        VALUES (@MaHD, @NgayXuatHD, @MaNV, @MaKH, @TongTien, @PTTT, @DiemSuDung, @TienGiamTru)";
                    using (SqlCommand cmd = new SqlCommand(insertHD, _conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@MaHD", maHD);
                        cmd.Parameters.AddWithValue("@NgayXuatHD", DateTime.Now);
                        cmd.Parameters.AddWithValue("@MaNV", data.MaNV);
                        cmd.Parameters.AddWithValue("@MaKH", (object)data.MaKH ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TongTien", tongTien);
                        cmd.Parameters.AddWithValue("@PTTT", data.PhuongThucThanhToan ?? "Tiền mặt");
                        cmd.Parameters.AddWithValue("@DiemSuDung", data.DiemSuDung);
                        cmd.Parameters.AddWithValue("@TienGiamTru", data.TienGiamTru);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    // 3. Insert ChiTietHoaDon & Update SanPham
                    foreach (var item in data.Items)
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia, ThanhTien) VALUES (@MaHD, @MaSP, @SoLuong, @DonGia, @ThanhTien)",
                            _conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@MaHD", maHD);
                            cmd.Parameters.AddWithValue("@MaSP", item.MaSP);
                            cmd.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                            cmd.Parameters.AddWithValue("@DonGia", item.DonGia);
                            cmd.Parameters.AddWithValue("@ThanhTien", item.ThanhTien);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE SanPham SET SoLuongTonKho = SoLuongTonKho - @SoLuong WHERE MaSP = @MaSP",
                            _conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                            cmd.Parameters.AddWithValue("@MaSP", item.MaSP);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    await trans.CommitAsync();
                    return maHD;
                }
                catch
                {
                    await trans.RollbackAsync();
                    throw;
                }
                finally { _conn.Close(); }
            }
        }
        public (HoaDon, List<ChiTietHoaDon>) GetHoaDonInfo(string maHD)
        {
            HoaDon hoaDon = null;
            var chiTiet = new List<ChiTietHoaDon>();

            // Lấy thông tin hóa đơn + tên khách hàng + tên nhân viên
            string sqlHD = @"SELECT h.MaHD, h.NgayXuatHD, h.TongTien, h.PhuongThucThanhToan, 
                                    h.DiemSuDung, h.TienGiamTru,
                                    ISNULL(k.Ho + ' ' + k.Ten, N'Khách lẻ') AS TenKH,
                                    ISNULL(nv.Ho + ' ' + nv.Ten, '') AS TenNV
                             FROM HoaDon h
                             LEFT JOIN KhachHang k ON h.MaKH = k.MaKH
                             LEFT JOIN NhanVien nv ON h.MaNV = nv.MaNV
                             WHERE h.MaHD = @MaHD";
            using (SqlCommand cmd = new SqlCommand(sqlHD, _conn))
            {
                cmd.Parameters.AddWithValue("@MaHD", maHD);
                try
                {
                    _conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            hoaDon = new HoaDon
                            {
                                MaHD = r["MaHD"].ToString(),
                                NgayXuatHD = Convert.ToDateTime(r["NgayXuatHD"]),
                                TongTien = Convert.ToDecimal(r["TongTien"]),
                                PhuongThucThanhToan = r["PhuongThucThanhToan"].ToString(),
                                DiemSuDung = Convert.ToInt32(r["DiemSuDung"]),
                                TienGiamTru = Convert.ToDecimal(r["TienGiamTru"]),
                                TenKhachHang = r["TenKH"].ToString(),
                                TenNhanVien = r["TenNV"].ToString()
                            };
                        }
                    }
                }
                finally { _conn.Close(); }
            }

            if (hoaDon == null) return (null, chiTiet);

            // Lấy chi tiết hóa đơn
            string sqlCT = @"SELECT c.MaSP, s.TenSP, s.DonViTinh, c.SoLuong, c.DonGia, c.ThanhTien
                             FROM ChiTietHoaDon c
                             INNER JOIN SanPham s ON c.MaSP = s.MaSP
                             WHERE c.MaHD = @MaHD";
            using (SqlCommand cmd = new SqlCommand(sqlCT, _conn))
            {
                cmd.Parameters.AddWithValue("@MaHD", maHD);
                try
                {
                    _conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                            chiTiet.Add(new ChiTietHoaDon
                            {
                                MaSP = r["MaSP"].ToString(),
                                SoLuong = Convert.ToInt32(r["SoLuong"]),
                                DonGia = Convert.ToDecimal(r["DonGia"]),
                                ThanhTien = Convert.ToDecimal(r["ThanhTien"]),
                                SanPham = new SanPham
                                {
                                    TenSP = r["TenSP"].ToString(),
                                    DonViTinh = r["DonViTinh"].ToString()
                                }
                            });
                    }
                }
                finally { _conn.Close(); }
            }

            return (hoaDon, chiTiet);
        }
    }
}
