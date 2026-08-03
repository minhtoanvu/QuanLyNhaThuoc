using System.ComponentModel.DataAnnotations;

namespace QLNhaThuoc.DTO
{
    public class ChiTietHoaDon
    {
        [StringLength(20)]
        public string MaHD { get; set; }

        [StringLength(20)]
        public string MaSP { get; set; }

        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }

        public decimal ThanhTien { get; set; }

        // Navigation (dùng trong View)
        public string TenSP { get; set; }

        // Dùng trong BanHang khi cần đối tượng SanPham
        public SanPham SanPham { get; set; }
    }
}
