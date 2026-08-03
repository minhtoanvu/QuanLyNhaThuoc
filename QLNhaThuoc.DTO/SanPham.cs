using System.ComponentModel.DataAnnotations;

namespace QLNhaThuoc.DTO
{
    public class SanPham
    {
        [Key]
        [Required(ErrorMessage = "Trường này là bắt buộc nhập")]
        [StringLength(20)]
        public string MaSP { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc nhập")]
        [StringLength(100)]
        public string TenSP { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc nhập")]
        [StringLength(50)]
        public string DonViTinh { get; set; }

        public string ThanhPhan { get; set; }

        public string CongDung { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc nhập")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0")]
        public decimal GiaBan { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc nhập")]
        public int SoLuongTonKho { get; set; }

        public string MoTa { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc nhập")]
        [StringLength(20)]
        public string MaDM { get; set; }

        // Navigation (dÃ¹ng trong View náº¿u cáº§n)
        public string TenDM { get; set; }
    }
}
