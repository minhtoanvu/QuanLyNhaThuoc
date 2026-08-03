using System.ComponentModel.DataAnnotations;

namespace QLNhaThuoc.DTO
{
    public class BaoCaoSuCo
    {
        [Key]
        [StringLength(20)]
        public string MaBCSC { get; set; }

        public DateTime NgayGui { get; set; }

        public string NoiDung { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; }

        [StringLength(20)]
        public string MaHD { get; set; }

        [StringLength(20)]
        public string MaNV { get; set; }

        [StringLength(20)]
        public string MaKH { get; set; }

        // Navigation (dùng trong View)
        public string TenKhachHang { get; set; }
    }
}
