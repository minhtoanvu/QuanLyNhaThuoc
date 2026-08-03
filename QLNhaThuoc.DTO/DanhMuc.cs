using System.ComponentModel.DataAnnotations;

namespace QLNhaThuoc.DTO
{
    public class DanhMuc
    {
        [Key]
        [StringLength(20)]
        public string MaDM { get; set; }

        [Required(ErrorMessage = "Tru?ng này là b?t bu?c nh?p")]
        [StringLength(30)]
        public string TenDM { get; set; }

        public string MoTa { get; set; }
    }
}
