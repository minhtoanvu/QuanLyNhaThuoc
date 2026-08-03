using System.ComponentModel.DataAnnotations;

namespace QLNhaThuoc.DTO
{
    public class PhieuXuatKho
    {
        [Key]
        [StringLength(20)]
        public string MaPXK { get; set; }

        public DateTime NgayXuat { get; set; }

        public decimal GiaXuat { get; set; }

        public int SoLuong { get; set; }

        [StringLength(20)]
        public string MaKho { get; set; }

        [StringLength(20)]
        public string MaNV { get; set; }

        [StringLength(20)]
        public string MaSP { get; set; }

        [StringLength(20)]
        public string MaHD { get; set; }
    }
}
