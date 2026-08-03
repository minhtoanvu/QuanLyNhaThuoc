using System;
using System.Collections.Generic;

namespace QLNhaThuoc.DTO
{
    public class PhieuXuatInfoDTO
    {
        public string MaPXK { get; set; }
        public DateTime NgayXuat { get; set; }
        public string TenKho { get; set; }
        public string TenNV { get; set; }
        public decimal TongTien { get; set; }
        public List<PhieuXuatDetailDTO> ChiTiet { get; set; } = new List<PhieuXuatDetailDTO>();
    }

    public class PhieuXuatDetailDTO
    {
        public string MaPXK { get; set; }
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public string DonViTinh { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaXuat { get; set; }
        public decimal ThanhTien => SoLuong * GiaXuat;
    }
}
