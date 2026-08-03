using Microsoft.Data.SqlClient;
using QLNhaThuoc.DTO;

namespace QLNhaThuoc.DAL
{
    public class KhoDAL : DBConnect
    {
        public KhoDAL(string connectionString) : base(connectionString) { }

        public async Task<(List<KhoDTO> kho, List<NccDTO> ncc, List<SpDTO> sp)> GetDanhSachNhap()
        {
            var kho = new List<KhoDTO>();
            var ncc = new List<NccDTO>();
            var sp = new List<SpDTO>();

            await _conn.OpenAsync();
            try
            {
                using (var cmd = new SqlCommand("SELECT MaKho, TenKho FROM Kho", _conn))
                using (var r = await cmd.ExecuteReaderAsync())
                    while (await r.ReadAsync()) kho.Add(new KhoDTO { id = r["MaKho"].ToString(), name = r["TenKho"].ToString() });

                using (var cmd = new SqlCommand("SELECT MaNCC, TenNCC FROM NhaCungCap", _conn))
                using (var r = await cmd.ExecuteReaderAsync())
                    while (await r.ReadAsync()) ncc.Add(new NccDTO { id = r["MaNCC"].ToString(), name = r["TenNCC"].ToString() });

                using (var cmd = new SqlCommand("SELECT MaSP, TenSP, DonViTinh FROM SanPham", _conn))
                using (var r = await cmd.ExecuteReaderAsync())
                    while (await r.ReadAsync()) sp.Add(new SpDTO { id = r["MaSP"].ToString(), name = r["TenSP"].ToString(), unit = r["DonViTinh"].ToString() });
            }
            finally { _conn.Close(); }

            return (kho, ncc, sp);
        }

