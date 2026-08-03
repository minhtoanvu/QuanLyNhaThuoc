using System.ComponentModel.DataAnnotations;

namespace QLNhaThuoc.DTO
{
    public class PhieuNhapKho
    {
        [Key]
        [StringLength(20)]
        public string MaPNK { get; set; }

        public DateTime NgayNhap { get; set; }

        public decimal TongTienNhap { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; }

        [StringLength(20)]
        public string MaNV { get; set; }

        [StringLength(20)]
        public string MaKho { get; set; }

        [StringLength(20)]
        public string MaNCC { get; set; }
    }
}
