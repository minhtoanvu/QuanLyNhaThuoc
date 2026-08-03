using System.ComponentModel.DataAnnotations;

namespace QLNhaThuoc.DTO
{
    public class HoaDon
    {
        [Key]
        [StringLength(20)]
        public string MaHD { get; set; }

        public DateTime NgayXuatHD { get; set; }

        [StringLength(20)]
        public string MaNV { get; set; }

        [StringLength(20)]
        public string MaKH { get; set; }

        public decimal TongTien { get; set; }

        [StringLength(50)]
        public string PhuongThucThanhToan { get; set; }

        public int DiemSuDung { get; set; }

        public decimal TienGiamTru { get; set; }

        // Navigation (dùng trong View)
        public string TenKhachHang { get; set; }
        public string TenNhanVien { get; set; }

        // Dùng trong BanHang
        public KhachHang KhachHang { get; set; }
    }
}
