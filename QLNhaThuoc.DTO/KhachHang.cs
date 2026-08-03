using System.ComponentModel.DataAnnotations;

namespace QLNhaThuoc.DTO
{
    public class KhachHang
    {
        [Key]
        [StringLength(20)]
        public string MaKH { get; set; }

        [Required(ErrorMessage = "Tru?ng này là b?t bu?c nh?p")]
        [StringLength(30)]
        public string Ho { get; set; }

        [Required(ErrorMessage = "Tru?ng này là b?t bu?c nh?p")]
        [StringLength(20)]
        public string Ten { get; set; }

        [StringLength(10)]
        public string SoDT { get; set; }

        public string Email { get; set; }

        public string DiaChi { get; set; }

        public int DiemTichLuy { get; set; }

        [StringLength(50)]
        public string HangThanhVien { get; set; }
    }
}