        /// <summary>
        /// Nhập kho trong 1 transaction: Tạo PhieuNhapKho → Tạo ChiTietNhapKho → Cập nhật SoLuongTonKho
        /// </summary>
        public async Task<string> ProcessNhapKho(NhapKhoDTO data)
        {
            string maPNK = "PNK" + DateTime.Now.ToString("yyMMddHHmmss");
            decimal tongTien = data.Items.Sum(x => x.SoLuong * x.DonGiaNhap);

            await _conn.OpenAsync();
            using (var trans = _conn.BeginTransaction())
            {
                try
                {
                    using (var cmd = new SqlCommand(
                        @"INSERT INTO PhieuNhapKho (MaPNK, NgayNhap, TongTienNhap, TrangThai, MaNV, MaKho, MaNCC)
                          VALUES (@MaPNK, @NgayNhap, @TongTienNhap, N'Hoàn Thành', @MaNV, @MaKho, @MaNCC)",
                        _conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@MaPNK", maPNK);
                        cmd.Parameters.AddWithValue("@NgayNhap", DateTime.Now);
                        cmd.Parameters.AddWithValue("@TongTienNhap", tongTien);
                        cmd.Parameters.AddWithValue("@MaNV", data.MaNV);
                        cmd.Parameters.AddWithValue("@MaKho", data.MaKho);
                        cmd.Parameters.AddWithValue("@MaNCC", data.MaNCC);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    foreach (var item in data.Items)
                    {
                        using (var cmd = new SqlCommand(
                            "INSERT INTO ChiTietNhapKho (MaPNK, MaSP, SoLuongNhap, GiaNhap, HanSuDung) VALUES (@MaPNK, @MaSP, @SoLuong, @DGN, @HSD)",
                            _conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@MaPNK", maPNK);
                            cmd.Parameters.AddWithValue("@MaSP", item.MaSP);
                            cmd.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                            cmd.Parameters.AddWithValue("@DGN", item.DonGiaNhap);
                            cmd.Parameters.AddWithValue("@HSD", item.HanSuDung);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        using (var cmd = new SqlCommand(
                            "UPDATE SanPham SET SoLuongTonKho = SoLuongTonKho + @Qty WHERE MaSP = @MaSP",
                            _conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@Qty", item.SoLuong);
                            cmd.Parameters.AddWithValue("@MaSP", item.MaSP);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    await trans.CommitAsync();
                    return maPNK;
                }
                catch
                {
                    await trans.RollbackAsync();
                    throw;
                }
                finally { _conn.Close(); }
            }
        }

        /// <summary>
        /// Xuất kho trong 1 transaction: Kiểm tra tồn kho → Tạo PhieuXuatKho → Cập nhật SoLuongTonKho
        /// </summary>
        public async Task<string> ProcessXuatKho(XuatKhoDTO data)
        {
            string firstMaPXK = null;
            await _conn.OpenAsync();
            using (var trans = _conn.BeginTransaction())
            {
                try
                {
                    // Kiểm tra tồn kho
                    foreach (var item in data.Items)
                    {
                        using (var cmd = new SqlCommand("SELECT SoLuongTonKho, TenSP FROM SanPham WHERE MaSP = @MaSP", _conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@MaSP", item.MaSP);
                            using (var r = await cmd.ExecuteReaderAsync())
                            {
                                if (await r.ReadAsync())
                                {
                                    int slTon = Convert.ToInt32(r["SoLuongTonKho"]);
                                    if (slTon < item.SoLuong)
                                        throw new Exception($"Sản phẩm {r["TenSP"]} chỉ còn {slTon}.");
                                }
                            }
                        }
                    }

                    foreach (var item in data.Items)
                    {
                        string maPXK = "PXK" + Guid.NewGuid().ToString("N").Substring(0, 5) + DateTime.Now.ToString("mmss");
                        if (firstMaPXK == null) firstMaPXK = maPXK;
                        using (var cmd = new SqlCommand(
                            "INSERT INTO PhieuXuatKho (MaPXK, NgayXuat, GiaXuat, SoLuong, MaKho, MaNV, MaSP) VALUES (@MaPXK, @NgayXuat, @GiaXuat, @SoLuong, @MaKho, @MaNV, @MaSP)",
                            _conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@MaPXK", maPXK);
                            cmd.Parameters.AddWithValue("@NgayXuat", DateTime.Now);
                            cmd.Parameters.AddWithValue("@GiaXuat", item.GiaXuat);
                            cmd.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                            cmd.Parameters.AddWithValue("@MaKho", data.MaKho);
                            cmd.Parameters.AddWithValue("@MaNV", data.MaNV);
                            cmd.Parameters.AddWithValue("@MaSP", item.MaSP);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        using (var cmd = new SqlCommand(
                            "UPDATE SanPham SET SoLuongTonKho = SoLuongTonKho - @Qty WHERE MaSP = @MaSP",
                            _conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@Qty", item.SoLuong);
                            cmd.Parameters.AddWithValue("@MaSP", item.MaSP);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    await trans.CommitAsync();
                    return firstMaPXK;
                }
                catch
                {
                    await trans.RollbackAsync();
                    throw;
                }
                finally { _conn.Close(); }
            }
        }

        public PhieuNhapInfoDTO GetPhieuNhapInfo(string maPNK)
        {
            PhieuNhapInfoDTO phieuNhap = null;

            string sqlPN = @"SELECT p.MaPNK, p.NgayNhap, p.TongTienNhap, p.TrangThai,
                                    k.TenKho, ncc.TenNCC,
                                    ISNULL(nv.Ho + ' ' + nv.Ten, '') AS TenNV
                             FROM PhieuNhapKho p
                             LEFT JOIN Kho k ON p.MaKho = k.MaKho
                             LEFT JOIN NhaCungCap ncc ON p.MaNCC = ncc.MaNCC
                             LEFT JOIN NhanVien nv ON p.MaNV = nv.MaNV
                             WHERE p.MaPNK = @MaPNK";

            using (SqlCommand cmd = new SqlCommand(sqlPN, _conn))
            {
                cmd.Parameters.AddWithValue("@MaPNK", maPNK);
                try
                {
                    _conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            phieuNhap = new PhieuNhapInfoDTO
                            {
                                MaPNK = r["MaPNK"].ToString(),
                                NgayNhap = Convert.ToDateTime(r["NgayNhap"]),
                                TongTienNhap = Convert.ToDecimal(r["TongTienNhap"]),
                                TrangThai = r["TrangThai"].ToString(),
                                TenKho = r["TenKho"].ToString(),
                                TenNCC = r["TenNCC"].ToString(),
                                TenNV = r["TenNV"].ToString()
                            };
                        }
                    }
                }
                finally { _conn.Close(); }
            }

            if (phieuNhap == null) return null;

            string sqlCT = @"SELECT c.MaSP, s.TenSP, s.DonViTinh, c.SoLuongNhap, c.GiaNhap, c.HanSuDung
                             FROM ChiTietNhapKho c
                             INNER JOIN SanPham s ON c.MaSP = s.MaSP
                             WHERE c.MaPNK = @MaPNK";

            using (SqlCommand cmd = new SqlCommand(sqlCT, _conn))
            {
                cmd.Parameters.AddWithValue("@MaPNK", maPNK);
                try
                {
                    _conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            phieuNhap.ChiTiet.Add(new PhieuNhapDetailDTO
                            {
                                MaSP = r["MaSP"].ToString(),
                                TenSP = r["TenSP"].ToString(),
                                DonViTinh = r["DonViTinh"].ToString(),
                                SoLuongNhap = Convert.ToInt32(r["SoLuongNhap"]),
                                GiaNhap = Convert.ToDecimal(r["GiaNhap"]),
                                HanSuDung = Convert.ToDateTime(r["HanSuDung"])
                            });
                        }
                    }
                }
                finally { _conn.Close(); }
            }

            return phieuNhap;
        }

        public PhieuXuatInfoDTO GetPhieuXuatInfo(string maPXK)
        {
            PhieuXuatInfoDTO phieuXuat = null;
            DateTime referenceNgayXuat = DateTime.Now;
            string referenceMaNV = "";
            string referenceMaKho = "";
            string referenceTenKho = "";
            string referenceTenNV = "";

            string sqlRef = @"SELECT p.NgayXuat, p.MaNV, p.MaKho, k.TenKho,
                                     ISNULL(nv.Ho + ' ' + nv.Ten, '') AS TenNV
                              FROM PhieuXuatKho p
                              LEFT JOIN Kho k ON p.MaKho = k.MaKho
                              LEFT JOIN NhanVien nv ON p.MaNV = nv.MaNV
                              WHERE p.MaPXK = @MaPXK";

            using (SqlCommand cmd = new SqlCommand(sqlRef, _conn))
            {
                cmd.Parameters.AddWithValue("@MaPXK", maPXK);
                try
                {
                    _conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            referenceNgayXuat = Convert.ToDateTime(r["NgayXuat"]);
                            referenceMaNV = r["MaNV"].ToString();
                            referenceMaKho = r["MaKho"].ToString();
                            referenceTenKho = r["TenKho"].ToString();
                            referenceTenNV = r["TenNV"].ToString();
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                finally { _conn.Close(); }
            }

            phieuXuat = new PhieuXuatInfoDTO
            {
                MaPXK = maPXK,
                NgayXuat = referenceNgayXuat,
                TenKho = referenceTenKho,
                TenNV = referenceTenNV
            };

            var chiTiet = new List<PhieuXuatDetailDTO>();
            string sqlAll = @"SELECT p.MaPXK, p.NgayXuat, p.SoLuong, p.GiaXuat, s.TenSP, s.DonViTinh, p.MaSP
                              FROM PhieuXuatKho p
                              INNER JOIN SanPham s ON p.MaSP = s.MaSP
                              WHERE p.MaNV = @MaNV 
                                AND p.MaKho = @MaKho 
                                AND ABS(DATEDIFF(second, p.NgayXuat, @NgayXuat)) <= 5";

            using (SqlCommand cmd = new SqlCommand(sqlAll, _conn))
            {
                cmd.Parameters.AddWithValue("@MaNV", referenceMaNV);
                cmd.Parameters.AddWithValue("@MaKho", referenceMaKho);
                cmd.Parameters.AddWithValue("@NgayXuat", referenceNgayXuat);
                try
                {
                    _conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            phieuXuat.ChiTiet.Add(new PhieuXuatDetailDTO
                            {
                                MaPXK = r["MaPXK"].ToString(),
                                MaSP = r["MaSP"].ToString(),
                                TenSP = r["TenSP"].ToString(),
                                DonViTinh = r["DonViTinh"].ToString(),
                                SoLuong = Convert.ToInt32(r["SoLuong"]),
                                GiaXuat = Convert.ToDecimal(r["GiaXuat"])
                            });
                        }
                    }
                }
                finally { _conn.Close(); }
            }

            decimal tongTien = 0;
            foreach (var c in phieuXuat.ChiTiet)
            {
                tongTien += c.ThanhTien;
            }
            phieuXuat.TongTien = tongTien;

            return phieuXuat;
        }
    }
}
