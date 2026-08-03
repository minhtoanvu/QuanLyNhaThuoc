using System.ComponentModel.DataAnnotations;

namespace QLNhaThuoc.DTO
{
    public class Kho
    {
        [Key]
        [StringLength(20)]
        public string MaKho { get; set; }

        [StringLength(30)]
        public string TenKho { get; set; }

        public string DiaChi { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; }
    }
}
