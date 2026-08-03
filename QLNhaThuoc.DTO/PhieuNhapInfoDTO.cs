using System;
using System.Collections.Generic;

namespace QLNhaThuoc.DTO
{
    public class PhieuNhapInfoDTO
    {
        public string MaPNK { get; set; }
        public DateTime NgayNhap { get; set; }
        public decimal TongTienNhap { get; set; }
        public string TrangThai { get; set; }
        public string TenKho { get; set; }
        public string TenNCC { get; set; }
        public string TenNV { get; set; }
        public List<PhieuNhapDetailDTO> ChiTiet { get; set; } = new List<PhieuNhapDetailDTO>();
    }

    public class PhieuNhapDetailDTO
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public string DonViTinh { get; set; }
        public int SoLuongNhap { get; set; }
        public decimal GiaNhap { get; set; }
        public DateTime HanSuDung { get; set; }
        public decimal ThanhTien => SoLuongNhap * GiaNhap;
    }
}
