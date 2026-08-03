using System.ComponentModel.DataAnnotations;

namespace QLNhaThuoc.DTO
{
    public class NhanVien
    {
        [Key]
        [StringLength(20)]
        public string MaNV { get; set; }

        [Required(ErrorMessage = "Tru?ng này là b?t bu?c nh?p")]
        [StringLength(30)]
        public string Ho { get; set; }

        [Required(ErrorMessage = "Tru?ng này là b?t bu?c nh?p")]
        [StringLength(20)]
        public string Ten { get; set; }

        public string Email { get; set; }

        [StringLength(10)]
        public string SoDT { get; set; }

        public decimal Luong { get; set; }

        public string DiaChi { get; set; }

        [StringLength(30)]
        public string ChucVu { get; set; }

        [StringLength(50)]
        public string TenDangNhap { get; set; }

        [StringLength(255)]
        public string MatKhau { get; set; }
    }
}
